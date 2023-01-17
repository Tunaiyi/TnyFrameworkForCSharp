// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

namespace TnyFramework.Navmesh.Navmesh.Collection
{

    public struct BvhNode2<TValue>
    {
        private readonly QuotaVector2 min;

        private readonly QuotaVector2 max;

        public IQuotaVector2 Min => min;

        public IQuotaVector2 Max => max;

        public TValue Value { get; }

        public int Next { get; }

        public static BvhNode2<TValue> NoneOf()
        {
            return new BvhNode2<TValue>(new QuotaVector2(0), new QuotaVector2(0), -2);
        }

        public BvhNode2(BvhItem2<TValue> item)
        {
            min = item.Min;
            max = item.Max;
            Value = item.Value;
            Next = -1;
        }

        public BvhNode2(QuotaVector2 min, QuotaVector2 max, int next)
        {
            this.min = min;
            this.max = max;
            Value = default;
            Next = next;
        }

        public bool Leaf => Next == -1;

        public bool Node => Next >= 0;

        public bool None => Next <= -2;
    }

}
