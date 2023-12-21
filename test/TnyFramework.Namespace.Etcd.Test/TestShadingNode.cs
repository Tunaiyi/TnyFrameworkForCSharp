// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using TnyFramework.Namespace.Sharding;

namespace TnyFramework.Namespace.Etcd.Test
{

    public class TestShadingNode : IShardingNode
    {
        public string Key { get; set; } = null!;

        public TestShadingNode()
        {
        }

        public TestShadingNode(string key)
        {
            Key = key;
        }

        protected bool Equals(TestShadingNode other)
        {
            return Key == other.Key;
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((TestShadingNode) obj);
        }

        public override int GetHashCode()
        {
            return (Key != null ? Key.GetHashCode() : 0);
        }

        public override string ToString()
        {
            return $"{nameof(Key)}: {Key}";
        }
    }

}
