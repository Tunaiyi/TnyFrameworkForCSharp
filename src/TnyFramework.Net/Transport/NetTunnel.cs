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
using TnyFramework.Common.EventBus;
using TnyFramework.Common.Extensions;
using TnyFramework.Net.Application;
using TnyFramework.Net.Command.Dispatcher;
using TnyFramework.Net.Message;
using TnyFramework.Net.Session;
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
        public static IEventWatch<TunnelActivate> ActivateGlobalEvent => ACTIVATE_GLOBAL_EVENT;

        /// <summary>
        /// 断线事件总线, 可监听到所有 Tunnel 的事件
        /// </summary>
        public static IEventWatch<TunnelUnactivated> UnactivatedGlobalEvent => UNACTIVATED_GLOBAL_EVENT;

        /// <summary>
        /// 关闭事件总线, 可监听到所有 Tunnel 的事件
        /// </summary>
        public static IEventWatch<TunnelClose> CloseGlobalEvent => CLOSE_GLOBAL_EVENT;
    }

    public abstract class NetTunnel<TSession> : Communicator, ITunnelEventSource, INetTunnel
        where TSession : INetSession
    {
        private int status = TunnelStatus.Init.Value();

        private TSession session;

        private readonly INetService service;

        private readonly ReaderWriterLockSlim sessionLock = new();

        public long Id { get; }

        public long AccessId { get; private set; }

        public TunnelStatus Status {
            get => (TunnelStatus) status;

            protected set => status = value.Value();
        }

        public string Service => service.Service;

        public override ICertificate Certificate => session.IsNull() ? null! : session.Certificate;

        public INetworkContext Context { get; }

        public IMessageFactory MessageFactory => Context.MessageFactory;

        public INetSession Session => session;

        public override NetAccessMode AccessMode { get; }

        ISession ITunnel.Session => Session;

        public bool IsOpen() => Status == TunnelStatus.Open;

        public override bool IsClosed() => Status == TunnelStatus.Closed;

        private readonly IEventBus<TunnelActivate> activateEvent;
        private readonly IEventBus<TunnelUnactivated> unactivatedEvent;
        private readonly IEventBus<TunnelClose> closeEvent;

        public IEventWatch<TunnelActivate> ActivateEvent => activateEvent;

        public IEventWatch<TunnelUnactivated> UnactivatedEvent => unactivatedEvent;

        public IEventWatch<TunnelClose> CloseEvent => closeEvent;

        protected NetTunnel(long id, NetAccessMode accessMode, INetworkContext context, INetService service)
        {
            this.service = service;
            Id = id;
            Context = context;
            session = default!;
            AccessMode = accessMode;
            activateEvent = ITunnelEventSource.ACTIVATE_GLOBAL_EVENT.ForkChild();
            unactivatedEvent = ITunnelEventSource.UNACTIVATED_GLOBAL_EVENT.ForkChild();
            closeEvent = ITunnelEventSource.CLOSE_GLOBAL_EVENT.ForkChild();
            // EventHandle<ITunnel, int> handle = (_, _) => { };
            // Action<ITunnel> action = new Action<ITunnel>(handle);
        }

        public ISession GetSession()
        {
            return session;
        }

        public INetSession NetSession => session;

        protected void SetSession(TSession value)
        {
            session = value;
        }

        public bool Receive(INetMessage message)
        {
            sessionLock.EnterReadLock();
            try
            {
                var rpcContext = RpcMessageTransactionContext.CreateEnter(this, message);
                var rpcMonitor = Context.RpcMonitor;
                rpcMonitor.OnReceive(rpcContext);
                return session.Receive(rpcContext);
            } finally
            {
                sessionLock.ExitReadLock();
            }
        }

        public ValueTask<IMessageSent> Send(MessageContent content, bool waitWritten = false)
        {
            sessionLock.EnterReadLock();
            try
            {
                return session.Send(this, content, waitWritten);
            } finally
            {
                sessionLock.ExitReadLock();
            }
        }

        public void SetAccessId(long accessId)
        {
            if (AccessId == 0)
            {
                AccessId = accessId;
            }
        }

        public bool Bind(INetSession newSession)
        {
            if (newSession.IsNull())
            {
                return false;
            }
            if (Session == newSession)
            {
                return true;
            }
            sessionLock.EnterWriteLock();
            try
            {
                if (Session == newSession)
                {
                    return true;
                }
                if (session.IsNull())
                {
                    session = (TSession) newSession;
                    return true;
                } else
                {
                    return ResetSession(newSession);
                }
            } finally
            {
                sessionLock.ExitWriteLock();
            }
        }

        protected abstract bool ResetSession(INetSession newSession);

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
            INetSession? netSession;
            lock (this)
            {
                var current = Status;
                if (current == TunnelStatus.Closed || current == TunnelStatus.Suspend)
                {
                    return;
                }
                Status = TunnelStatus.Suspend;
                OnDisconnect();
                netSession = session;
                OnDisconnected();
            }
            netSession.OnUnactivated(this);
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
            INetSession? netSession;
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
                netSession = session;
                OnClosed();
            }
            netSession.OnUnactivated(this);
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
