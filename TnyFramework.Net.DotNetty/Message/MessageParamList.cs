using System;
using System.Collections;
using System.Collections.Generic;

namespace TnyFramework.Net.DotNetty.Message
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


        public int Add(object value)
        {
            return paramList.Add(value);
        }


        public void Clear()
        {
            paramList.Clear();
        }


        public bool Contains(object value)
        {
            return paramList.Contains(value);
        }


        public int IndexOf(object value)
        {
            return paramList.IndexOf(value);
        }


        public void Insert(int index, object value)
        {
            paramList.Insert(index, value);
        }


        public void Remove(object value)
        {
            paramList.Remove(value);
        }


        public void RemoveAt(int index)
        {
            paramList.RemoveAt(index);
        }


        public bool IsFixedSize => paramList.IsFixedSize;

        public bool IsReadOnly => paramList.IsReadOnly;

        public object this[int index] {
            get => paramList[index];
            set => paramList[index] = value;
        }
    }
}
