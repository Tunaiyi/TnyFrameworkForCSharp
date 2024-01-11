// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TnyFramework.Common.Attribute;
using TnyFramework.Common.Logger;
using TnyFramework.Net.Application;
using TnyFramework.Net.Message;
using TnyFramework.Net.Nats.Codecs;
using TnyFramework.Net.Nats.Core;
using TnyFramework.Net.Session;
using TnyFramework.Net.Transport;

namespace TnyFramework.Net.Nats.Transports
{

    public class NatsTransport : AttributesContext, IMessageTransporter, IAsyncDisposable
    {
        private static readonly ILogger LOGGER = LogFactory.Logger<NatsTransport>();

        private readonly NatsChannel channel;

        private volatile NatsTransportStatus status;

        private volatile TaskCompletionSource connectSource = new();

        private INatsAccessor? remoteAccessor;

        private readonly INetTunnel tunnel;

        public NatsTransport(NatsChannel channel, INatsAccessor remoteAccessor, string topic, NetAccessMode mode,
            INetworkContext context, INetService service)
        {
            status = NatsTransportStatus.Init;
            Topic = topic;
            Uri = new NatsUri(remoteAccessor);
            RemoteAddress = new NatsEndPoint(remoteAccessor);
            LocalAddress = new NatsEndPoint(channel.LocalNode, remoteAccessor.ChannelId);
            this.channel = channel;
            this.remoteAccessor = remoteAccessor;
            LocalAccessKey = NatsAccessor.OfKey(channel.LocalNode, remoteAccessor.ChannelId);
            tunnel = new NatsTunnel(ConnectIdFactory.NewTunnelId(), this, mode, context, service);
        }

        public INatsAccessor RemoteAccessor => remoteAccessor!;

        public string LocalAccessKey { get; }

        public string RemoteAccessKey => remoteAccessor?.AccessKey ?? "";

        internal INetTunnel NetTunnel => tunnel;

        public EndPoint? RemoteAddress { get; }

        public EndPoint? LocalAddress { get; }

        public NatsTransportStatus Status => status;

        public string Topic { get; }

        public bool Connecting => status == NatsTransportStatus.Connecting;

        public IMessageFactory MessageFactory => tunnel.MessageFactory;

        public NatsUri Uri { get; }

        public bool Receive(INetMessage message)
        {
            return tunnel.Receive(message);
        }

        public void Bind(INetTunnel tunnel)
        {
        }

        public ValueTask Write(IMessage message, bool waitWritten = false)
        {
            var command = message.Mode switch {
                MessageMode.Ping => NatsMessageCommand.Ping(this),
                MessageMode.Pong => NatsMessageCommand.Ping(this),
                _ => NatsMessageCommand.MessageCommand(this, message)
            };
            return channel.Write(command, waitWritten);
        }

        public ValueTask Write(IMessageAllocator maker, MessageContent content, bool waitWritten = false)
        {
            var command = NatsContentCommand.ContentCommand(this, maker.Allocate, content);
            return channel.Write(command, waitWritten);
        }

        public ValueTask Write(MessageAllocator allocator, MessageContent content, bool waitWritten = false)
        {
            var command = NatsContentCommand.ContentCommand(this, allocator, content);
            return channel.Write(command, waitWritten);
        }

        internal async ValueTask<bool> Connect()
        {
            if (!UpdateStatusIf(NatsTransportStatus.Connecting, NatsTransportStatus.Init))
            {
                return false;
            }
            await channel.Write(NatsMessageCommand.Connect(this), true).AsTask().WaitAsync(TimeSpan.FromMilliseconds(3000));
            await connectSource.Task;
            return true;
        }

        internal async ValueTask<bool> Accept()
        {
            if (!UpdateStatusIf(NatsTransportStatus.Connected, NatsTransportStatus.Init))
            {
                return false;
            }
            connectSource.TrySetResult();
            tunnel.Open();
            LOGGER.LogInformation("NatsChannel {channel} accept transport {key}", channel.LocalNode, remoteAccessor?.AccessKey);
            await channel.Write(NatsMessageCommand.Connected(this), true);
            return true;
        }

        internal ValueTask<bool> Connected()
        {
            var result = UpdateStatusIf(NatsTransportStatus.Connected, NatsTransportStatus.Connecting);
            if (!result)
            {
                return ValueTask.FromResult(result);
            }
            tunnel.Open();
            connectSource.TrySetResult();
            LOGGER.LogInformation("NatsChannel {channel} connected transport {key}", channel.LocalNode, remoteAccessor?.AccessKey);
            return ValueTask.FromResult(result);
        }

        bool IConnection.Close()
        {
            _ = Close();
            return true;
        }

        public bool IsActive()
        {
            return status == NatsTransportStatus.Connected;
        }

        public bool IsClosed()
        {
            return status == NatsTransportStatus.Close;
        }

        internal ValueTask<bool> Close()
        {
            return DoClose(() => channel.Write(NatsMessageCommand.Close(this)));
        }

        internal ValueTask<bool> Closed()
        {
            try
            {
                LOGGER.LogInformation("NatsChannel {channel} closed transport {key}", channel.LocalNode, remoteAccessor?.AccessKey);
                return DoClose();
            } finally
            {
                tunnel.Close();
            }
        }

        private async ValueTask<bool> DoClose(Func<ValueTask>? callback = null)
        {
            if (IsClosed())
            {
                return false;
            }
            lock (this)
            {
                if (IsClosed())
                {
                    return false;
                }
                status = NatsTransportStatus.Close;
                tunnel.Close();
                channel.TransportOnClose(this);
            }
            if (callback == null)
            {
                return true;
            }
            await callback();
            return true;
        }

        private bool UpdateStatusIf(NatsTransportStatus value, NatsTransportStatus expect)
        {
            if (status != expect)
            {
                return false;
            }
            lock (this)
            {
                if (status != expect)
                {
                    return false;
                }
                status = value;
                return true;
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (status == NatsTransportStatus.Disposed)
            {
                return;
            }
            if (status != NatsTransportStatus.Close)
            {
                await Close();
            }
            lock (this)
            {
                if (status == NatsTransportStatus.Disposed)
                {
                    return;
                }
                var currentRemote = remoteAccessor;
                remoteAccessor = null!;
                status = NatsTransportStatus.Disposed;
                if (currentRemote is IDisposable remote)
                {
                    remote.Dispose();
                }
                // var currentLocal = localAccessor;
                // localAccessor = null!;
                // if (currentLocal is IDisposable local)
                // {
                //     local.Dispose();
                // }
            }
        }
    }

}
