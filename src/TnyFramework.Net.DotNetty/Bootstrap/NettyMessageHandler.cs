// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.IO;
using System.Threading.Tasks;
using DotNetty.Handlers.Timeout;
using DotNetty.Transport.Channels;
using Microsoft.Extensions.Logging;
using TnyFramework.Common.Exceptions;
using TnyFramework.Common.Logger;
using TnyFramework.Common.Result;
using TnyFramework.Net.Base;
using TnyFramework.Net.Command.Dispatcher.Monitor;
using TnyFramework.Net.DotNetty.Common;
using TnyFramework.Net.Message;
using TnyFramework.Net.Transport;

namespace TnyFramework.Net.DotNetty.Bootstrap
{

    public class NettyMessageHandler : ChannelDuplexHandler
    {
        private static readonly ILogger LOGGER = LogFactory.Logger<NettyMessageHandler>();

        public override bool IsSharable => true;

        private readonly RpcMonitor rpcMonitor;

        public NettyMessageHandler(INetworkContext networkContext)
        {
            rpcMonitor = networkContext.RpcMonitor;
        }

        public override void ChannelActive(IChannelHandlerContext context)
        {
            if (LOGGER.IsEnabled(LogLevel.Information))
            {
                var channel = context.Channel;
                LOGGER.LogInformation(channel.Active ? "[Tunnel] 连接成功 ## 通道 {Remote} ==> {Local}" : "[Tunnel] 连接失败 ## 通道 {Remote} ==> {Local}",
                    channel.RemoteAddress, channel.LocalAddress);
            }
            base.ChannelActive(context);
        }

        public override void ChannelInactive(IChannelHandlerContext context)
        {
            var channel = context.Channel;
            var tunnel = channel.GetAttribute(NettyNetAttrKeys.TUNNEL).GetAndRemove();
            if (tunnel != null)
            {
                if (LOGGER.IsEnabled(LogLevel.Information))
                {
                    LOGGER.LogInformation("[Tunnel] 断开链接 ## 通道 {Remote} ==> {Local} 断开链接", channel.RemoteAddress, channel.LocalAddress);
                }
                if (Equals(tunnel.AccessMode, NetAccessMode.Server))
                {
                    tunnel.Close();
                } else
                {
                    tunnel.Disconnect();
                }
            }
            base.ChannelInactive(context);
        }

        public override void ChannelRead(IChannelHandlerContext context, object data)
        {
            var channel = context.Channel;
            switch (data)
            {
                case null:
                    LOGGER.LogInformation("[Tunnel] {Msg} ## 通道 {Remote} ==> {Local} 断开链接", "消息为null", channel.RemoteAddress, channel.LocalAddress);
                    channel.DisconnectAsync();
                    return;
                case INetMessage message:
                    try
                    {
                        var tunnel = channel.GetAttribute(NettyNetAttrKeys.TUNNEL).Get();
                        if (tunnel != null)
                        {
                            tunnel.Receive(message);
                        } else
                        {
                            // TODO rpcMonitor 处理未找到 tunnel!!
                        }
                    } catch (Exception ex)
                    {
                        LOGGER.LogError(ex, "#GameServerHandler#接受请求异常");
                    }
                    break;
            }
        }

        public override Task WriteAsync(IChannelHandlerContext context, object msg)
        {
            if (!(msg is INetMessage message))
                return context.WriteAsync(msg);
            var channel = context.Channel;
            var tunnel = channel.GetAttribute(NettyNetAttrKeys.TUNNEL).Get();
            rpcMonitor.OnSend(tunnel, message);
            return context.WriteAsync(msg);
        }

        public override void ExceptionCaught(IChannelHandlerContext context, Exception cause)
        {
            var channel = context.Channel;
            if (this == channel.Pipeline.Last())
            {
                switch (cause)
                {
                    case ClosedChannelException _:
                        LOGGER.LogError("[Tunnel] # {Cause} # 客户端连接已断开 # {Msg}", cause.GetType(), cause.Message);
                        break;
                    case WriteTimeoutException _:
                        LOGGER.LogError("[Tunnel]  ## 通道 {Remote} ==> {Local} 断开链接 # cause {Cause} 写出数据超时, message: {Msg}",
                            channel.RemoteAddress, channel.LocalAddress, typeof(WriteTimeoutException), cause.Message);
                        break;
                    case ReadTimeoutException _:
                        LOGGER.LogError("[Tunnel]  ## 通道 {Remote} ==> {Local} 断开链接 # cause {Cause} 读取数据超时, message: {Msg}",
                            channel.RemoteAddress, channel.LocalAddress, typeof(ReadTimeoutException), cause.Message);
                        break;
                    case IOException _:
                        LOGGER.LogError("[Tunnel] # cause {Cause} # {Msg}", cause.GetType(), cause.Message);
                        break;
                    case ResultCodeException ex:
                        HandleResultCodeException(channel, ex.Code, cause);
                        break;
                    default: {
                        LOGGER.LogError(cause, "[Tunnel] ## 通道 {Remote} ==> {Local} 异常", channel.RemoteAddress, channel.LocalAddress);
                        break;
                    }
                }
            } else
            {
                LOGGER.LogError(cause, "[Tunnel] ## 通道 {Remote} ==> {Local} 异常", channel.RemoteAddress, channel.LocalAddress);
            }
            base.ExceptionCaught(context, cause);
        }

        private void HandleResultCodeException(IChannel channel, IResultCode code, Exception cause)
        {
            if (code.Level == ResultLevel.Error)
            {
                LOGGER.LogError(cause, "[Tunnel]  ## 通道 {Remote} ==> {Local} 断开链接 # cause {Code}({CodeValue})[{CodeMsg}], message:{Msg}",
                    channel.RemoteAddress, channel.LocalAddress, code, code.Value, code.Message, cause.Message);
                var tunnel = channel.GetAttribute(NettyNetAttrKeys.TUNNEL).GetAndSet(null!);
                if (tunnel != null)
                {
                    RpcMessageAide.Send(tunnel, MessageContents.Push(Protocols.PUSH, code));
                }
            } else
            {
                LOGGER.LogError(cause, "[Tunnel]  ## 通道 {Remote} ==> {Local} 异常 # cause {Code}({CodeValue})[{CodeMsg}], message:{Msg}",
                    channel.RemoteAddress, channel.LocalAddress, code, code.Value, code.Message, cause.Message);
            }
        }

        // public override void UserEventTriggered(IChannelHandlerContext context, object evt)
        // {
        //     //TODO 空闲超时处理 IdleStateEvent
        //     base.UserEventTriggered(context, evt);
        // }
    }

}
