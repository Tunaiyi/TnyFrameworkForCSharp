using System.Net;
using System.Threading;
using System.Threading.Tasks;
using TnyFramework.Common.Event;
using TnyFramework.Net.Base;
using TnyFramework.Net.Command;
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

        private readonly ReaderWriterLockSlim endpointLock = new ReaderWriterLockSlim();

        public long Id { get; }

        public long AccessId { get; private set; }

        public TunnelMode Mode { get; }

        public TunnelStatus Status {
            get => (TunnelStatus) status;

            protected set => status = value.Value();
        }

        public override ICertificate<TUserId> Certificate => Endpoint?.Certificate;

        public INetworkContext Context { get; }

        public IMessageFactory MessageFactory => Context.MessageFactory;

        public ICertificateFactory CertificateFactory => Context.GetCertificateFactory();

        ICertificateFactory<TUserId> INetTunnel<TUserId>.CertificateFactory => Context.CertificateFactory<TUserId>();

        public abstract EndPoint RemoteAddress { get; }

        public abstract EndPoint LocalAddress { get; }

        public INetEndpoint<TUserId> Endpoint => endpoint;

        IEndpoint<TUserId> ITunnel<TUserId>.Endpoint => Endpoint;

        public abstract bool IsActive();

        public bool IsOpen() => Status == TunnelStatus.Open;

        public bool IsClosed() => Status == TunnelStatus.Closed;

        private readonly IEventBus<TunnelActivate> activateEvent;
        private readonly IEventBus<TunnelUnactivated> unactivatedEvent;
        private readonly IEventBus<TunnelClose> closeEvent;

        public IEventBox<TunnelActivate> ActivateEvent => activateEvent;

        public IEventBox<TunnelUnactivated> UnactivatedEvent => unactivatedEvent;

        public IEventBox<TunnelClose> CloseEvent => closeEvent;

        protected NetTunnel(long id, TunnelMode mode, INetworkContext context)
        {
            Id = id;
            Mode = mode;
            Context = context;
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

        public bool Receive(IMessage message)
        {
            endpointLock.EnterReadLock();
            try
            {
                return endpoint.Receive(this, message);
            } finally
            {
                endpointLock.ExitReadLock();
            }
        }

        public ISendReceipt Send(MessageContext messageContext)
        {
            endpointLock.EnterReadLock();
            try
            {
                return endpoint.Send(this, messageContext);
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
            if (newEndpoint == null)
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
                if (endpoint == null)
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
            if (IsClosed())
            {
                return false;
            }
            if (IsActive())
            {
                return true;
            }
            lock (this)
            {
                if (IsClosed())
                {
                    return false;
                }
                if (IsActive())
                {
                    return true;
                }
                if (!OnOpen())
                {
                    return false;
                }
                status = TunnelStatus.Open.Value();
                activateEvent?.Notify(this);
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
            lock (this)
            {
                var current = Status;
                if (current == TunnelStatus.Closed || current == TunnelStatus.Suspend)
                {
                    return;
                }
                OnDisconnect();
                DoDisconnect();
                Status = TunnelStatus.Suspend;
                var netEndpoint = endpoint;
                unactivatedEvent?.Notify(this);
                netEndpoint?.OnUnactivated(this);
                OnDisconnected();
            }

        }

        protected virtual void OnDisconnected()
        {
        }

        protected abstract void DoDisconnect();

        protected virtual void OnDisconnect()
        {
        }

        public bool Close()
        {
            var current = Status;
            if (current == TunnelStatus.Closed)
            {
                return false;
            }
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
                var netEndpoint = endpoint;
                netEndpoint?.OnUnactivated(this);
                closeEvent?.Notify(this);
                OnClosed();
            }
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

        public abstract Task Write(MessageAllocator allocator, MessageContext messageContext);

        public Task Write(IMessageAllocator allocator, MessageContext messageContext)
        {
            return Write(allocator.Allocate, messageContext);
        }
    }

}
