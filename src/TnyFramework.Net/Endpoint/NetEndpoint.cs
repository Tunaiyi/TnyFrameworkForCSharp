// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TnyFramework.Common.Event;
using TnyFramework.Common.Logger;
using TnyFramework.Coroutines.Async;
using TnyFramework.Net.Command;
using TnyFramework.Net.Command.Tasks;
using TnyFramework.Net.Common;
using TnyFramework.Net.Endpoint.Event;
using TnyFramework.Net.Exceptions;
using TnyFramework.Net.Message;
using TnyFramework.Net.Transport;

namespace TnyFramework.Net.Endpoint
{

    public static class NetEndpoint
    {
        internal static readonly IEventBus<EndpointOnline> ONLINE_EVENT_BUS = EventBuses.Create<EndpointOnline>();
        internal static readonly IEventBus<EndpointOffline> OFFLINE_EVENT_BUS = EventBuses.Create<EndpointOffline>();
        internal static readonly IEventBus<EndpointClose> CLOSE_EVENT_BUS = EventBuses.Create<EndpointClose>();

        /// <summary>
        /// 激活事件总线, 可监听到所有 Tunnel 的事件
        /// </summary>
        public static IEventBox<EndpointOnline> OnlineEventBox => ONLINE_EVENT_BUS;

        /// <summary>
        /// 断线事件总线, 可监听到所有 Tunnel 的事件
        /// </summary>
        public static IEventBox<EndpointOffline> OfflineEventBox => OFFLINE_EVENT_BUS;

        /// <summary>
        /// 关闭事件总线, 可监听到所有 Tunnel 的事件
        /// </summary>
        public static IEventBox<EndpointClose> CloseEventBox => CLOSE_EVENT_BUS;
    }

    public abstract class NetEndpoint<TUserId> : Communicator<TUserId>, INetEndpoint<TUserId>
    {
        private static readonly ILogger LOGGER = LogFactory.Logger<NetEndpoint<TUserId>>();

        private volatile INetTunnel<TUserId> tunnel;

        private ICertificate<TUserId> certificate;

        private volatile TaskResponseSourceMonitor sourceMonitor;

        private int status;

        private long idCreator;

        private readonly IEventBus<EndpointOnline> onlineEvent;

        private readonly IEventBus<EndpointOffline> offlineEvent;

        private readonly IEventBus<EndpointClose> closeEvent;

        public IEventBox<EndpointOnline> OnlineEvent => onlineEvent;

        public IEventBox<EndpointOffline> OfflineEvent => offlineEvent;

        public IEventBox<EndpointClose> CloseEvent => closeEvent;

        public NetEndpoint(ICertificate<TUserId> certificate, IEndpointContext context)
        {
            Id = TransporterIdFactory.NewEndpointId();
            this.certificate = certificate;
            Context = context;
            Status = EndpointStatus.Init;
            CommandTaskBox = new CommandTaskBox(context.CommandTaskProcessor);
            onlineEvent = NetEndpoint.ONLINE_EVENT_BUS.ForkChild();
            offlineEvent = NetEndpoint.OFFLINE_EVENT_BUS.ForkChild();
            closeEvent = NetEndpoint.CLOSE_EVENT_BUS.ForkChild();
        }

        public long Id { get; }

        public CommandTaskBox CommandTaskBox { get; }

        public IEndpointContext Context { get; }

        public EndPoint RemoteAddress => CurrentTunnel?.LocalAddress;

        public EndPoint LocalAddress => CurrentTunnel?.RemoteAddress;

        public long OfflineTime { get; private set; }

        public override ICertificate<TUserId> Certificate => certificate;

        protected INetTunnel<TUserId> CurrentTunnel => tunnel;

        private TaskResponseSourceMonitor ResponseSourceMonitor {
            get {
                if (sourceMonitor != null)
                    return sourceMonitor;
                lock (this)
                {
                    if (sourceMonitor != null)
                        return sourceMonitor;
                    return sourceMonitor = TaskResponseSourceMonitor.LoadMonitor(this);
                }
            }
        }

        private void PutSource(long messageId, TaskResponseSource source)
        {
            var monitor = ResponseSourceMonitor;
            monitor?.Put(messageId, source);
        }

        private TaskResponseSource PollSource(IMessageSchema message)
        {
            var monitor = sourceMonitor;
            if (monitor == null)
            {
                return null;
            }
            return message.Mode == MessageMode.Response ? monitor.Poll(message.ToMessage) : null;
        }

        public EndpointStatus Status {
            get => (EndpointStatus) status;

            private set => status = (int) value;
        }

        public MessageHandleFilter SendFilter { get; set; }

        public MessageHandleFilter ReceiveFilter { get; set; }

        public bool Receive(IMessage message) => Receive(null, message);

        public bool Receive(INetTunnel receiver, IMessage message)
        {

            var filter = ReceiveFilter;
            if (filter != null)
            {
                switch (filter(this, message))
                {
                    case MessageHandleStrategy.Ignore:
                        return true;
                    case MessageHandleStrategy.Throw:
                        var causeMessage = $"{this} cannot receive {message} from {receiver} after being filtered by {filter}";
                        throw new EndpointException(causeMessage);
                }
            }
            var source = PollSource(message);
            if (source != null)
            {
                CommandTaskBox.AddTask(new RespondCommandTask(message, source));
            }
            return CommandTaskBox.AddTask(new MessageCommandTask(message, receiver, Context.MessageDispatcher));
        }

        protected void SetTunnel(INetTunnel value)
        {
            if (value is INetTunnel<TUserId> newOne)
                tunnel = newOne;
        }

        public void TakeOver(CommandTaskBox commandTaskBox)
        {
            CommandTaskBox.TakeOver(commandTaskBox);
        }

