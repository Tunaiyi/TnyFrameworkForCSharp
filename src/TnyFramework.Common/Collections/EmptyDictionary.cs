// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.Collections;
using System.Collections.Generic;

namespace TnyFramework.Common.Collections
{

    public class EmptyDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        public int Count => 0;

        public bool IsReadOnly => true;

        public bool ContainsKey(TKey key) => false;

        public bool Contains(KeyValuePair<TKey, TValue> item) => false;

        public bool TryGetValue(TKey key, out TValue value)
        {
            value = default!;
            return false;
        }

        public TValue this[TKey key] {
            get => default!;
            set => throw new NotImplementedException();
        }

        public ICollection<TKey> Keys => Array.Empty<TKey>();

        public ICollection<TValue> Values => Array.Empty<TValue>();

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return new EmptyEnumerator<KeyValuePair<TKey, TValue>>();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            throw new NotImplementedException();
        }

        public void Add(TKey key, TValue value)
        {
            throw new NotImplementedException();
        }

        public bool Remove(TKey key)
        {
            throw new NotImplementedException();
        }
    }

}
