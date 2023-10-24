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

        public Task Write(IMessage message)
        {
            return channel.WriteAndFlushAsync(message);
        }

        public Task Write(IMessageAllocator maker, IMessageFactory factory, MessageContent content)
        {
            return Write(maker.Allocate, factory, content);
        }

        public Task Write(MessageAllocator maker, IMessageFactory factory, MessageContent content)
        {
            return channel.EventLoop.SubmitAsync<object?>(() => {
                IMessage? message = null;
                try
                {
                    message = maker(factory, content);
                } catch (Exception e)
                {
                    content.Cancel(e);
                    LOGGER.LogError(e, "");
                }
                var task = channel.WriteAndFlushAsync(message);
                if (content is IMessageWritableContext ctx)
                    ctx.SetWrittenTask(task);
                return default;
            });
        }
    }

}
