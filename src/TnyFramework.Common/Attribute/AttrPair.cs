// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

namespace TnyFramework.Common.Attribute
{

    public interface IAttrPair
    {
        IAttrKey Key { get; }

        object Value { get; }
    }

    public interface IAttrPair<T>
    {
        AttrKey<T> Key { get; }

        T Value { get; }
    }

    public sealed class AttrPair<T> : IAttrPair, IAttrPair<T>
    {
        public AttrPair<T> Entry(ref AttrKey<T> key, T value)
        {
            return new AttrPair<T>(ref key, value);
        }

        private AttrPair(ref AttrKey<T> key, T value)
        {
            Key = key;
            Value = value;
        }

        public AttrKey<T> Key { get; }

        public T Value { get; }

        IAttrKey IAttrPair.Key => Key;

        object IAttrPair.Value => Value!;
    }

}
