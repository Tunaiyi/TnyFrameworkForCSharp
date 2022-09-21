// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.Threading.Tasks;
using dotnet_etcd;
using Etcdserverpb;
using Microsoft.Extensions.Logging;
using TnyFramework.Codec;
using TnyFramework.Common.Event;
using TnyFramework.Common.Extensions;
using TnyFramework.Common.Logger;
using TnyFramework.Coroutines.Async;
using TnyFramework.Namespace.Exceptions;
using TnyFramework.Namespace.Listener;

namespace TnyFramework.Namespace.Etcd
{

    public abstract class EtcdNameNodeWatcher : EtcdObject, INameNodesWatcher
    {
        protected static readonly ILogger LOGGER = LogFactory.Logger<EtcdNameNodeWatcher>();

        protected const int UNWATCH = 0;

        protected const int TRY_WATCH = 1;

        protected const int WATCH = 2;

        protected readonly bool match;

        protected volatile int status = UNWATCH;

        protected readonly IEventBus<OnWatch> watchEvent = EventBuses.Create<OnWatch>();

        protected readonly IEventBus<OnComplete> completeEvent = EventBuses.Create<OnComplete>();

        protected readonly IEventBus<OnError> errorEvent = EventBuses.Create<OnError>();

        public string WatchPath { get; }

        protected EtcdNameNodeWatcher(string watchPath, bool match, EtcdAccessor accessor, ObjectCodecAdapter objectCodecAdapter)
            : base(accessor, objectCodecAdapter)
        {
            this.match = match;
            WatchPath = watchPath;

        }

        public bool IsMatch() => match;

        public bool IsUnwatch() => status == UNWATCH;

        public bool IsWatch() => status == WATCH;

        public abstract Task Unwatch();

        public IEventBox<OnWatch> WatchEvent => watchEvent;

        public IEventBox<OnComplete> CompleteEvent => completeEvent;

        public IEventBox<OnError> ErrorEvent => errorEvent;
    }

    public class EtcdNameNodesWatcher<TValue> : EtcdNameNodeWatcher, INameNodesWatcher<TValue>
    {
        private readonly string key;

        private readonly string endKey;

        private readonly ICoroutine coroutine;

        private readonly ObjectMimeType<TValue> valueType;

        private readonly IEventBus<OnNodeLoad<TValue>> loadEvent = EventBuses.Create<OnNodeLoad<TValue>>();

        private readonly IEventBus<OnNodeCreate<TValue>> createEvent = EventBuses.Create<OnNodeCreate<TValue>>();

        private readonly IEventBus<OnNodeUpdate<TValue>> updateEvent = EventBuses.Create<OnNodeUpdate<TValue>>();

        private readonly IEventBus<OnNodeDelete<TValue>> deleteEvent = EventBuses.Create<OnNodeDelete<TValue>>();

        private IEtcdWatcher watcher;

        public EtcdNameNodesWatcher(string watchPath, bool match, EtcdAccessor client, ObjectMimeType<TValue> valueType,
            ObjectCodecAdapter objectCodecAdapter)
            : this(watchPath, null, match, client, valueType, objectCodecAdapter)
        {
            coroutine = DefaultCoroutineFactory.Default.Create("EtcdNameNodeWatcher");
        }

        public EtcdNameNodesWatcher(string watchPath, string endPath, bool match, EtcdAccessor accessor, ObjectMimeType<TValue> valueType,
            ObjectCodecAdapter objectCodecAdapter)
            : base(watchPath, match, accessor, objectCodecAdapter)
        {
            this.valueType = valueType;
            key = watchPath;
            endKey = endPath != null ? EtcdClient.GetRangeEnd(key) : null;
            coroutine = DefaultCoroutineFactory.Default.Create("EtcdNameNodeWatcher");
        }

        public IEventBox<OnNodeLoad<TValue>> LoadEvent => loadEvent;

        public IEventBox<OnNodeCreate<TValue>> CreateEvent => createEvent;

