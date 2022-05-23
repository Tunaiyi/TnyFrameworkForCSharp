using System;

namespace TnyFramework.Namespace.Sharding
{

    public class PartitionSlot<TNode> : ShardingPartition<TNode>
        where TNode : IShardingNode
    {
        private int Seed { get; set; }

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

}
