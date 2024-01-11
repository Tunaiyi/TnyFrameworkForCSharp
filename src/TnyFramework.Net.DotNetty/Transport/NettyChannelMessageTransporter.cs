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
using TnyFramework.Net.Message;
using TnyFramework.Net.Session;
using TnyFramework.Net.Transport;

namespace TnyFramework.Net.DotNetty.Transport;

public class NettyChannelMessageTransporter : NettyChannelConnection, IMessageTransporter
{
    private static readonly ILogger LOGGER = LogFactory.Logger<NettyChannelMessageTransporter>();

    private INetTunnel tunnel = null!;

    public NettyChannelMessageTransporter(NetAccessMode accessMode, IChannel channel) : base(accessMode, channel)
    {
    }

    public void Bind(INetTunnel tunnel)
    {
        channel.GetAttribute(NettyNetAttrKeys.TUNNEL).Set(tunnel);
        this.tunnel = tunnel;
    }

    protected override void DoClose()
    {
        var tunnelAttr = channel.GetAttribute(NettyNetAttrKeys.TUNNEL);
        var current = tunnelAttr.GetAndRemove();
        if (current != null && (current.IsOpen() || current.IsActive()))
        {
            current.Disconnect();
        }
    }

    public ValueTask Write(IMessage message, bool waitWritten = false)
    {
        var task = channel.WriteAndFlushAsync(message);
        return waitWritten ? new ValueTask(task) : ValueTask.CompletedTask;
    }

    public ValueTask Write(IMessageAllocator maker, MessageContent content, bool waitWritten = false)
    {
        return Write(maker.Allocate, content, waitWritten);
    }

    public ValueTask Write(MessageAllocator maker, MessageContent content, bool waitWritten = false)
    {
        var task = channel.EventLoop.SubmitAsync(() => {
            IMessage? message = null;
            try
            {
                message = maker(tunnel.MessageFactory, content);
            } catch (Exception e)
            {
                content.Cancel(e);
                LOGGER.LogError(e, "");
            }
            return channel.WriteAndFlushAsync(message);
        }).Unwrap();
        return waitWritten ? new ValueTask(task) : ValueTask.CompletedTask;
    }
}
