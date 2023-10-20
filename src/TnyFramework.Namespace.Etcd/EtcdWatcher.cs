// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.Threading;
using System.Threading.Tasks;
using Etcdserverpb;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using TnyFramework.Common.Event;
using TnyFramework.Common.Extensions;
using TnyFramework.Common.Logger;
using TnyFramework.Common.Tasks;
using TnyFramework.Coroutines.Async;
using TnyFramework.Namespace.Etcd.Exceptions;
using TnyFramework.Namespace.Etcd.Listener;

namespace TnyFramework.Namespace.Etcd
{

    internal class EtcdWatcher : IEtcdWatcher
    {
        private static readonly ILogger LOGGER = LogFactory.Logger<EtcdWatcher>();

        private const long EMPTY_ID = -1L;

        private readonly string key;

        private readonly EtcdAccessor client;

        private readonly ICoroutine coroutine;

        private readonly EtcdWatchOption option;

        private volatile ITaskCompletionSource? watchSource;

        private volatile CancellationTokenSource? watchCancel;

        private volatile bool started;

        private volatile bool closed;

        private long id = EMPTY_ID;

        private long revision;

        private readonly IEventBus<EtcdWatchOnWatch> watchEvent = EventBuses.Create<EtcdWatchOnWatch>();

        private readonly IEventBus<EtcdWatchOnChange> changeEvent = EventBuses.Create<EtcdWatchOnChange>();

        private readonly IEventBus<EtcdWatchOnError> errorEvent = EventBuses.Create<EtcdWatchOnError>();

        private readonly IEventBus<EtcdWatchOnCompleted> completedEvent = EventBuses.Create<EtcdWatchOnCompleted>();

        public EtcdWatcher(EtcdAccessor client, string key, EtcdWatchOption option)
        {
            coroutine = DefaultCoroutineFactory.Default.Create("EtcdWatcher");
            this.client = client;
            this.key = key;
            this.option = option;
            revision = option.Revision;
        }

        public IEventBox<EtcdWatchOnWatch> WatchEvent => watchEvent;

        public IEventBox<EtcdWatchOnChange> ChangeEvent => changeEvent;

        public IEventBox<EtcdWatchOnError> ErrorEvent => errorEvent;

        public IEventBox<EtcdWatchOnCompleted> CompletedEvent => completedEvent;

        public bool IsClosed() => closed;

        public Task Close()
        {
            return coroutine.AsyncFunc(() => {
                if (closed)
                {
                    return Task.CompletedTask;
                }
                closed = true;
                watchCancel?.Cancel();
                return Task.CompletedTask;
            });
        }

        internal Task Resume()
        {
            if (IsClosed())
            {
                return Task.CompletedTask;
            }

            return coroutine.AsyncExec(async () => {
                try
                {
                    if (started)
                    {
                        return;
                    }
                    id = EMPTY_ID;
                    started = true;
                    var createRequest = new WatchCreateRequest {
                        Key = EtcdObject.Key(key),
                        PrevKv = option.PrevKv,
                        ProgressNotify = option.ProgressNotify
                    };
                    if (option.EndKey.IsNotBlank())
                    {
                        createRequest.RangeEnd = EtcdObject.EndKey(option.EndKey);
                    } else
                    {
                        if (option.Prefix)
                        {
                            var endKey = EtcdObject.EndKey(key);
                            createRequest.RangeEnd = endKey;
                        }
                    }

                    if (option.NoDelete)
                    {
                        createRequest.Filters.Add(WatchCreateRequest.Types.FilterType.Nodelete);
                    }

                    if (option.NoPut)
                    {
                        createRequest.Filters.Add(WatchCreateRequest.Types.FilterType.Noput);
                    }

                    var watchRequest = new WatchRequest {
                        CreateRequest = createRequest
                    };
                    watchSource = new NoneTaskCompletionSource();
                    watchCancel = new CancellationTokenSource();
                    var _ = client.Watch(watchRequest, HandleWatch, null, null, watchCancel.Token);
                    await watchSource.Task;
                    watchCancel.Token.Register(() => coroutine.Exec(async () => await HandleCompleted()));
                } catch (Exception exception)
                {
                    HandleError(exception);
                }
            });
        }

