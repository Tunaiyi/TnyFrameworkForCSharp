// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System.Text;

namespace TnyFramework.Namespace.Sharding
{

    public class HashingOptions<TNode>
        where TNode : IShardingNode
    {
        public string Name { get; set; }

        public long Ttl { get; set; } = 3000L;

        public int PartitionCount { get; set; } = 5;

        public long MaxSlots { get; set; } = 1024L;

        public Encoding Encoding { get; set; } = Encoding.UTF8;

        public IHasher<string> KeyHasher { get; set; }

        public IHasher<PartitionSlot<TNode>> NodeHasher { get; set; }

        public HashingOptions()
        {
        }
    }

}
