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

    public class BvhElementVisitor<TValue> : IBvhElementVisitor<IBvhElement<TValue>, TValue>
    {
        public FmVector2 GetMin(IBvhElement<TValue> value, int index) => value.BoundMin;

        public FmVector2 GetMax(IBvhElement<TValue> value, int index) => value.BoundMax;

        public TValue GetValue(IBvhElement<TValue> value, int index) => value.Value;
        
    }

}
