// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TnyFramework.Codec;
using TnyFramework.Common.Event;
using TnyFramework.Common.Extensions;
using TnyFramework.Common.Logger;
using TnyFramework.Common.Tasks;
using TnyFramework.Coroutines.Async;
using TnyFramework.Namespace.Exceptions;
using TnyFramework.Namespace.Listener;
using TnyFramework.Namespace.Sharding;

namespace TnyFramework.Namespace.Etcd
{

    public class EtcdHashingSubscriber<TValue> : EtcdHashing<TValue>, IHashingSubscriber<TValue>
    {
        private static readonly ILogger LOGGER = LogFactory.Logger<EtcdHashing<TValue>>();

        private const long WATCH_ALL_SLOT = -1L;

        private volatile ImmutableDictionary<long, RangeWatcher<TValue>> watcherMap =
            new Dictionary<long, RangeWatcher<TValue>>().ToImmutableDictionary();

        private readonly IEventBus<OnNodeLoad<TValue>> loadEvent = EventBuses.Create<OnNodeLoad<TValue>>();

        private readonly IEventBus<OnNodeCreate<TValue>> createEvent = EventBuses.Create<OnNodeCreate<TValue>>();

        private readonly IEventBus<OnNodeUpdate<TValue>> updateEvent = EventBuses.Create<OnNodeUpdate<TValue>>();

        private readonly IEventBus<OnNodeDelete<TValue>> deleteEvent = EventBuses.Create<OnNodeDelete<TValue>>();

        private readonly ICoroutine coroutine;

        private bool closed;

        public EtcdHashingSubscriber(string path, long maxSlot, ObjectMimeType<TValue> mineType, INamespaceExplorer explorer)
            : base(path, maxSlot, mineType, explorer)
        {
            coroutine = DefaultCoroutineFactory.Default.Create("EtcdHashingSubscriber");
        }

        public Task SubscribeAll()
        {
            return coroutine.AsyncExec(async () => {
                if (closed)
                {
                    throw new NamespaceHashingSubscriberClosedException($"EtcdHashingSubscriber {Path} close");
                }
                var existMap = new Dictionary<long, RangeWatcher<TValue>>(watcherMap);
                var watchers = new Dictionary<long, RangeWatcher<TValue>>();
                if (!existMap.TryGetValue(WATCH_ALL_SLOT, out var exist))
                {
                    watchers = AddAllSubscription();
                } else
                {
                    existMap.Remove(WATCH_ALL_SLOT);
                    watchers.Add(WATCH_ALL_SLOT, exist);
                }
                await HandleWatchers(existMap, watchers);
            });
        }

        public Task Subscribe<TRange>(IList<TRange> ranges) where TRange : ShardingRange
        {
            return coroutine.AsyncExec(async () => {
                if (closed)
                {
                    throw new NamespaceHashingSubscriberClosedException($"EtcdHashingSubscriber {Path} close");
                }
                var existMap = new Dictionary<long, RangeWatcher<TValue>>(watcherMap);
                var watchers = new Dictionary<long, RangeWatcher<TValue>>();
                foreach (var range in ranges)
                {
                    if (!existMap.TryGetValue(range.ToSlot, out var exist))
                    {
                        AddSubscription(range, watchers);
                    } else
                    {
                        existMap.Remove(range.ToSlot);
                        _ = exist.Close();
                        AddSubscription(range, watchers);
                        watchers.Add(WATCH_ALL_SLOT, exist);
                    }
                }
                await HandleWatchers(existMap, watchers);
            });
        }

        private async Task HandleWatchers(Dictionary<long, RangeWatcher<TValue>> existMap, Dictionary<long, RangeWatcher<TValue>> watchers)
        {

            foreach (var _ in existMap.Values.Select(watcher => watcher.Close()))
            {

            }
            watcherMap = watchers.ToImmutableDictionary();
            foreach (var watcher in watcherMap.Values)
            {
                try
                {
                    await watcher.WatchTask;
                } catch (Exception e)
                {
                    LOGGER.LogError(e, "");
                }
            }

        }

        private Dictionary<long, RangeWatcher<TValue>> AddAllSubscription()
        {
            var newSub = RangeWatcher.OfAll(this, coroutine);
            newSub.Watch();
            return new Dictionary<long, RangeWatcher<TValue>> {{WATCH_ALL_SLOT, newSub}};
        }

        private void AddSubscription(ShardingRange range, Dictionary<long, RangeWatcher<TValue>> dictionary)
        {
            var newSub = RangeWatcher.OfRange(this, range, coroutine);
            if (dictionary.ContainsKey(range.ToSlot))
                return;
            dictionary[range.ToSlot] = newSub;
            newSub.Watch();
        }

        public Task Unsubscribe()
        {
            return coroutine.AsyncExec(async () => {
                var tasks = watcherMap.Values.Select(w => w.Close()).ToList();
                watcherMap = ImmutableDictionary.Create<long, RangeWatcher<TValue>>();
                foreach (var task in tasks)
                {
                    try
                    {
                        await task;
                    } catch (Exception e)
                    {
                        LOGGER.LogError(e, "");
                    }

                }
            });
        }

        public void ClearListener()
        {
            coroutine.ExecAction(() => {
                loadEvent.Clear();
                createEvent.Clear();
                updateEvent.Clear();
                deleteEvent.Clear();
            });
        }

        public void Close()
        {
            coroutine.AsyncExec(async () => {
                if (closed)
                {
                    return;
                }
                closed = true;
                await Unsubscribe();
                ClearListener();
            });
        }

