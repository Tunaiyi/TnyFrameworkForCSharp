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
using TnyFramework.Common.EventBus;
using TnyFramework.Common.Extensions;
using TnyFramework.Common.Logger;
using TnyFramework.Coroutines.Async;
using TnyFramework.Net.Command.Dispatcher;
using TnyFramework.Net.Command.Tasks;
using TnyFramework.Net.Common;
using TnyFramework.Net.Exceptions;
using TnyFramework.Net.Message;
using TnyFramework.Net.Session.Event;
using TnyFramework.Net.Transport;

namespace TnyFramework.Net.Session;

public abstract class NetSession : Communicator, INetSession
{
    private static readonly IEventBus<SessionOnline> ONLINE_GLOBAL_EVENT_BUS = EventBuses.Create<SessionOnline>();
    private static readonly IEventBus<SessionOffline> OFFLINE_GLOBAL_BUS = EventBuses.Create<SessionOffline>();
    private static readonly IEventBus<SessionClose> CLOSE_GLOBAL_BUS = EventBuses.Create<SessionClose>();

    /// <summary>
    /// 激活事件总线, 可监听到所有 Tunnel 的事件
    /// </summary>
    public static IEventWatch<SessionOnline> OnlineGlobalEvent => ONLINE_GLOBAL_EVENT_BUS;

    /// <summary>
    /// 断线事件总线, 可监听到所有 Tunnel 的事件
    /// </summary>
    public static IEventWatch<SessionOffline> OfflineGlobalBox => OFFLINE_GLOBAL_BUS;

    /// <summary>
    /// 关闭事件总线, 可监听到所有 Tunnel 的事件
    /// </summary>
    public static IEventWatch<SessionClose> CloseGlobalBox => CLOSE_GLOBAL_BUS;

    private static readonly ILogger LOGGER = LogFactory.Logger<NetSession>();

    private volatile INetTunnel? tunnel;

    private ICertificate certificate;

    private volatile TaskResponseSourceMonitor? sourceMonitor;

    private int status;

    private long idCreator;

    private readonly MessageAllocator allocator;

    private readonly IEventBus<SessionOnline> onlineEvent;

    private readonly IEventBus<SessionOffline> offlineEvent;

    private readonly IEventBus<SessionClose> closeEvent;

    public IEventWatch<SessionOnline> OnlineEvent => onlineEvent;

    public IEventWatch<SessionOffline> OfflineEvent => offlineEvent;

    public IEventWatch<SessionClose> CloseEvent => closeEvent;

    public override NetAccessMode AccessMode => tunnel?.AccessMode ?? default!;

    public NetSession(ICertificate certificate, ISessionContext context, INetTunnel tunnel)
    {
        Id = ConnectIdFactory.NewSessionId();
        this.certificate = certificate;
        Context = context;
        Status = SessionStatus.Init;
        allocator = (this as INetSession).CreateMessage;
        CommandBox = context.CommandBoxFactory.CreateCommandBox(this.certificate);
        onlineEvent = ONLINE_GLOBAL_EVENT_BUS.ForkChild();
        offlineEvent = OFFLINE_GLOBAL_BUS.ForkChild();
        closeEvent = CLOSE_GLOBAL_BUS.ForkChild();
        UpdateTunnel(tunnel);
    }

    public long Id { get; }

    public CommandBox CommandBox { get; }

    public ISessionContext Context { get; }

    public override EndPoint? RemoteAddress => CurrentTunnel.LocalAddress;

    public override EndPoint? LocalAddress => CurrentTunnel.RemoteAddress;

    public long OfflineTime { get; private set; }

    public override ICertificate Certificate => certificate;

    protected INetTunnel CurrentTunnel => tunnel!;

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

    private void PutSource(long messageId, TaskResponseSource? source)
    {
        var monitor = ResponseSourceMonitor;
        monitor.Put(messageId, source);
    }

    private TaskResponseSource? PollSource(IMessage message)
    {
        var monitor = sourceMonitor;
        if (monitor == null)
        {
            return null;
        }
        return message.Mode == MessageMode.Response ? monitor.Poll(message.ToMessage) : null;
    }

    public SessionStatus Status {
        get => (SessionStatus) status;

        private set => status = (int) value;
    }

    public MessageHandleFilter? SendFilter { get; set; }

    public MessageHandleFilter? ReceiveFilter { get; set; }

    public bool Receive(IRpcMessageEnterContext rpcMessageContext)
    {
        RpcRejectReceiveException cause;
        var result = MessageHandleStrategy.Handle;
        var message = rpcMessageContext.NetMessage;
        try
        {
            var filter = ReceiveFilter;
            var rcTunnel = rpcMessageContext.NetTunnel;
            if (filter != null)
            {
                result = filter(this, message);
            }
            if (result.IsHandleable())
            {
                return CommandBox.AddCommand(rpcMessageContext);
            }
            cause = new RpcRejectReceiveException(RejectMessage(true, filter, message, rcTunnel));
        } catch (Exception e)
        {
            LOGGER.LogError(e, "");
            rpcMessageContext.Complete(e);
            throw new NetException(NetResultCode.SERVER_ERROR, e);
        } finally
        {
            var source = PollSource(message);
            if (source != null)
            {
                CommandBox.AsyncExec(() => {
                    source.SetResult(message);
                    return source.Task as Task;
                });
            }
        }
        LOGGER.LogError(cause, "");
        rpcMessageContext.Complete(cause);
        if (result.IsThrowable())
        {
            throw cause;
        }
        return true;
    }