        public ISendReceipt Send(MessageContext messageContext) => Send(null, messageContext);

        public ISendReceipt Send(INetTunnel sender, MessageContext messageContext)
        {
            try
            {
                if (IsClosed())
                {
                    messageContext.Cancel(new EndpointCloseException($"endpoint {this} closed"));
                    return messageContext;
                }
                if (sender == null)
                {
                    sender = CurrentTunnel;
                }
                var filter = SendFilter;
                if (filter != null)
                {
                    var throwable = true;
                    switch (filter(this, messageContext))
                    {
                        case MessageHandleStrategy.Ignore:
                            messageContext.Cancel(false);
                            return messageContext;
                        case MessageHandleStrategy.Throw:
                            messageContext.Cancel(false);
                            var causeMessage = $"{this} cannot send {messageContext} to {sender} after being filtered by {filter}";
                            if (throwable)
                            {
                                throw new EndpointException(causeMessage);
                            }
                            return messageContext;
                        case MessageHandleStrategy.Handle:
                            break;
                    }
                }
                sender.Write(BuildMessage, messageContext);
                return messageContext;
            } catch (Exception e)
            {
                LOGGER.LogError(e, "");
                messageContext.Cancel(e);
                throw new NetException(e, "");
            }
        }

        private long AllocateMessageId()
        {
            return Interlocked.Increment(ref idCreator);
        }

        public INetMessage BuildMessage(IMessageFactory messageFactory, MessageContext context)
        {
            var message = messageFactory.Create(AllocateMessageId(), context);
            if (context is DefaultMessageContext requestContext && requestContext.IsRespondAwaitable())
            {
                PutSource(message.Id, requestContext.ResponseSource);
            }
            // TODO 加入已发送队列 this.sentMessageQueue.addMessage(message);
            return message;
        }

        private void SetOnline()
        {
            OfflineTime = 0;
            Status = EndpointStatus.Online;
            onlineEvent?.Notify(this);
        }

        protected void SetOffline()
        {
            OfflineTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            Status = EndpointStatus.Offline;
            offlineEvent?.Notify(this);
        }

        private void SetClose()
        {
            Status = EndpointStatus.Close;
            DestroySourceMonitor();
            closeEvent?.Notify(this);
        }

        private void DestroySourceMonitor()
        {
            TaskResponseSourceMonitor.RemoveMonitor(this);
        }

        public bool IsActive()
        {
            var current = CurrentTunnel;
            return current != null && current.IsActive();
        }

        public bool IsOnline() => Status == EndpointStatus.Online;

        public bool IsOffline() => Status == EndpointStatus.Offline;

        public bool IsClosed() => Status == EndpointStatus.Close;

        public void Offline()
        {
            lock (this)
            {
                var current = CurrentTunnel;
                if (!current.IsClosed())
                {
                    current.Close();
                }
                SetOffline();
            }
        }

        private void OfflineIf(INetTunnel check)
        {
            lock (this)
            {
                var current = CurrentTunnel;
                if (check != current)
                {
                    return;
                }
                if (!current.IsClosed())
                {
                    current.Close();
                }
                SetOffline();
            }
        }

        public void Heartbeat()
        {
            var current = CurrentTunnel;
            if (current.IsOpen())
            {
                current.Ping();
            }
        }

        public bool Close()
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
                Offline();
                PrepareClose();
                SetClose();
                PostClose();
                return true;
            }
        }

        protected virtual void PostClose()
        {
        }

        protected virtual void PrepareClose()
        {
        }

        private void CheckOnlineCertificate(ICertificate newCertificate)
        {
            var currentCert = Certificate;
            if (!newCertificate.IsAuthenticated())
            {
                throw new ValidatorFailException(NetResultCode.NO_LOGIN);
            }
            if (currentCert != null && currentCert.IsAuthenticated() && !currentCert.IsSameCertificate(newCertificate))
            {
                // 是否是同一个授权
                throw new ValidatorFailException(NetResultCode.VALIDATOR_FAIL_ERROR, $"Certificate new [{newCertificate}] 与 old [{currentCert}] 不同");
            }
            if (IsClosed()) // 判断 session 状态是否可以重登
            {
                throw new ValidatorFailException(NetResultCode.SESSION_LOSS_ERROR);
            }
        }

        public void Online(ICertificate newCertificate, INetTunnel onlineOne)
        {
            if (onlineOne == null)
                throw new NullReferenceException();
            CheckOnlineCertificate(newCertificate);
            lock (this)
            {
                CheckOnlineCertificate(newCertificate);
                certificate = (ICertificate<TUserId>) newCertificate;
                AcceptTunnel(onlineOne);
            }
        }

        private void AcceptTunnel(INetTunnel newTunnel)
        {
            if (newTunnel.Bind(this))
            {
                var oldTunnel = CurrentTunnel;
                tunnel = (INetTunnel<TUserId>) newTunnel;
                OfflineTime = 0;
                if (oldTunnel != null && newTunnel != oldTunnel)
                {
                    oldTunnel.Close(); // 关闭旧 Tunnel
                }
                SetOnline();
            } else
            {
                OfflineIf(newTunnel);
                throw new ValidatorFailException(NetResultCode.VALIDATOR_FAIL_ERROR, $"{newTunnel} tunnel is bound session");
            }
        }

        public virtual void OnUnactivated(INetTunnel one)
        {
        }

        public Task AsyncExec(AsyncHandle handle)
        {
            return CommandTaskBox.AsyncExec(handle);
        }

        public Task<T> AsyncExec<T>(AsyncHandle<T> function)
        {
            return CommandTaskBox.AsyncExec(function);
        }
    }

}
