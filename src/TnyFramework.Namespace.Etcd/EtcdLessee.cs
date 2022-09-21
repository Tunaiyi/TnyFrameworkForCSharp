// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Etcdserverpb;
using Microsoft.Extensions.Logging;
using TnyFramework.Common.Event;
using TnyFramework.Common.Logger;
using TnyFramework.Coroutines.Async;
using TnyFramework.Namespace.Exceptions;
using TnyFramework.Namespace.Listener;

namespace TnyFramework.Namespace.Etcd
{

    public class EtcdLessee : ILessee
    {
        private static readonly ILogger LOGGER = LogFactory.Logger<EtcdLessee>();

        private const int STOP = 0;

        private const int GRANT = 1;

        private const int LIVE = 2;

        private const int PAUSE = 3;

        private const int SHUTDOWN = 4;

        private volatile int status = STOP;

        private volatile Stopwatch stopwatch = new Stopwatch();

        private readonly EtcdAccessor client;

        private CancellationTokenSource keepAliveSource;

        private readonly ICoroutine coroutine;

        private readonly ICoroutine keepAliveCoroutine;

        private readonly IEventBus<LesseeOnRenew> renewEvent = EventBuses.Create<LesseeOnRenew>();

        private readonly IEventBus<LesseeOnCompleted> completedEvent = EventBuses.Create<LesseeOnCompleted>();

        private readonly IEventBus<LesseeOnLease> leaseEvent = EventBuses.Create<LesseeOnLease>();

        private readonly IEventBus<LesseeOnResume> resumeEvent = EventBuses.Create<LesseeOnResume>();

        private readonly IEventBus<LesseeOnError> errorEvent = EventBuses.Create<LesseeOnError>();

        public EtcdLessee(string name, EtcdAccessor client, long ttl)
        {
            this.client = client;
            Ttl = ttl;
            Name = name;
            coroutine = DefaultCoroutineFactory.Default.Create("EtcdLessee");
            keepAliveCoroutine = DefaultCoroutineFactory.Default.Create("EtcdLesseeKeepAlive");
        }

        public string Name { get; }

        public long Id { get; private set; }

        public long Ttl { get; private set; }

        public IEventBox<LesseeOnRenew> RenewEvent => renewEvent;

        public IEventBox<LesseeOnCompleted> CompletedEvent => completedEvent;

        public IEventBox<LesseeOnError> ErrorEvent => errorEvent;

        public IEventBox<LesseeOnLease> LeaseEvent => leaseEvent;

        public IEventBox<LesseeOnResume> ResumeEvent => resumeEvent;

        public int nextKeepAlive;

        public bool IsLive()
        {
            return status == LIVE;
        }

        public bool IsPause()
        {
            return status == PAUSE;
        }

        public bool IsStop()
        {
            return status == STOP;
        }

        public bool IsGranting()
        {
            return status == GRANT;
        }

        public bool IsShutdown()
        {
            return status == SHUTDOWN;
        }

        public Task<bool> Lease()
        {
            return Lease(Ttl);
        }

        public Task<bool> Lease(long ttl)
        {
            return DoGrant(ttl, STOP);
        }

        internal Task<bool> Resume()
        {
            return DoGrant(Ttl, PAUSE);
        }

        private Task<bool> DoGrant(long ttl, int whenStatus)
        {
            return coroutine.AsyncExec(async () => {
                try
                {
                    if (IsLive())
                    {
                        return true;
                    }
                    var current = status;
                    if (current != Interlocked.CompareExchange(ref status, GRANT, whenStatus))
                        return false;
                    var response = await client.LeaseGrantAsync(new LeaseGrantRequest {
                        TTL = Math.Max(ttl / 1000L, 1L)
                    });
                    Ttl = ttl;
                    Id = response.ID;
                    NextKeepAlive(response.TTL);
                    var tokenSource = new CancellationTokenSource();
                    var token = tokenSource.Token;
                    status = LIVE;
                    StartKeepAlive(Id, token);
                    keepAliveSource = tokenSource;
                    if (whenStatus == PAUSE)
                    {
                        resumeEvent.Notify(this);
                    } else
                    {
                        leaseEvent.Notify(this);
                    }
                    token.Register(() => coroutine.ExecAction(HandleCompleted));
                    return true;
                } catch (Exception e)
                {
                    HandleGrantError(whenStatus, e);
                    throw;
                }
            });
        }

        private void StartKeepAlive(long id, CancellationToken token)
        {
            keepAliveCoroutine.AsyncExec(async () => {
                while (Id == id && IsLive() && !token.IsCancellationRequested)
                {
                    var keepRequest = new LeaseKeepAliveRequest {
                        ID = Id
                    };
                    await client.LeaseKeepAlive(keepRequest, OnKeepAlive, token);
                    var useTime = stopwatch.ElapsedMilliseconds;
                    var delay = nextKeepAlive - useTime;
                    delay = Math.Max(delay, 1);
                    await Task.Delay((int) delay, token);
                    stopwatch.Restart();
                }
            });
        }

        private void HandleGrantError(int whenStatus, Exception e)
        {
            status = whenStatus;
            LOGGER.LogError(e, "DoGrant exception");
            errorEvent.Notify(this, e);
            throw e;
        }

        private void HandleCompleted()
        {
            if (status == LIVE)
            {
                status = STOP;
                var _ = DoRevoke();
            }
            completedEvent.Notify(this);
        }

        public Task<bool> Revoke()
        {
            return coroutine.AsyncExec(async () => {
                if (IsStop())
                {
                    return true;
                }
                var current = status;
                if (Interlocked.CompareExchange(ref status, STOP, LIVE) != current)
                    return false;
                await DoRevoke();
                return true;
            });

        }

        public Task Shutdown()
        {
            return coroutine.AsyncExec(async () => {
                var current = status;
                if (current == SHUTDOWN)
                {
                    return;
                }
                status = SHUTDOWN;
                await DoRevoke();
            });

        }

        private async Task DoRevoke()
        {
            keepAliveSource?.Cancel();
            await client.LeaseRevokeAsync(new LeaseRevokeRequest {ID = Id}, cancellationToken: default);
        }

        private void NextKeepAlive(long ttl)
        {
            nextKeepAlive = (int) (ttl * 1000 / 3);
            if (stopwatch.IsRunning)
            {
                stopwatch.Stop();
            }
        }

        private void OnKeepAlive(LeaseKeepAliveResponse response)
        {
            var ttl = response.TTL;
            if (ttl > 0)
            {
                NextKeepAlive(ttl);
            }
            coroutine.ExecAction(() => {
                var id = response.ID;
                if (ttl > 0)
                {
                    renewEvent.Notify(this);
                } else
                {
                    status = PAUSE;
                    errorEvent.Notify(this, new NamespaceLeaseNotFoundException($"Lease {id} 未找到"));
                    Resume();
                }
            });
        }
    }

}
