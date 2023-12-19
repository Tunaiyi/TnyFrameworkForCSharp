// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System.Threading;
using System.Threading.Tasks;
using TnyFramework.Common.Event;
using TnyFramework.Common.Extensions;
using TnyFramework.Net.Base;
using TnyFramework.Net.Command.Dispatcher;
using TnyFramework.Net.Endpoint;
using TnyFramework.Net.Message;
using TnyFramework.Net.Transport.Event;

namespace TnyFramework.Net.Transport
{

    public interface ITunnelEventSource
    {
        protected static readonly IEventBus<TunnelActivate> ACTIVATE_GLOBAL_EVENT = EventBuses.Create<TunnelActivate>();
        protected static readonly IEventBus<TunnelUnactivated> UNACTIVATED_GLOBAL_EVENT = EventBuses.Create<TunnelUnactivated>();
        protected static readonly IEventBus<TunnelClose> CLOSE_GLOBAL_EVENT = EventBuses.Create<TunnelClose>();

        /// <summary>
        /// 激活事件总线, 可监听到所有 Tunnel 的事件
        /// </summary>
        public static IEventBox<TunnelActivate> ActivateGlobalEvent => ACTIVATE_GLOBAL_EVENT;

        /// <summary>
        /// 断线事件总线, 可监听到所有 Tunnel 的事件
        /// </summary>
        public static IEventBox<TunnelUnactivated> UnactivatedGlobalEvent => UNACTIVATED_GLOBAL_EVENT;

        /// <summary>
        /// 关闭事件总线, 可监听到所有 Tunnel 的事件
        /// </summary>
        public static IEventBox<TunnelClose> CloseGlobalEvent => CLOSE_GLOBAL_EVENT;
    }

    public abstract class NetTunnel<TEndpoint> : Connector, ITunnelEventSource, INetTunnel
        where TEndpoint : INetEndpoint
    {
        private int status = TunnelStatus.Init.Value();

        private TEndpoint endpoint;

        private readonly ReaderWriterLockSlim endpointLock = new();

        public long Id { get; }

        public long AccessId { get; private set; }

        public TunnelStatus Status {
            get => (TunnelStatus) status;

            protected set => status = value.Value();
        }

        public override ICertificate Certificate => endpoint.IsNull() ? null! : endpoint.Certificate;

        public INetworkContext Context { get; }

        public IMessageFactory MessageFactory => Context.MessageFactory;

        // public abstract override EndPoint RemoteAddress { get; }
        //
        // public abstract override EndPoint LocalAddress { get; }

        public INetEndpoint Endpoint => endpoint;

        public override NetAccessMode AccessMode { get; }

        IEndpoint ITunnel.Endpoint => Endpoint;

        public bool IsOpen() => Status == TunnelStatus.Open;

        public override bool IsClosed() => Status == TunnelStatus.Closed;

        private readonly IEventBus<TunnelActivate> activateEvent;
        private readonly IEventBus<TunnelUnactivated> unactivatedEvent;
        private readonly IEventBus<TunnelClose> closeEvent;

        public IEventBox<TunnelActivate> ActivateEvent => activateEvent;

        public IEventBox<TunnelUnactivated> UnactivatedEvent => unactivatedEvent;

        public IEventBox<TunnelClose> CloseEvent => closeEvent;

        protected NetTunnel(long id, NetAccessMode accessMode, INetworkContext context)
        {
            Id = id;
            Context = context;
            endpoint = default!;
            AccessMode = accessMode;
            activateEvent = ITunnelEventSource.ACTIVATE_GLOBAL_EVENT.ForkChild();
            unactivatedEvent = ITunnelEventSource.UNACTIVATED_GLOBAL_EVENT.ForkChild();
            closeEvent = ITunnelEventSource.CLOSE_GLOBAL_EVENT.ForkChild();
        }

        public IEndpoint GetEndpoint()
        {
            return endpoint;
        }

        public INetEndpoint NetEndpoint => endpoint;

        protected void SetEndpoint(TEndpoint value)
        {
            endpoint = value;
        }

        public bool Receive(INetMessage message)
        {
            endpointLock.EnterReadLock();
            try
            {
                var rpcContext = RpcMessageTransactionContext.CreateEnter(this, message);
                var rpcMonitor = Context.RpcMonitor;
                rpcMonitor.OnReceive(rpcContext);
                return endpoint.Receive(rpcContext);
            } finally
            {
                endpointLock.ExitReadLock();
            }
        }

        public ValueTask<IMessageSent> Send(MessageContent content, bool waitWritten = false)
        {
            endpointLock.EnterReadLock();
            try
            {
                return endpoint.Send(this, content, waitWritten);
            } finally
            {
                endpointLock.ExitReadLock();
            }
        }

        public void SetAccessId(long accessId)
        {
            if (AccessId == 0)
            {
                AccessId = accessId;
            }
        }

        public bool Bind(INetEndpoint newEndpoint)
        {
            if (newEndpoint.IsNull())
            {
                return false;
            }
            if (Endpoint == newEndpoint)
            {
                return true;
            }
            endpointLock.EnterWriteLock();
            try
            {
                if (Endpoint == newEndpoint)
                {
                    return true;
                }
                if (endpoint.IsNull())
                {
                    endpoint = (TEndpoint) newEndpoint;
                    return true;
                } else
                {
                    return ReplaceEndpoint(newEndpoint);
                }
            } finally
            {
                endpointLock.ExitWriteLock();
            }
        }

        protected abstract bool ReplaceEndpoint(INetEndpoint newEndpoint);

        public bool Open()
        {
            if (IsClosed() || IsActive())
            {
                return false;
            }
            lock (this)
            {
                if (IsClosed() || IsActive())
                {
                    return false;
                }
                if (!OnOpen())
                {
                    return false;
                }
                status = TunnelStatus.Open.Value();
                activateEvent.Notify(this);
                OnOpened();
            }
            return true;
        }

        protected virtual void OnOpened()
        {
        }

        protected virtual bool OnOpen()
        {
            return true;
        }

        public void Disconnect()
        {
            INetEndpoint? netEndpoint;
            lock (this)
            {
                var current = Status;
                if (current == TunnelStatus.Closed || current == TunnelStatus.Suspend)
                {
                    return;
                }
                Status = TunnelStatus.Suspend;
                OnDisconnect();
                netEndpoint = endpoint;
                OnDisconnected();
            }
            netEndpoint.OnUnactivated(this);
            unactivatedEvent.Notify(this);
        }

        protected abstract void OnDisconnect();

        protected virtual void OnDisconnected()
        {
        }

        public override bool Close()
        {
            var current = Status;
            if (current == TunnelStatus.Closed)
            {
                return false;
            }
            INetEndpoint? netEndpoint;
            lock (this)
            {
                current = Status;
                if (current == TunnelStatus.Closed)
                {
                    return false;
                }
                Status = TunnelStatus.Closed;
                OnDisconnect();
                OnClose();
                netEndpoint = endpoint;
                OnClosed();
            }
            netEndpoint.OnUnactivated(this);
            closeEvent.Notify(this);
            return true;
        }

        protected virtual void OnClosed()
        {
        }

        protected virtual void OnClose()
        {
        }

        public abstract void Reset();

        public void Pong()
        {
            Write(TickMessage.Pong());
        }

        public void Ping()
        {
            Write(TickMessage.Ping());
        }

        public abstract ValueTask Write(IMessage message, bool waitWritten = false);

        public abstract ValueTask Write(MessageAllocator allocator, MessageContent messageContent, bool waitWritten = false);
    }

}
