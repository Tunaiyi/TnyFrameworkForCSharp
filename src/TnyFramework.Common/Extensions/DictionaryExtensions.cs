// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace TnyFramework.Common.Extensions
{

    public static class DictionaryExtensions
    {
        public static bool IsEmpty<TKey, TValue>(this IDictionary<TKey, TValue> dictionary)
        {
            return dictionary == null || dictionary.Count == 0;
        }

        public static TValue Get<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            dictionary.TryGetValue(key, out var value);
            return value;
        }

        public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key,
            TValue defaultValue)
        {
            dictionary.TryGetValue(key, out var value);
            return value ?? defaultValue;
        }

        public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key,
            Func<TKey, TValue> defaultValue)
        {
            dictionary.TryGetValue(key, out var value);
            return value ?? defaultValue(key);
        }

        public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key,
            Func<TValue> defaultValue)
        {
            dictionary.TryGetValue(key, out var value);
            return value ?? defaultValue();
        }

        public static void RemoveRange<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, IEnumerable<TKey> keys)
        {
            foreach (var key in keys)
            {
                dictionary.Remove(key);
            }
        }

        public static TValue Put<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            if (dictionary is ConcurrentDictionary<TKey, TValue> conDic)
            {
                return conDic.AddOrUpdate(key, value, (k, v) => value);
            }
            if (dictionary.ContainsKey(key))
            {
                var exist = dictionary[key];
                dictionary[key] = value;
                return exist;
            }
            dictionary.Add(key, value);
            return default;
        }

        public static TValue PutIfAbsent<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key,
            TValue value)
        {
            if (dictionary is ConcurrentDictionary<TKey, TValue> conDic)
            {
                return conDic.TryAdd(key, value) ? default : dictionary.Get(key);
            }
            if (dictionary.ContainsKey(key))
            {
                return dictionary[key];
            }
            dictionary.Add(key, value);
            return default;
        }

        public static TValue ComputeIfAbsent<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key,
            Func<TKey, TValue> func)
        {
            if (dictionary is ConcurrentDictionary<TKey, TValue> conDic)
            {
                return conDic.GetOrAdd(key, func);
            }
            if (dictionary.ContainsKey(key))
            {
                return dictionary[key];
            }
            var value = func(key);
            dictionary.Add(key, value);
            return value;
        }
    }

}