        private Task HandleCompleted()
        {
            if (id == -1)
                return Task.CompletedTask;
            id = -1;
            FireCompleted();
            var request = new WatchRequest {
                CancelRequest = new WatchCancelRequest {
                    WatchId = id
                }
            };
            var _ = client.Watch(request, HandleUnwatch);
            watchCancel = null;
            watchSource = null;
            return Task.CompletedTask;
        }

        private void HandleWatch(WatchResponse response)
        {
            coroutine.AsyncAction(() => {
                if (closed)
                {
                    return;
                }
                if (response.Created && response.Canceled && response.CancelReason != null &&
                    response.CancelReason.Contains("etcdserver: permission denied"))
                {
                    var error = new Status(StatusCode.Cancelled, response.CancelReason);
                    var exception = EtcdExceptionFactory.ToEtcdException(error);
                    HandleError(exception, true);
                } else if (response.Created)
                {
                    if (response.WatchId == EMPTY_ID)
                    {
                        var exception = EtcdExceptionFactory.NewEtcdException(StatusCode.Internal, "etcd server failed to create watch id");
                        FireError(exception);
                        return;
                    }
                    revision = Math.Max(revision, response.Header.Revision);
                    id = response.WatchId;
                    watchSource?.SetResult();
                    watchEvent.Notify();
                } else if (response.Canceled)
                {
                    var reason = response.CancelReason;
                    Exception error;
                    if (response.CompactRevision != 0)
                    {
                        error = EtcdExceptionFactory.NewCompactedException(response.CompactRevision);
                    } else if (reason.IsBlank())
                    {
                        error = EtcdExceptionFactory.NewEtcdException(StatusCode.OutOfRange,
                            "etcdserver: mvcc: required revision is a future revision");
                    } else
                    {
                        error = EtcdExceptionFactory.NewEtcdException(StatusCode.FailedPrecondition, reason ?? "");
                    }
                    HandleError(EtcdExceptionFactory.ToEtcdException(error), false);
                } else if (IsProgressNotify(response))
                {
                    FireNext(new WatchResponse(response));
                    revision = Math.Max(revision, response.Header.Revision);
                } else if (response.Events.Count == 0 && option.ProgressNotify)
                {
                    FireNext(new WatchResponse(response));
                    revision = response.Header.Revision;
                } else if (response.Events.Count > 0)
                {
                    FireNext(new WatchResponse(response));
                    var index = response.Events.Count - 1;
                    revision = response.Events[index].Kv.ModRevision + 1;
                }
            });
        }

        private static bool IsProgressNotify(WatchResponse response)
        {
            return response.Events.Count == 0 && !response.Created && !response.Canceled
                   && response.CompactRevision == 0 && response.Header.Revision != 0;
        }

        private void HandleError(Exception exception)
        {
            HandleError(EtcdExceptionFactory.ToEtcdException(exception), ShouldReschedule(EtcdErrors.FormatException(exception)));
        }

        private void HandleError(EtcdException exception, bool shouldReschedule)
        {
            if (closed)
            {
                return;
            }
            FireError(exception);
            started = false;

            if (shouldReschedule)
            {
                Reschedule();
                return;
            }
            Close();
        }

        private static void HandleUnwatch(WatchResponse response)
        {
        }

        private void Reschedule()
        {
            coroutine.AsyncExec(async () => {
                try
                {
                    await Task.Delay(500);
                    var _ = Resume();
                } catch (Exception e)
                {
                    LOGGER.LogError(e, "scheduled resume failed");
                }
            });
        }

        private void FireError(Exception exception)
        {
            errorEvent.Notify(exception);
        }

        private void FireNext(WatchResponse response)
        {
            changeEvent.Notify(response);
        }

        private void FireCompleted()
        {
            completedEvent.Notify();
        }

        private static bool ShouldReschedule(Status status)
        {
            return !EtcdErrors.IsHaltError(status) && !EtcdErrors.IsNoLeaderError(status);
        }
    }

}
