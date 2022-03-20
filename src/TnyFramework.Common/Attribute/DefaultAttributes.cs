using System;
using System.Collections.Generic;
using System.Collections.Immutable;
namespace TnyFramework.Common.Attribute
{
    public class DefaultAttributes : IAttributes
    {
        // 属性 map
        private readonly Dictionary<IAttrKey, object> attributeMap = new Dictionary<IAttrKey, object>();


        public T Get<T>(AttrKey<T> key)
        {
            lock (this)
            {
                if (!attributeMap.TryGetValue(key, out var value))
                    return default;
                return (T)value;
            }
        }


        public T Get<T>(AttrKey<T> key, T defaultValue)
        {
            CheckNotNull(key, "Get Attributes key is null");
            lock (this)
            {
                if (!attributeMap.TryGetValue(key, out var value))
                    return defaultValue;
                return (T)value;
            }
        }


        public bool TryAdd<T>(AttrKey<T> key, T value)
        {
            CheckNotNull(key, "TryAdd Attributes key is null");
            CheckNotNull(value, "TryAdd Attributes value is null");
            lock (this)
            {
                if (attributeMap.ContainsKey(key))
                    return false;
                attributeMap.Add(key, value);
                return true;
            }
        }


        public bool TryAdd<T>(AttrKey<T> key, Func<T> supplier)
        {
            CheckNotNull(key, "TryAdd Attributes key is null");
            CheckNotNull(supplier, "TryAdd Attributes supplier is null");
            lock (this)
            {
                if (attributeMap.ContainsKey(key))
                    return false;
                var value = supplier.Invoke();
                CheckNotNull(value, "TryAdd Attributes supplier return value is null");
                attributeMap[key] = value;
                return true;
            }
        }


        public T Remove<T>(AttrKey<T> key)
        {
            CheckNotNull(key, "Remove Attributes key is null");
            lock (this)
            {
                if (!attributeMap.TryGetValue(key, out var exist))
                    return default;
                attributeMap.Remove(key);
                return (T)exist;
            }
        }


        public void Set<T>(AttrKey<T> key, T value)
        {
            CheckNotNull(key, "Set Attributes key is null");
            CheckNotNull(value, "Set Attributes supplier is null");
            lock (this)
            {
                attributeMap.Add(key, value);
            }
        }


        public void Set<T>(AttrPair<T> pair)
        {
            Set(pair.Key, pair.Value);
        }


        public void SetAll(ICollection<IAttrPair> pairs)
        {
            foreach (var pair in pairs)
            {
                lock (this)
                {
                    attributeMap.Add(pair.Key, pair.Value);
                }
            }
        }


        public void SetAll(params IAttrPair[] pairs)
        {
            foreach (var pair in pairs)
            {
                lock (this)
                {
                    attributeMap.Add(pair.Key, pair.Value);
                }
            }
        }


        public void RemoveAll(ICollection<IAttrKey> keys)
        {
            lock (this)
            {
                foreach (var key in keys)
                {
                    lock (this)
                    {
                        attributeMap.Remove(key);
                    }
                }
            }
        }


        public IDictionary<IAttrKey, object> AttributeMap()
        {
            lock (this)
            {
                return attributeMap.ToImmutableDictionary();
            }
        }


        public void Clear()
        {
            lock (this)
            {
                attributeMap.Clear();
            }
        }


        public bool Empty {
            get {
                lock (this)
                {
                    return attributeMap.Count == 0;
                }
            }
        }


        private static void CheckNotNull(object value, string massage)
        {
            if (value == null)
            {
                throw new NullReferenceException(massage);
            }
        }
    }
}