    private string RejectMessage(bool receive, MessageHandleFilter? filter, IMessageSubject message, IConnection handleTunnel)
    {
        if (filter == null)
            throw new ArgumentNullException(nameof(filter));
        var mode = receive ? "receive" : "send";
        return $"{this} cannot {mode} {message} from {handleTunnel} after being filtered by {nameof(filter)} {filter}";
    }

    protected void SetTunnel(INetTunnel value)
    {
        tunnel = value;
    }

    public void TakeOver(CommandBox commandBox)
    {
        CommandBox.TakeOver(commandBox);
    }

    public ValueTask<IMessageSent> Send(MessageContent content, bool waitWritten = false) => Send(null, content, waitWritten);

    public async ValueTask<IMessageSent> Send(INetTunnel? sendTunnel, MessageContent content, bool waitWritten = false)
    {
        RpcRejectSendException cause;
        var result = MessageHandleStrategy.Handle;
        IMessageWritable write = content;
        try
        {
            sendTunnel ??= CurrentTunnel;
            if (IsClosed())
            {
                throw new SessionClosedException($"session {this} closed");
            }
            var filter = SendFilter;
            if (filter != null)
            {
                result = filter(this, content);
            }
            if (result.IsHandleable())
            {
                var written = sendTunnel.Write(allocator, content);
                if (waitWritten)
                {
                    await written;
                }
                write.Written();
                return content;
            }
            cause = new RpcRejectSendException(RejectMessage(false, filter, content, sendTunnel));
        } catch (Exception e)
        {
            LOGGER.LogError(e, "");
            content.Cancel(e);
            throw new NetException(NetResultCode.SERVER_ERROR, e);
        }
        LOGGER.LogError(cause, "");
        content.Cancel(cause);
        if (result.IsThrowable())
        {
            throw cause;
        }
        return content;
    }

    private long AllocateMessageId()
    {
        return Interlocked.Increment(ref idCreator);
    }

    INetMessage INetSession.CreateMessage(IMessageFactory messageFactory, MessageContent content)
    {
        var message = messageFactory.Create(AllocateMessageId(), content);
        if (content is IMessageResponsable completable)
        {
            PutSource(message.Id, completable.ResponseSource);
        }
        return message;
    }

    private void SetOnline()
    {
        OfflineTime = 0;
        Status = SessionStatus.Online;
        onlineEvent.Notify(this);
    }

    protected void SetOffline()
    {
        OfflineTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        Status = SessionStatus.Offline;
        offlineEvent.Notify(this);
    }

    private void SetClose()
    {
        Status = SessionStatus.Close;
        DestroySourceMonitor();
        closeEvent.Notify(this);
    }

    private void DestroySourceMonitor()
    {
        TaskResponseSourceMonitor.RemoveMonitor(this);
    }

    public override bool IsActive()
    {
        var current = tunnel;
        return current != null && current.IsActive();
    }

    public override bool IsClosed() => Status == SessionStatus.Close;

    public bool IsOnline() => Status == SessionStatus.Online;

    public bool IsOffline() => Status == SessionStatus.Offline;

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

    public override bool Close()
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
            throw new AuthFailedException(NetResultCode.NO_LOGIN);
        }
        if (currentCert.IsNotNull() && currentCert.IsAuthenticated() && !currentCert.IsSameCertificate(newCertificate))
        {
            // 是否是同一个授权
            throw new AuthFailedException($"Certificate new [{newCertificate}] 与 old [{currentCert}] 不同");
        }
        if (IsClosed()) // 判断 session 状态是否可以重登
        {
            throw new AuthFailedException(NetResultCode.SESSION_LOSS_ERROR);
        }
    }

    public void Online(ICertificate newCertificate)
    {
        CheckOnlineCertificate(newCertificate);
        lock (this)
        {
            CheckOnlineCertificate(newCertificate);
            certificate = newCertificate;
            SetOnline();
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
            certificate = newCertificate;
            AcceptTunnel(onlineOne);
        }
    }

    private bool UpdateTunnel(INetTunnel tunnel)
    {
        if (!tunnel.Bind(this))
        {
            return false;
        }
        var oldTunnel = this.tunnel;
        this.tunnel = tunnel;
        OfflineTime = 0;
        if (oldTunnel != null && tunnel != oldTunnel)
        {
            oldTunnel.Close(); // 关闭旧 Tunnel
        }
        return true;
    }

    private void AcceptTunnel(INetTunnel newTunnel)
    {
        if (UpdateTunnel(newTunnel))
        {
            SetOnline();
        } else
        {
            OfflineIf(newTunnel);
            throw new AuthFailedException($"{newTunnel} tunnel is bound session");
        }
    }

    public virtual void OnUnactivated(INetTunnel one)
    {
    }

    public Task AsyncExec(AsyncHandle handle)
    {
        return CommandBox.AsyncExec(handle);
    }

    public Task<T> AsyncExec<T>(AsyncHandle<T> function)
    {
        return CommandBox.AsyncExec(function);
    }
}
