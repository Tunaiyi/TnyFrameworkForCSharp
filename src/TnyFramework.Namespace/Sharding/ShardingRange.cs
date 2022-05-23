using System.Collections.Generic;
using System.Text;

namespace TnyFramework.Namespace.Sharding
{

    public class ShardingRange
    {
        private List<Range> ranges;

        public long FromSlot { get; }

        public long ToSlot { get; }

        public long MaxSlot { get; }

        public bool Across { get; }

        public ShardingRange(long fromSlot, long toSlot, long maxSlot)
        {
            if (fromSlot > maxSlot)
            {
                fromSlot = 0;
            }
            MaxSlot = maxSlot;
            FromSlot = fromSlot;
            ToSlot = toSlot;
            if (fromSlot > toSlot)
            {
                Across = true;
            }
        }

        public IReadOnlyList<Range> GetRanges()
        {
            if (ranges != null)
            {
                return ranges;
            }
            if (Across)
            {
                ranges = new List<Range> {
                    new Range(0L, this.ToSlot),
                    new Range(FromSlot, this.MaxSlot)
                };
            } else
            {
                ranges = new List<Range> {
                    new Range(FromSlot, this.ToSlot)
                };
            }
            return ranges;
        }
    }

    public class ShardingRange<TNode> : ShardingRange
        where TNode : IShardingNode
    {
        public IPartition<TNode> Partition { get; }

        public ShardingRange(IPartition<TNode> partition, long slot, long maxSlot)
            : this(slot, slot, partition, maxSlot)
        {

        }

        public ShardingRange(long fromSlot, long toSlot, IPartition<TNode> partition, long maxSlot) : base(fromSlot, toSlot, maxSlot)
        {
            Partition = partition;
        }

        public override string ToString()
        {
            var buffer = new StringBuilder();
            foreach (var r in GetRanges())
            {
                buffer.Append('[').Append(r.Min).Append('-').Append(r.Max).Append(']');
            }
            return $"{Partition}{{{FromSlot} to {ToSlot}}}{buffer}";
        }
    }

}
