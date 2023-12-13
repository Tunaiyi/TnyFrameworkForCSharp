// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.Threading.Tasks;
using DotNetty.Transport.Channels;
using Microsoft.Extensions.Logging;
using TnyFramework.Common.Logger;
using TnyFramework.Net.DotNetty.Common;
using TnyFramework.Net.Endpoint;
using TnyFramework.Net.Message;
using TnyFramework.Net.Transport;

namespace TnyFramework.Net.DotNetty.Transport
{

    public class NettyChannelMessageTransporter : NettyChannelConnection, IMessageTransporter
    {
        private static readonly ILogger LOGGER = LogFactory.Logger<NettyChannelMessageTransporter>();

        public NettyChannelMessageTransporter(NetAccessMode accessMode, IChannel channel) : base(accessMode, channel)
        {
        }

        public void Bind(INetTunnel tunnel)
        {
            channel?.GetAttribute(NettyNetAttrKeys.TUNNEL).Set(tunnel);
        }

        protected override void DoClose()
        {
            var tunnelAttr = channel?.GetAttribute(NettyNetAttrKeys.TUNNEL);
            var tunnel = tunnelAttr?.Get();
            tunnelAttr?.Remove();
            if (tunnel != null && (tunnel.IsOpen() || tunnel.IsActive()))
            {
                tunnel.Disconnect();
            }
        }

        public ValueTask Write(IMessage message, bool waitWritten = false)
        {
            return waitWritten ? ValueTask.CompletedTask : new ValueTask(channel.WriteAndFlushAsync(message));
        }

        public ValueTask Write(IMessageAllocator maker, IMessageFactory factory, MessageContent content, bool waitWritten = false)
        {
            return waitWritten ? ValueTask.CompletedTask : Write(maker.Allocate, factory, content);
        }

        public ValueTask Write(MessageAllocator maker, IMessageFactory factory, MessageContent content, bool waitWritten = false)
        {
            if (waitWritten)
            {
                return ValueTask.CompletedTask;
            }
            return new ValueTask(channel.EventLoop.SubmitAsync(() => {
                IMessage? message = null;
                try
                {
                    message = maker(factory, content);
                } catch (Exception e)
                {
                    content.Cancel(e);
                    LOGGER.LogError(e, "");
                }
                return channel.WriteAndFlushAsync(message);
            }).Unwrap());
        }
    }

}
