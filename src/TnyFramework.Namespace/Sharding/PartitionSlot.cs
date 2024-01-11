// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;

namespace TnyFramework.Namespace.Sharding;

public interface IPartitionSlot<out TNode> : IPartition<TNode>
    where TNode : IShardingNode
{
}

public class PartitionSlot<TNode> : ShardingPartition<TNode>, IPartitionSlot<TNode>
    where TNode : IShardingNode
{
    public int Seed { get; set; }

    public PartitionSlot(int index, TNode node) : base(index, node)
    {
    }

    public PartitionSlot(int index, long slot, TNode node) : base(index, slot, node)
    {
    }

    public override void Hash(IHasher<PartitionSlot<TNode>> hasher, long maxSlots)
    {
        Seed++;
        Slot = Math.Abs(hasher.Hash(this, Seed, maxSlots));
    }
}