        public IEventBox<OnNodeUpdate<TValue>> UpdateEvent => updateEvent;

        public IEventBox<OnNodeDelete<TValue>> DeleteEvent => deleteEvent;

        public Task<INameNodesWatcher<TValue>> Watch()
        {
            return coroutine.AsyncExec(async () => {
                if (status != UNWATCH)
                {
                    throw new NameNodeWatchException($"watch failed when status is {status}");
                }
                if (status != UNWATCH)
                {
                    throw new NameNodeWatchException($"watch failed when status is {status}");
                }
                status = TRY_WATCH;
                try
                {
                    var rangeRequest = new RangeRequest {
                        Key = Key(key)
                    };
                    if (endKey.IsNotBlank())
                    {
                        rangeRequest.RangeEnd = EndKey(endKey);
                    } else
                    {
                        if (match)
                        {
                            rangeRequest.RangeEnd = EndKey(WatchPath);
                        }
                    }
                    var response = await accessor.GetAsync(rangeRequest);
                    return await DoWatch(response);
                } catch (Exception e)
                {
                    LOGGER.LogError(e, "watch {path} exception", WatchPath);
                    status = UNWATCH;
                    throw;
                }
            });
        }

        private async Task<INameNodesWatcher<TValue>> DoWatch(RangeResponse response)
        {
            try
            {
                if (status != TRY_WATCH)
                {
                    throw new NameNodeWatchException($"watch {WatchPath} failed when status is {status}");
                }
                var watchRevision = response.Header.Revision;
                var node = DecodeAllKeyValues(response.Kvs, valueType);
                loadEvent.Notify(this, node);
                var option = new EtcdWatchOption {
                    EndKey = endKey,
                    PrevKv = true,
                    Prefix = match,
                    ProgressNotify = true,
                    Revision = watchRevision
                };
                var etcdWatcher = new EtcdWatcher(accessor, key, option);
                etcdWatcher.WatchEvent.Add(OnWatch);
                etcdWatcher.ChangeEvent.Add(OnChange);
                etcdWatcher.CompletedEvent.Add(OnCompleted);
                etcdWatcher.ErrorEvent.Add(OnError);
                await etcdWatcher.Resume();
                watcher = etcdWatcher;
                status = WATCH;
                return this;
            } catch (Exception e)
            {
                status = UNWATCH;
                var cause = new NameNodeWatchException($"watch {WatchPath} exception", e);
                throw cause;
            }
        }

        public override Task Unwatch()
        {
            return coroutine.AsyncExec(async () => {
                if (watcher == null)
                    return;
                await watcher.Close();
                watcher = null;
                status = UNWATCH;
            });
        }

        private void OnChange(WatchResponse response)
        {
            coroutine.AsyncAction(() => {
                foreach (var respEvent in response.Events)
                {
                    var kv = respEvent.Kv;
                    if (kv.Version == 0)
                    {
                        var removeKv = respEvent.PrevKv;
                        var node = Decode(removeKv.Value, kv, removeKv.CreateRevision, removeKv.Version, valueType);
                        deleteEvent.Notify(this, node);
                    } else
                    {
                        var preKv = respEvent.PrevKv;
                        var node = Decode(respEvent.Kv, valueType);
                        LOGGER.LogInformation($"{respEvent.Kv.Version} | {node}");
                        if (preKv == null && kv.Version == 1)
                        {
                            createEvent.Notify(this, node);
                        } else
                        {
                            updateEvent.Notify(this, node);
                        }
                    }
                }
            });
        }

        private void OnWatch()
        {
            coroutine.AsyncAction(() => watchEvent.Notify(this));
        }

        private void OnCompleted()
        {
            coroutine.AsyncAction(() => completeEvent.Notify(this));
        }

        private void OnError(Exception exception)
        {
            coroutine.AsyncAction(() => errorEvent.Notify(this, exception));
        }
    }

}
