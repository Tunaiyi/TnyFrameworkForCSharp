// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System.Collections.Generic;
using System.Text;

namespace TnyFramework.Namespace.Sharding
{

    public class ShardingRange
    {
        private List<SlotRange> ranges;

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

        public IReadOnlyList<SlotRange> GetRanges()
        {
            if (ranges != null)
            {
                return ranges;
            }
            if (Across)
            {
                ranges = new List<SlotRange> {
                    new SlotRange(0L, this.ToSlot),
                    new SlotRange(FromSlot, this.MaxSlot)
                };
            } else
            {
                ranges = new List<SlotRange> {
                    new SlotRange(FromSlot, this.ToSlot)
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
