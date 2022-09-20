// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.Collections.Generic;
using TnyFramework.Common.Event;
using TnyFramework.Namespace.Sharding.Listener;

namespace TnyFramework.Namespace.Sharding
{

    public class ConsistentHashRing<TNode> : IShardingSet<TNode>
        where TNode : IShardingNode
    {
        private string name;

        private IHasher<string> hasher;

        public long MaxSlots { get; }

        public ConsistentHashRing(string name, long maxSlots, IHasher<string> hasher)
        {
            this.name = name;
            this.hasher = hasher;
            MaxSlots = maxSlots;
        }

        public bool Contains(IPartition<TNode> partition)
        {
            throw new NotImplementedException();
        }

        public List<IPartition<TNode>> FindPartitions(string nodeId)
        {
            throw new NotImplementedException();
        }

        public List<ShardingRange<TNode>> FindRanges(string nodeId)
        {
            throw new NotImplementedException();
        }

        public List<ShardingRange<TNode>> GetAllRanges()
        {
            throw new NotImplementedException();
        }

        public IPartition<TNode> PrevPartition(long slot)
        {
            throw new NotImplementedException();
        }

        public IPartition<TNode> NextPartition(long slot)
        {
            throw new NotImplementedException();
        }

        public List<IPartition<TNode>> GetAllPartitions()
        {
            throw new NotImplementedException();
        }

        public IPartition<TNode> Locate(string key)
        {
            throw new NotImplementedException();
        }

        public List<IPartition<TNode>> Locate(string key, int count)
        {
            throw new NotImplementedException();
        }

        public int PartitionSize()
        {
            throw new NotImplementedException();
        }

        public bool Add(IPartition<TNode> partition)
        {
            throw new NotImplementedException();
        }

        public bool Update(IPartition<TNode> partition)
        {
            throw new NotImplementedException();
        }

        public void Save(IPartition<TNode> partition)
        {
            throw new NotImplementedException();
        }

        public List<IPartition<TNode>> AddAll(IList<IPartition<TNode>> partitions)
        {
            throw new NotImplementedException();
        }

        public IPartition<TNode> Remove(long slot)
        {
            throw new NotImplementedException();
        }

        public bool Remove(IPartition<TNode> partition)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public IEventBox<ShardingOnChange<TNode>> ChangeEvent { get; }

        public IEventBox<ShardingOnRemove<TNode>> RemoveEvent { get; }
    }

}
