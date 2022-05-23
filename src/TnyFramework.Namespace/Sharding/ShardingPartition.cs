using System;

namespace TnyFramework.Namespace.Sharding
{

    public abstract class ShardingPartition<TNode> : IPartition<TNode>
        where TNode : IShardingNode
    {
        public string Key { get; }

        public string NodeKey => Node?.Key;

        public int Index { get; }

        public TNode Node { get; }

        public long Slot { get; protected set; }

        protected ShardingPartition(int index, TNode node)
        {
            Key = node.Key + "$" + index;
            Index = index;
            Node = node;
        }

        protected ShardingPartition(int index, long slot, TNode node)
        {
            Key = node.Key + "$" + index;
            Index = index;
            Slot = slot;
            Node = node;
        }

        public IShardingNode GetNode()
        {
            return Node;
        }

        public TValue GetNode<TValue>() where TValue : IShardingNode
        {
            if (Node is TValue value)
                return value;
            throw new InvalidCastException();
        }

        public abstract void Hash(IHasher<PartitionSlot<TNode>> hasher, long maxSlots);

        public override string ToString()
        {
            return $"{nameof(Key)}: {Key}, {nameof(Index)}: {Index}, {nameof(Slot)}: {Slot}";
        }

        private bool Equals(IPartition other)
        {
            return Key == other.Key && Index == other.Index;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((ShardingPartition<TNode>) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Key != null ? Key.GetHashCode() : 0) * 397) ^ Index;
            }
        }
    }

}