        public IEventBox<OnNodeLoad<TValue>> LoadEvent => loadEvent;

        public IEventBox<OnNodeCreate<TValue>> CreateEvent => createEvent;

        public IEventBox<OnNodeUpdate<TValue>> UpdateEvent => updateEvent;

        public IEventBox<OnNodeDelete<TValue>> DeleteEvent => deleteEvent;

        internal void OnLoad(INameNodesWatcher<TValue> watcher, List<NameNode<TValue>> nameNodes)
        {
            loadEvent.Notify(watcher, nameNodes);
        }

        internal void OnCreate(INameNodesWatcher<TValue> watcher, NameNode<TValue> node)
        {
            createEvent.Notify(watcher, node);
        }

        internal void OnUpdate(INameNodesWatcher<TValue> watcher, NameNode<TValue> node)
        {
            updateEvent.Notify(watcher, node);
        }

        internal void OnDelete(INameNodesWatcher<TValue> watcher, NameNode<TValue> node)
        {
            deleteEvent.Notify(watcher, node);
        }
    }

    internal class RangeWatcher
    {
        internal static RangeWatcher<TValue> OfRange<TValue>(EtcdHashingSubscriber<TValue> parent, ShardingRange range, ICoroutine coroutine)
        {
            return new RangeWatcher<TValue>(parent, range, coroutine);
        }

        internal static RangeWatcher<TValue> OfAll<TValue>(EtcdHashingSubscriber<TValue> parent, ICoroutine coroutine)
        {
            return new RangeWatcher<TValue>(parent, null, coroutine);
        }
    }

    internal class RangeWatcher<TValue> : RangeWatcher
    {
        private static readonly ILogger LOGGER = LogFactory.Logger<RangeWatcher<TValue>>();

        private readonly EtcdHashingSubscriber<TValue> parent;

        private readonly ShardingRange? shardingRange;

        private readonly ITaskCompletionSource watchTaskSource;

        private readonly ICoroutine coroutine;

        private bool start;

        private int retryTimes;

        private List<INameNodesWatcher<TValue>>? watchers;

        internal RangeWatcher(EtcdHashingSubscriber<TValue> parent, ShardingRange? shardingRange, ICoroutine coroutine)
        {
            this.parent = parent;
            this.shardingRange = shardingRange;
            this.coroutine = coroutine;
            watchTaskSource = new NoneTaskCompletionSource();
        }

        public Task WatchTask => watchTaskSource.Task;

        internal void Watch()
        {
            coroutine.AsyncExec(async () => {
                if (start)
                {
                    return;
                }
                start = true;
                try
                {
                    IEnumerable<INameNodesWatcher<TValue>> watcherEnumerable;
                    if (shardingRange == null)
                    {
                        watcherEnumerable = new[] {parent.Explorer.AllNodeWatcher(parent.Path, parent.MineType)};
                    } else
                    {
                        watcherEnumerable = shardingRange.GetRanges().Select(range => {
                            var lower = range.Min;
                            var upper = range.Max;
                            if (!range.Contains(lower) && lower < upper)
                            {
                                lower++;
                            }
                            if (!range.Contains(upper) && lower < upper)
                            {
                                upper--;
                            }
                            if (!range.Contains(lower) || !range.Contains(upper))
                            {
                                throw new NamespaceHashingException($"It is illegal to {range.Min} to {range.Max} for the interval");
                            }
                            if (lower == upper)
                            {
                                var path = parent.SubPath(range.Min);
                                return parent.Explorer.AllNodeWatcher(path, parent.MineType);
                            }
                            var from = parent.SubPath(lower);
                            var to = parent.SubPath(upper + 1);
                            return parent.Explorer.AllNodeWatcher(from, to, parent.MineType);
                        });
                    }
                    watchers = watcherEnumerable.ToList();
                    var tasks = watchers.Select(watcher => {
                            watcher.LoadEvent.Add(parent.OnLoad);
                            watcher.CreateEvent.Add(parent.OnCreate);
                            watcher.UpdateEvent.Add(parent.OnUpdate);
                            watcher.DeleteEvent.Add(parent.OnDelete);
                            return DoWatch(watcher);
                        })
                        .ToList();
                    foreach (var wTask in tasks)
                    {
                        await wTask;
                    }
                    watchTaskSource.SetResult();
                } catch (Exception e)
                {
                    LOGGER.LogError(e, "");
                    watchTaskSource.SetException(e);
                }
            });
        }

        private async Task DoWatch(INameNodesWatcher<TValue> watcher)
        {
            await coroutine.AsyncExec(async () => {
                var success = false;
                while (!success)
                {
                    try
                    {
                        if (!start)
                        {
                            return;
                        }
                        if (watcher.IsWatch())
                        {
                            return;
                        }
                        var result = await watcher.Watch();
                        if (result.IsNull())
                        {
                            retryTimes = 0;
                            success = true;
                            return;
                        }
                    } catch (Exception e)
                    {
                        LOGGER.LogError(e, "");
                    }
                    retryTimes++;
                    await Task.Delay(Math.Min(1000 * retryTimes, 30000));
                }
            });
        }

        internal Task Close()
        {
            return coroutine.AsyncExec(() => {
                if (!start)
                {
                    return Task.CompletedTask;
                }
                start = false;
                watchers?.ForEach(watcher => watcher.Unwatch());
                watchers?.Clear();
                return Task.CompletedTask;
            });
        }
    }

}
