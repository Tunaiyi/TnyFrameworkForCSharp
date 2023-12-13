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
using TnyFramework.Net.Command;
using TnyFramework.Net.Command.Dispatcher;
using TnyFramework.Net.Endpoint;
using TnyFramework.Net.Message;
using TnyFramework.Net.Transport.Event;

namespace TnyFramework.Net.Transport
{

    public static class NetTunnel
    {
        internal static readonly IEventBus<TunnelActivate> ACTIVATE_EVENT_BUS = EventBuses.Create<TunnelActivate>();
        internal static readonly IEventBus<TunnelUnactivated> UNACTIVATED_EVENT_BUS = EventBuses.Create<TunnelUnactivated>();
        internal static readonly IEventBus<TunnelClose> CLOSE_EVENT_BUS = EventBuses.Create<TunnelClose>();

        /// <summary>
        /// 激活事件总线, 可监听到所有 Tunnel 的事件
        /// </summary>
        public static IEventBox<TunnelActivate> ActivateEventBox => ACTIVATE_EVENT_BUS;

        /// <summary>
        /// 断线事件总线, 可监听到所有 Tunnel 的事件
        /// </summary>
        public static IEventBox<TunnelUnactivated> UnactivatedEventBox => UNACTIVATED_EVENT_BUS;

        /// <summary>
        /// 关闭事件总线, 可监听到所有 Tunnel 的事件
        /// </summary>
        public static IEventBox<TunnelClose> CloseEventBox => CLOSE_EVENT_BUS;
    }

    public abstract class NetTunnel<TUserId, TEndpoint> : Communicator<TUserId>, INetTunnel<TUserId>
        where TEndpoint : INetEndpoint<TUserId>
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

        public override ICertificate<TUserId> Certificate => endpoint.IsNull() ? null! : endpoint.Certificate;

        public INetworkContext Context { get; }

        public IMessageFactory MessageFactory => Context.MessageFactory;

        public ICertificateFactory CertificateFactory => Context.GetCertificateFactory();

        ICertificateFactory<TUserId> INetTunnel<TUserId>.CertificateFactory => Context.CertificateFactory<TUserId>();

        // public abstract override EndPoint RemoteAddress { get; }
        //
        // public abstract override EndPoint LocalAddress { get; }

        public INetEndpoint<TUserId> Endpoint => endpoint;

        public override NetAccessMode AccessMode { get; }

        IEndpoint<TUserId> ITunnel<TUserId>.Endpoint => Endpoint;

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
            activateEvent = NetTunnel.ACTIVATE_EVENT_BUS.ForkChild();
            unactivatedEvent = NetTunnel.UNACTIVATED_EVENT_BUS.ForkChild();
            closeEvent = NetTunnel.CLOSE_EVENT_BUS.ForkChild();
        }

        public IEndpoint GetEndpoint()
        {
            return endpoint;
        }

        INetEndpoint INetTunnel.GetEndpoint()
        {
            return endpoint;
        }

        protected void SetEndpoint(TEndpoint value)
        {
            endpoint = value;
        }

        public bool Receive(INetMessage message)
        {
            endpointLock.EnterReadLock();
            try
            {
                var rpcContext = RpcTransactionContext.CreateEnter(this, message);
                var rpcMonitor = Context.RpcMonitor;
                rpcMonitor.OnReceive(rpcContext);
                return endpoint.Receive(rpcContext);
            } finally
            {
                endpointLock.ExitReadLock();
            }
        }

        public ISendReceipt Send(MessageContent content)
        {
            endpointLock.EnterReadLock();
            try
            {
                return endpoint.Send(this, content);
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
            TEndpoint? netEndpoint;
            lock (this)
            {
                var current = Status;
                if (current == TunnelStatus.Closed || current == TunnelStatus.Suspend)
                {
                    return;
                }
                DoDisconnect();
                Status = TunnelStatus.Suspend;
                netEndpoint = endpoint;
                OnDisconnected();
            }
            netEndpoint.OnUnactivated(this);
            unactivatedEvent.Notify(this);
        }

        protected virtual void OnDisconnected()
        {
        }

        protected abstract void DoDisconnect();

        protected virtual void OnDisconnect()
        {
        }

        public override bool Close()
        {
            var current = Status;
            if (current == TunnelStatus.Closed)
            {
                return false;
            }
            TEndpoint? netEndpoint;
            lock (this)
            {
                current = Status;
                if (current == TunnelStatus.Closed)
                {
                    return false;
                }
                Status = TunnelStatus.Closed;
                OnClose();
                DoDisconnect();
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

        public abstract Task Write(IMessage message);

        public abstract Task Write(MessageAllocator allocator, MessageContent messageContent);

        public Task Write(IMessageAllocator allocator, MessageContent messageContent)
        {
            return Write(allocator.Allocate, messageContent);
        }
    }

}
