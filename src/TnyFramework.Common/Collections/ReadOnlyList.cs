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

    public class ReadOnlyList<TValue> : IList<TValue>
    {
        private readonly IList<TValue> list;

        public ReadOnlyList(IList<TValue> list)
        {
            this.list = list;
        }

        public IEnumerator<TValue> GetEnumerator()
        {
            return list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool Contains(TValue item)
        {
            return list.Contains(item);
        }

        public void CopyTo(TValue[] array, int arrayIndex)
        {
            list.CopyTo(array, arrayIndex);
        }

        public int Count => list.Count;

        public bool IsReadOnly => true;

        public int IndexOf(TValue item)
        {
            return list.IndexOf(item);
        }

        public TValue this[int index] {
            get => list[index];
            set => throw new NotImplementedException();
        }

        public void Add(TValue item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Remove(TValue item)
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, TValue item)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }
    }

}
