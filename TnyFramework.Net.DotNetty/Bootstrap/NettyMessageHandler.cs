using System.IO;
using DotNetty.Handlers.Timeout;
using DotNetty.Transport.Channels;
using Microsoft.Extensions.Logging;
using TnyFramework.Common.Exception;
using TnyFramework.Common.Logger;
using TnyFramework.Common.Result;
using TnyFramework.Net.DotNetty.Common;
using TnyFramework.Net.Message;
using TnyFramework.Net.Transport;
namespace TnyFramework.Net.DotNetty.Bootstrap
{
    public class NettyMessageHandler : ChannelDuplexHandler
    {
        private static readonly ILogger LOGGER = LogFactory.Logger<NettyMessageHandler>();

        public override bool IsSharable => true;


        public override void ChannelActive(IChannelHandlerContext context)
        {
            if (LOGGER.IsEnabled(LogLevel.Information))
            {
                var channel = context.Channel;
                if (channel.Active)
                {
                    LOGGER.LogInformation("[Tunnel] 连接成功 ## 通道 {} ==> {}", channel.RemoteAddress, channel.LocalAddress);
                } else
                {
                    LOGGER.LogInformation("[Tunnel] 连接失败 ## 通道 {} ==> {}", channel.RemoteAddress, channel.LocalAddress);
                }
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
                    LOGGER.LogInformation("[Tunnel] 断开链接 ## 通道 {} ==> {} 断开链接", channel.RemoteAddress, channel.LocalAddress);
                }
                if (Equals(tunnel.Mode, TunnelMode.SERVER))
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
                    LOGGER.LogInformation("[Tunnel] {} ## 通道 {} ==> {} 断开链接", "消息为null", channel.RemoteAddress, channel.LocalAddress);
                    channel.DisconnectAsync();
                    return;
                case INetMessage message:
                    try
                    {
                        var tunnel = channel.GetAttribute(NettyNetAttrKeys.TUNNEL).Get();
                        tunnel?.Receive(message);
                    } catch (System.Exception ex)
                    {
                        LOGGER.LogError("#GameServerHandler#接受请求异常", ex);
                    }
                    break;
            }
        }


        public override void ExceptionCaught(IChannelHandlerContext context, System.Exception cause)
        {
            var channel = context.Channel;
            if (this == channel.Pipeline.Last())
            {
                switch (cause)
                {
                    case ClosedChannelException _:
                        LOGGER.LogError("[Tunnel] # {} # 客户端连接已断开 # {}", cause.GetType(), cause.Message);
                        break;
                    case WriteTimeoutException _:
                        LOGGER.LogError("[Tunnel]  ## 通道 {} ==> {} 断开链接 # cause {} 写出数据超时, message: {}",
                            channel.RemoteAddress, channel.LocalAddress, typeof(WriteTimeoutException), cause.Message);
                        break;
                    case ReadTimeoutException _:
                        LOGGER.LogError("[Tunnel]  ## 通道 {} ==> {} 断开链接 # cause {} 读取数据超时, message: {}",
                            channel.RemoteAddress, channel.LocalAddress, typeof(ReadTimeoutException), cause.Message);
                        break;
                    case IOException _:
                        LOGGER.LogError("[Tunnel] # {} # {}", cause.GetType(), cause.Message);
                        break;
                    case ResultCodeException ex:
                        HandleResultCodeException(channel, ex.Code, cause);
                        break;
                    default: {
                        LOGGER.LogError(cause, "[Tunnel] ## 通道 {} ==> {} 异常", channel.RemoteAddress, channel.LocalAddress);
                        break;
                    }
                }
            } else
            {
                LOGGER.LogError(cause, "[Tunnel] ## 通道 {} ==> {} 异常", channel.RemoteAddress, channel.LocalAddress);
            }
            base.ExceptionCaught(context, cause);
        }


        private void HandleResultCodeException(IChannel channel, IResultCode code, System.Exception cause)
        {
            if (code.Level == ResultLevel.Error)
            {
                LOGGER.LogError(cause, "[Tunnel]  ## 通道 {} ==> {} 断开链接 # cause {}({})[{}], message:{}",
                    channel.RemoteAddress, channel.LocalAddress, code, code.Value, code.Message, cause.Message);
                var tunnel = channel.GetAttribute(NettyNetAttrKeys.TUNNEL).GetAndSet(null);
                if (tunnel != null)
                {
                    //TODO 发送关闭消息
                    TunnelAide.ResponseMessage(tunnel, MessageContexts.Push(Protocols.PUSH, code));
                }
            } else
            {
                LOGGER.LogError(cause, "[Tunnel]  ## 通道 {} ==> {} 异常 # cause {}({})[{}], message:{}",
                    channel.RemoteAddress, channel.LocalAddress, code, code.Value, code.Message, cause.Message);
            }
        }


        public override void UserEventTriggered(IChannelHandlerContext context, object evt)
        {
            //TODO 空闲超时处理 IdleStateEvent
            base.UserEventTriggered(context, evt);
        }
    }
}
