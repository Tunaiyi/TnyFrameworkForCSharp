// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TnyFramework.Codec;
using TnyFramework.Namespace.Sharding;
using CollectionExtensions = TnyFramework.Common.Extensions.CollectionExtensions;

namespace TnyFramework.Namespace.Etcd;

internal class PartitionComparer<TNode> : IComparer<IPartition<TNode>>
    where TNode : IShardingNode
{
    public int Compare(IPartition<TNode>? x, IPartition<TNode>? y)
    {
        var nodeKey = string.Compare(x?.NodeKey, y?.NodeKey, StringComparison.Ordinal);
        return nodeKey != 0 ? nodeKey : x!.Slot.CompareTo(y!.Slot);
    }
};

public class EtcdNodeHashingMultimap<TNode> : EtcdNodeHashing<TNode>
    where TNode : IShardingNode
{
    // 节点
    private readonly Dictionary<long, SortedSet<IPartition<TNode>>> slotPartitionsMap = new Dictionary<long, SortedSet<IPartition<TNode>>>();

    private readonly Dictionary<string, IPartition<TNode>> partitionMap = new Dictionary<string, IPartition<TNode>>();

    private volatile List<ShardingRange<TNode>>? ranges;

    private static readonly PartitionComparer<TNode> PARTITION_COMPARER = new PartitionComparer<TNode>();

    private readonly ReaderWriterLockSlim mutex = new ReaderWriterLockSlim();

    public EtcdNodeHashingMultimap(string path, HashingOptions<TNode> option, EtcdNamespaceExplorer explorer,
        ObjectCodecAdapter objectCodecAdapter)
        : base(path, option, explorer, objectCodecAdapter, false)
    {
    }

    private void WriterLock()
    {
        mutex.EnterWriteLock();
    }

    private void WriterUnlock()
    {
        mutex.ExitWriteLock();
    }

    private void ReaderUpgradeableLock()
    {
        mutex.EnterUpgradeableReadLock();
    }

    private void ReaderUpgradeableUnlock()
    {
        mutex.ExitUpgradeableReadLock();
    }

    private void ReaderLock()
    {
        mutex.EnterReadLock();
    }

    private void ReaderUnlock()
    {
        mutex.ExitReadLock();
    }

    protected override void LoadPartitions(IEnumerable<IPartition<TNode>> partitions)
    {
        WriterLock();
        try
        {
            var addList = partitions.Where(DoAddPartition).ToList();
            if (CollectionExtensions.IsNullOrEmpty(addList))
                return;
            ResetRange();
            FireChange(this, addList);
        } finally
        {
            WriterUnlock();
        }
    }

    protected override void PutPartition(IPartition<TNode> partition)
    {
        WriterLock();
        try
        {
            if (!DoReplacePartition(partition))
                return;
            ResetRange();
            FireChange(this, new List<IPartition<TNode>> {partition});
        } finally
        {
            WriterUnlock();
        }
    }

    protected override void RemovePartition(IPartition<TNode> partition)
    {
        WriterLock();
        try
        {
            var exist = DoRemovePartition(partition);
            if (exist == null)
                return;
            ResetRange();
            FireChange(this, new List<IPartition<TNode>> {exist});
        } finally
        {
            WriterUnlock();
        }
    }

    private bool DoAddPartition(IPartition<TNode> partition)
    {
        if (!partitionMap.TryAdd(partition.Key, partition))
        {
            return false;
        }
        var partitions = PartitionSet(partition.Slot);
        return partitions.Add(partition);
    }

    private bool DoReplacePartition(IPartition<TNode> partition)
    {
        partitionMap[partition.Key] = partition;
        if (partitionMap.TryGetValue(partition.Key, out var exist))
        {
            GetPartitionSet(partition.Slot)?.Remove(exist);
        }
        var partitions = PartitionSet(partition.Slot);
        return partitions.Add(partition);
    }

    private IPartition<TNode>? DoRemovePartition(IPartition<TNode> partition)
    {
        if (!partitionMap.TryGetValue(partition.Key, out var exist))
        {
            return default;
        }
        partitionMap.Remove(partition.Key);
        GetPartitionSet(partition.Slot)?.Remove(exist);
        return exist;
    }

    private SortedSet<IPartition<TNode>>? GetPartitionSet(long slot)
    {
        if (slotPartitionsMap.TryGetValue(slot, out var partitions))
        {
            return partitions;
        } else
        {
            return null;
        }
    }

    private SortedSet<IPartition<TNode>> PartitionSet(long slot)
    {
        if (slotPartitionsMap.TryGetValue(slot, out var partitions))
        {
            return partitions;
        }
        partitions = new SortedSet<IPartition<TNode>>(PARTITION_COMPARER);
        slotPartitionsMap[slot] = partitions;
        return partitions;
    }

    protected override string PartitionPath(string slotPath, IPartition<TNode> partition)
    {
        return NamespacePathNames.NodePath(slotPath, partition.Key);
    }

    protected override void DoShutdown()
    {
    }

    public override bool Contains(IPartition<TNode> partition)
    {
        ReaderLock();
        try
        {
            return slotPartitionsMap.TryGetValue(partition.Slot, out var partitions) && partitions.Contains(partition);
        } finally
        {
            ReaderUnlock();
        }
    }

    public override List<IPartition<TNode>> FindPartitions(string nodeId)
    {
        ReaderLock();
        try
        {
            return slotPartitionsMap.Values
                .SelectMany(partitions => partitions)
                .Where(partition => string.Equals(nodeId, partition.NodeKey))
                .ToList();
        } finally
        {
            ReaderUnlock();
        }
    }

    public override List<ShardingRange<TNode>> FindRanges(string nodeId)
    {
        ReaderLock();
        try
        {
            return slotPartitionsMap.Values
                .SelectMany(partitions => partitions)
                .Where(partition => string.Equals(nodeId, partition.NodeKey))
                .Select(partition => new ShardingRange<TNode>(partition, partition.Slot, MaxSlots))
                .ToList();
        } finally
        {
            ReaderUnlock();
        }
    }

    public override List<ShardingRange<TNode>> GetAllRanges()
    {
        ReaderUpgradeableLock();
        try
        {
            var value = ranges;
            if (value != null)
            {
                return value;
            }
            WriterLock();
            try
            {
                value = ranges;
                if (value != null)
                {
                    return value;
                }
                return ranges = slotPartitionsMap.Values
                    .SelectMany(partitions => partitions)
                    .Select(partition => new ShardingRange<TNode>(partition, partition.Slot, MaxSlots))
                    .ToList();
            } finally
            {
                WriterUnlock();
            }
        } finally
        {
            ReaderUpgradeableUnlock();
        }

    }

    public override IPartition<TNode>? PrevPartition(long slot)
    {
        return LocateBySlot(slot);
    }

    public override IPartition<TNode>? NextPartition(long slot)
    {
        return LocateBySlot(slot);
    }

    public override List<IPartition<TNode>> GetAllPartitions()
    {
        ReaderLock();
        try
        {
            return slotPartitionsMap.Values.SelectMany(p => p).ToList();
        } finally
        {
            ReaderUnlock();
        }
    }

    public override IPartition<TNode>? Locate(string key)
    {
        return LocateBySlot(KeyHash(key));
    }

    public override List<IPartition<TNode>> Locate(string key, int count)
    {
        ReaderLock();
        try
        {
            return LocateBySlot(KeyHash(key), count);
        } finally
        {
            ReaderUnlock();
        }
    }

    public override int PartitionSize()
    {
        ReaderLock();
        try
        {
            return partitionMap.Count;
        } finally
        {
            ReaderUnlock();
        }
    }

    private IPartition<TNode>? LocateBySlot(long slot)
    {
        ReaderLock();
        try
        {
            var index = slot % MaxSlots;
            return GetPartitionSet(index)?.First();
        } finally
        {
            ReaderUnlock();
        }
    }

    private List<IPartition<TNode>> LocateBySlot(long slot, int count)
    {
        ReaderLock();
        try
        {
            var index = slot % MaxSlots;
            var partitions = GetPartitionSet(index);
            return partitions == null ? new List<IPartition<TNode>>() : partitions.Take(count).ToList();
        } finally
        {
            ReaderUnlock();
        }
    }

    private void ResetRange()
    {
        ranges = null;
    }
}
