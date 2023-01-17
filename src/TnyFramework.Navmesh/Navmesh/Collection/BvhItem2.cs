// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

namespace TnyFramework.Navmesh.Navmesh.Collection
{

    public interface IBvhItem2
    {
        QuotaVector2 Min { get; }

        QuotaVector2 Max { get; }

        int Index { get; }
    }

    public struct BvhItem2<T> : IBvhItem2
    {
        public QuotaVector2 Min { get; }

        public QuotaVector2 Max { get; }

        public int Index { get; }

        public T Value { get; }

        public BvhItem2(QuotaVector2 min, QuotaVector2 max, int index, T value)
        {
            Min = min;
            Max = max;
            Index = index;
            Value = value;
        }
    }

}
