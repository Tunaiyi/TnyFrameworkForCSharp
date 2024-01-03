// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.Collections.Generic;
using System.Threading;
using TnyFramework.Common.Collections;

namespace TnyFramework.Common.Attribute
{

    public class DefaultAttributes : IAttributes
    {
        private static readonly IDictionary<IAttrKey, object> EMPTY = new EmptyDictionary<IAttrKey, object>();

        // 属性 map
        private volatile Dictionary<IAttrKey, object>? attributeMap;

        private IDictionary<IAttrKey, object> GetAttribute(out bool initialized)
        {
            var current = attributeMap;
            initialized = current != null;
            return current ?? EMPTY;
        }

        private Dictionary<IAttrKey, object> LoadAttribute()
        {
            var instance = attributeMap;
            if (instance != null)
            {
                return instance;
            }
            instance = new Dictionary<IAttrKey, object>();
            var value = Interlocked.CompareExchange(ref attributeMap, instance, null);
            if (value != null)
            {
                instance = value;
            }
            return instance;
        }

        public T Get<T>(AttrKey<T> key)
        {
            var attribute = GetAttribute(out var initialized);
            if (!initialized)
            {
                return default!;
            }
            lock (attribute)
            {
                if (!attribute.TryGetValue(key, out var value))
                    return default!;
                return (T) value;
            }
        }

        public T Get<T>(AttrKey<T> key, T defaultValue)
        {
            CheckNotNull(key, "Get Attributes key is null");
            var attribute = GetAttribute(out var initialized);
            if (!initialized)
            {
                return defaultValue;
            }
            lock (attribute)
            {
                if (!attribute.TryGetValue(key, out var value))
                    return defaultValue;
                return (T) value;
            }
        }

        public bool TryAdd<T>(AttrKey<T> key, T value)
        {
            CheckNotNull(key, "TryAdd Attributes key is null");
            CheckNotNull(value!, "TryAdd Attributes value is null");
            var attribute = LoadAttribute();
            lock (attribute)
            {
                if (attribute.ContainsKey(key))
                    return false;
                attribute.Add(key, value!);
                return true;
            }
        }

        public bool TryAdd<T>(AttrKey<T> key, Func<T> supplier)
        {
            CheckNotNull(key, "TryAdd Attributes key is null");
            CheckNotNull(supplier, "TryAdd Attributes supplier is null");
            var attribute = LoadAttribute();
            lock (attribute)
            {
                if (attribute.ContainsKey(key))
                    return false;
                var value = supplier.Invoke();
                CheckNotNull(value, "TryAdd Attributes supplier return value is null");
                attribute[key] = value!;
                return true;
            }
        }

        public T Load<T>(AttrKey<T> key, Func<T> supplier)
        {
            CheckNotNull(key, "TryAdd Attributes key is null");
            CheckNotNull(supplier, "TryAdd Attributes supplier is null");
            var attribute = LoadAttribute();
            lock (attribute)
            {
                if (attribute.TryGetValue(key, out var exist))
                    return (T) exist;
                var value = supplier.Invoke();
                CheckNotNull(value, "TryAdd Attributes supplier return value is null");
                attribute[key] = value!;
                return value;
            }
        }

        public T Remove<T>(AttrKey<T> key)
        {
            CheckNotNull(key, "Remove Attributes key is null");
            var attribute = GetAttribute(out var initialized);
            if (!initialized)
            {
                return default!;
            }
            lock (attribute)
            {
                if (!attribute.TryGetValue(key, out var exist))
                    return default!;
                attribute.Remove(key);
                return (T) exist;
            }
        }

        public void Set<T>(AttrKey<T> key, T value)
        {
            CheckNotNull(key, "Set Attributes key is null");
            CheckNotNull(value, "Set Attributes supplier is null");
            var attribute = LoadAttribute();
            lock (attribute)
            {
                attribute[key] = value!;
            }
        }

        public void Set<T>(IAttrPair<T> pair)
        {
            Set(pair.Key, pair.Value);
        }

        public void SetAll(ICollection<IAttrPair> pairs)
        {
            var attribute = LoadAttribute();
            lock (attribute)
            {
                foreach (var pair in pairs)
                {
                    attribute[pair.Key] = pair.Value;
                }
            }
        }

        public void SetAll(params IAttrPair[] pairs)
        {
            var attribute = LoadAttribute();
            lock (attribute)
            {
                foreach (var pair in pairs)
                {
                    attribute[pair.Key] = pair.Value;
                }
            }
        }

        public void RemoveAll(ICollection<IAttrKey> keys)
        {
            var attribute = GetAttribute(out var initialized);
            if (!initialized)
            {
                return;
            }
            lock (attribute)
            {
                foreach (var key in keys)
                {
                    attribute.Remove(key);
                }
            }
        }

        public IDictionary<IAttrKey, object> AttributeMap()
        {
            return GetAttribute(out _);
        }

        public void Clear()
        {
            var attribute = GetAttribute(out var initialized);
            if (!initialized)
            {
                return;
            }
            lock (this)
            {
                attribute.Clear();
            }
        }

        public bool Empty => GetAttribute(out _).Count == 0;

        private static void CheckNotNull(object? value, string massage)
        {
            if (value == null)
            {
                throw new NullReferenceException(massage);
            }
        }
    }

}
