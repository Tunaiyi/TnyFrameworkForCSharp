// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using GF.Base;

namespace TnyFramework.Navmesh.Navmesh.Collection
{

    public class BvhElement<T> : IBvhElement<T>
    {
        public FmVector2 BoundMin { get; }

        public FmVector2 BoundMax { get; }

        public T Value { get; }

        public BvhElement(FmVector2 boundMin, FmVector2 boundMax, T value)
        {
            BoundMin = boundMin;
            BoundMax = boundMax;
            Value = value;
        }

        public BvhElement(Fix64 minX, Fix64 minY, Fix64 maxX, Fix64 maxY, T value)
        {
            BoundMin = FmVector2.Create(minX, minY);
            BoundMax = FmVector2.Create(maxX, maxY);
            Value = value;
        }
    }

}
