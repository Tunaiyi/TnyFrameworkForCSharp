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

namespace TnyFramework.Net.Message
{

    public class MessageParamList : IList
    {
        private readonly IList paramList;

        public MessageParamList()
        {
            paramList = new List<object>();
        }

        public MessageParamList(IList paramList)
        {
            this.paramList = paramList;
        }

        public IEnumerator GetEnumerator()
        {
            return paramList.GetEnumerator();
        }

        public void CopyTo(Array array, int index)
        {
            paramList.CopyTo(array, index);
        }

        public int Count => paramList.Count;

        public bool IsSynchronized => paramList.IsSynchronized;

        public object SyncRoot => paramList.SyncRoot;

        public int Add(object? value)
        {
            return paramList.Add(value);
        }

        public void Clear()
        {
            paramList.Clear();
        }

        public bool Contains(object? value)
        {
            return paramList.Contains(value);
        }

        public int IndexOf(object? value)
        {
            return paramList.IndexOf(value);
        }

        public void Insert(int index, object? value)
        {
            paramList.Insert(index, value);
        }

        public void Remove(object? value)
        {
            paramList.Remove(value);
        }

        public void RemoveAt(int index)
        {
            paramList.RemoveAt(index);
        }

        public bool IsFixedSize => paramList.IsFixedSize;

        public bool IsReadOnly => paramList.IsReadOnly;

        public object? this[int index] {
            get => paramList[index];
            set => paramList[index] = value;
        }

        /**
         * 如果 没有则返回 null
         *
         * @param index 查找的 index
         * @return 返回查找的值
         */
        public bool Get<T>(int index, out T value)
        {
            return Get(index, default, out value!);
        }

        public bool Get<T>(int index, T defaultValue, out T value)
        {
            var param = paramList[index];
            if (param == null)
            {
                value = defaultValue;
                return false;
            }
            value = (T) param;
            return true;
        }

        public override string ToString()
        {
            var values = new object?[paramList.Count];
            for (var i = 0; i < values.Length; i++)
            {
                values[i] = paramList[i];
            }
            return $"[{string.Join(", ", values)}]";
        }
    }

}
