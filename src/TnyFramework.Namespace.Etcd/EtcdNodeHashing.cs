// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TnyFramework.Codec;
using TnyFramework.Codec.Newtonsoft.Json;
using TnyFramework.Common.Event;
using TnyFramework.Common.EventBus;
using TnyFramework.Common.Exceptions;
using TnyFramework.Common.Extensions;
using TnyFramework.Common.Logger;
using TnyFramework.Common.Util;
using TnyFramework.Coroutines.Async;
using TnyFramework.Namespace.Exceptions;
using TnyFramework.Namespace.Sharding;
using TnyFramework.Namespace.Sharding.Listener;

namespace TnyFramework.Namespace.Etcd
{

    public abstract class EtcdNodeHashing
    {
        private static int _INDEX;

        internal static int NextIndex()
        {
            return Interlocked.Increment(ref _INDEX);
        }
    }

    public abstract class EtcdNodeHashing<TNode> : EtcdObject, INodeHashing<TNode>
        where TNode : IShardingNode
    {
        private static readonly ILogger LOGGER = LogFactory.Logger<EtcdNodeHashing<TNode>>();

        private const int INIT = 0;

        private const int STARTING = 1;

        private const int EXECUTE = 2;

        private const int CLOSED = 3;

        // 分区路径
        public string Path { get; }

        // 名字
        public string Name { get; }

        // hash 算法
        protected IHasher<string> KeyHasher { get; }

        // hash 算法
        public IHasher<IPartitionSlot<TNode>> NodeHasher { get; }

        // 序列化类型
        private readonly ObjectMimeType<PartitionSlot<TNode>> partitionMineType;

        protected readonly ICoroutine coroutine;

        private readonly INamespaceExplorer explorer;

        private readonly long ttl;

        // 租客
        private volatile ILessee? lessee;

        // 分区监控器
        private volatile INameNodesWatcher<PartitionSlot<TNode>>? partitionWatcher;

        // 本地节点
        private readonly ConcurrentDictionary<string, List<EtcdPartitionRegisterTask<TNode>>> nodePartitionTaskMap = new();

        // 状态
        private volatile int status = INIT;

        // 每一节点分区数
        private readonly int partitionCount;

        private readonly bool enableRehash;

        private readonly IEventBus<ShardingOnChange<TNode>> onChangeEvent = EventBuses.Create<ShardingOnChange<TNode>>();

        private readonly IEventBus<ShardingOnRemove<TNode>> onRemoveEvent = EventBuses.Create<ShardingOnRemove<TNode>>();

        public long MaxSlots { get; }

        public abstract bool Contains(IPartition<TNode> partition);

        public abstract List<IPartition<TNode>> FindPartitions(string nodeId);

        public abstract List<ShardingRange<TNode>> FindRanges(string nodeId);

        public abstract List<ShardingRange<TNode>> GetAllRanges();

        public abstract IPartition<TNode>? PrevPartition(long slot);

        public abstract IPartition<TNode>? NextPartition(long slot);

        public abstract List<IPartition<TNode>> GetAllPartitions();

        public abstract IPartition<TNode>? Locate(string key);

        public abstract List<IPartition<TNode>> Locate(string key, int count);

        public abstract int PartitionSize();

        protected EtcdNodeHashing(string path, HashingOptions<TNode> option, EtcdNamespaceExplorer explorer,
            ObjectCodecAdapter objectCodecAdapter, bool enableRehash) : base(explorer.Accessor, objectCodecAdapter)
        {
            Name = option.Name.IsBlank() ? "HashingRing-" + EtcdNodeHashing.NextIndex() : option.Name;
            this.enableRehash = enableRehash;
            this.explorer = explorer;
            Path = NamespacePathNames.DirPath(path);
            ttl = option.Ttl;
            partitionCount = Math.Max(option.PartitionCount, 1);
            KeyHasher = option.KeyHasher ?? throw new NullReferenceException($"{Path} key hasher is null");
            NodeHasher = option.NodeHasher ?? throw new NullReferenceException($"{Path} node hasher is null");
            MaxSlots = Math.Max(option.MaxSlots, 1);
            coroutine = DefaultCoroutineFactory.Default.Create(Name);
            if (partitionCount > this.MaxSlots)
            {
                throw new IllegalArgumentException($"partitionCount {partitionCount} must less or equals than maxSlots {MaxSlots}.");
            }
            partitionMineType = ObjectMimeType.Of<PartitionSlot<TNode>>(JsonMimeType.JSON);
        }

        public async Task<INodeHashing<TNode>> Start()
        {
            return await coroutine.AsyncExec(async () => {
                if (status != INIT)
                {
                    throw new NamespaceHashingException($"{Name} start failed, status is {status}");
                }
                try
                {
                    status = STARTING;
                    if (lessee == null)
                    {
                        await DoLeaseAndWatch();
                    } else
                    {
                        await DoWatch();
                    }
                    return this;
                } catch (Exception e)
                {
                    LOGGER.LogError(e, "");
                    OnStartFailed();
                    throw;
                }
            });
        }

        public IEventWatch<ShardingOnChange<TNode>> ChangeEvent => onChangeEvent;

        public IEventWatch<ShardingOnRemove<TNode>> RemoveEvent => onRemoveEvent;

        /// <summary>
        /// 键值 Hash 值
        /// </summary>
        /// <param name="key">键值</param>
        /// <returns>返回键值 hash 值</returns>
        protected long KeyHash(string key)
        {
            return Math.Abs(KeyHasher.Hash(key, 0, MaxSlots));
        }

        // /// <summary>
        // /// 节点hash 值
        // /// </summary>
        // /// <param name="slot">节点</param>
        // /// <param name="index">第几分区</param>
        // /// <returns>返回 hash 值</returns>
        // private long SlotHash(IPartitionSlot<TNode> slot, int index)
        // {
        //     return Math.Abs(NodeHasher.Hash(slot, index, MaxSlots));
        // }

        public Task<List<IPartition<TNode>>> Register(TNode node)
        {
            return RegisterNode(node, n => CreatePartitionTasks(CreatePartitionSlots(n), true));
        }

        public Task<List<IPartition<TNode>>> Register(TNode node, ISet<long> slotIndexes)
        {
            return RegisterNode(node, n => CreatePartitionTasks(CreatePartitionSlots(n), false));
        }

        public Task Shutdown()
        {
            return coroutine.AsyncAction(() => {
                if (status == CLOSED)
                {
                    return;
                }
                if (partitionWatcher != null)
                {
                    partitionWatcher.Unwatch();
                    partitionWatcher = null;
                }
                DoShutdown();
                foreach (var pair in nodePartitionTaskMap)
                {
                    pair.Value.ForEach(task => _ = task.Close());
                }
                nodePartitionTaskMap.Clear();
                if (lessee == null)
                    return;
                lessee.Shutdown();
                lessee = null;
            });
        }

        protected void FireChange(ISharding<TNode> sharding, List<IPartition<TNode>> partitions)
        {
            onChangeEvent.Notify(sharding, partitions);
        }

        protected void FireRemove(ISharding<TNode> sharding, List<IPartition<TNode>> partitions)
        {
            onRemoveEvent.Notify(sharding, partitions);
        }

        private void RestorePartition()
        {
            coroutine.AsyncAction(() => {
                foreach (var task in nodePartitionTaskMap.Values.SelectMany(tasks => tasks))
                {
                    task.Register();
                }
            });
        }

        private async Task<List<IPartition<TNode>>> RegisterNode(TNode node, Func<TNode, List<EtcdPartitionRegisterTask<TNode>>> tasksFactory)
        {

            return await coroutine.AsyncExec(() => {
                if (status != EXECUTE)
                {
                    throw new NamespaceHashingPartitionException($"ring is not start, status is {status}");
                }
                if (nodePartitionTaskMap.ContainsKey(node.Key))
                {
                    throw new NamespaceHashingPartitionException("register failed");
                }
                var partitionTasks = tasksFactory(node);
                if (!nodePartitionTaskMap.TryAdd(node.Key, partitionTasks))
                {
                    throw new NamespaceHashingPartitionException("register failed");
                }
                return RegisterPartitionTasks(partitionTasks);
            });
        }

        private async Task<List<IPartition<TNode>>> RegisterPartitionTasks(IEnumerable<EtcdPartitionRegisterTask<TNode>> tasks)
        {
            var registeringTasks = tasks.Select(task => task.Register()).ToList();
            var successList = new List<IPartition<TNode>>();
            foreach (var task in registeringTasks)
            {
                try
                {
                    var partition = await task;
                    if (partition.IsNotNull())
                    {
                        successList.Add(partition);
                    }
                } catch (Exception e)
                {
                    LOGGER.LogError(e, "");
                }
            }
            return successList;
        }

        private IEnumerable<PartitionSlot<TNode>> CreatePartitionSlots(TNode node)
        {
            var slotSet = new List<PartitionSlot<TNode>>();
            var count = Math.Min(MaxSlots, this.partitionCount);
            for (var index = 0; slotSet.Count < count; index++)
            {
                var slot = new PartitionSlot<TNode>(index, node);
                slot.Hash(NodeHasher, MaxSlots);
                slotSet.Add(slot);
            }
            return slotSet;
        }

        private List<EtcdPartitionRegisterTask<TNode>> CreatePartitionTasks(IEnumerable<PartitionSlot<TNode>> slots, bool rehash)
        {
            return slots.Select(slot => new EtcdPartitionRegisterTask<TNode>(this, slot, enableRehash && rehash)).ToList();
        }

        private void OnStartFailed()
        {
            if (status != STARTING)
            {
                return;
            }
            status = INIT;
        }

        private async Task DoLeaseAndWatch()
        {
            if (lessee != null)
            {
                return;
            }
            var result = await explorer.Lease(Name, ttl);
            lessee = result;
            lessee.LeaseEvent.Add(LesseeOnAction);
            lessee.CompletedEvent.Add(LesseeOnAction);
            await DoWatch();
        }

        private async Task DoWatch()
        {
            if (partitionWatcher != null)
            {
                return;
            }
            partitionWatcher = explorer.AllNodeWatcher(Path, partitionMineType);
            partitionWatcher.LoadEvent.Add(WatchOnLoad);
            partitionWatcher.CreateEvent.Add(WatchOnCreate);
            partitionWatcher.UpdateEvent.Add(WatchOnUpdate);
            partitionWatcher.DeleteEvent.Add(WatchOnDelete);
            var watcher = await partitionWatcher.Watch();
            await DoExecute(watcher);
        }

        private async Task DoExecute(INameNodesWatcher<PartitionSlot<TNode>> watcher)
        {
            if (status == STARTING)
            {
                partitionWatcher = watcher;
                status = EXECUTE;
            } else
            {
                await watcher.Unwatch();
            }
        }

        protected abstract void LoadPartitions(IEnumerable<IPartition<TNode>> partitions);

        protected abstract void PutPartition(IPartition<TNode> partition);

        protected abstract void RemovePartition(IPartition<TNode> partition);

        protected abstract void DoShutdown();

        private void WatchOnLoad(INameNodesWatcher<PartitionSlot<TNode>> watcher, IEnumerable<NameNode<PartitionSlot<TNode>>> nameNodes)
        {
            coroutine.ExecAction(() => {
                var partitions = (from node in nameNodes where node.Value != null select node.Value).ToList();
                LoadPartitions(partitions);
            });
        }

        private void WatchOnCreate(INameNodesWatcher<PartitionSlot<TNode>> watcher, NameNode<PartitionSlot<TNode>> node)
        {

            coroutine.ExecAction(() => { PutPartition(node.Value); });

        }

        private void WatchOnUpdate(INameNodesWatcher<PartitionSlot<TNode>> watcher, NameNode<PartitionSlot<TNode>> node)
        {
            coroutine.ExecAction(() => { PutPartition(node.Value); });
        }

        private void WatchOnDelete(INameNodesWatcher<PartitionSlot<TNode>> watcher, NameNode<PartitionSlot<TNode>> node)
        {
            coroutine.ExecAction(() => {
                var removeNode = node.Value;
                if (nodePartitionTaskMap.TryGetValue(removeNode.Node.Key, out var tasks))
                {
                    foreach (var task in tasks.Where(task => task.Partition.Slot == removeNode.Slot))
                    {
                        task.Register();
                    }
                }
                RemovePartition(node.Value);
            });
        }

        private void LesseeOnAction(ILessee source)
        {
            RestorePartition();
        }

        protected virtual string PartitionPath(string slotPath, IPartition<TNode> partition)
        {
            return slotPath;
        }

        internal async Task<int> RegisterPartition(EtcdPartitionRegisterTask<TNode> task)
        {
            var partition = task.Partition;
            if (lessee != null && !lessee.IsLive())
            {
                return lessee.IsShutdown() ? EtcdPartitionRegisterTask.REGISTER_RESULT_CANCEL : EtcdPartitionRegisterTask.REGISTER_RESULT_RETRY;
            }
            var slotPath = NamespacePathNames.NodePath(Path, NumberFormatAide.AlignDigits(partition.Slot, MaxSlots));
            var result = await explorer.Add(PartitionPath(slotPath, partition), partitionMineType, partition, lessee);
            return result.IsNotNull() ? EtcdPartitionRegisterTask.REGISTER_RESULT_SUCCESS : EtcdPartitionRegisterTask.REGISTER_RESULT_FAILED;
        }
    }

}
