// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.Collections.Generic;

namespace TnyFramework.Common.Attribute
{

    public class EmptyAttributes : IAttributes
    {
        public static IAttributes GetEmpty() => new EmptyAttributes();

        private EmptyAttributes()
        {

        }

        public T Get<T>(AttrKey<T> key)
        {
            return default!;
        }

        public T Get<T>(AttrKey<T> key, T defaultValue)
        {
            return default!;
        }

        public bool TryAdd<T>(AttrKey<T> key, T value)
        {
            throw new NotImplementedException();
        }

        public bool TryAdd<T>(AttrKey<T> key, Func<T> supplier)
        {
            throw new NotImplementedException();
        }

        public T Load<T>(AttrKey<T> key, Func<T> supplier)
        {
            throw new NotImplementedException();
        }

        public T Remove<T>(AttrKey<T> key)
        {
            throw new NotImplementedException();
        }

        public void Set<T>(AttrKey<T> key, T value)
        {
            throw new NotImplementedException();
        }

        public void Set<T>(IAttrPair<T> pair)
        {
            throw new NotImplementedException();
        }

        public void SetAll(ICollection<IAttrPair> pairs)
        {
            throw new NotImplementedException();
        }

        public void SetAll(params IAttrPair[] pairs)
        {
            throw new NotImplementedException();
        }

        public void RemoveAll(ICollection<IAttrKey> keys)
        {
            throw new NotImplementedException();
        }

        public IDictionary<IAttrKey, object> AttributeMap()
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Empty => true;
    }

}
