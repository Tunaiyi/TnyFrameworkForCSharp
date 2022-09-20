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

        public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue)
        {
            dictionary.TryGetValue(key, out var value);
            return value ?? defaultValue;
        }

        public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TKey, TValue> defaultValue)
        {
            dictionary.TryGetValue(key, out var value);
            return value ?? defaultValue(key);
        }

        public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TValue> defaultValue)
        {
            dictionary.TryGetValue(key, out var value);
            return value ?? defaultValue();
        }

        public static TValue PutIfAbsent<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
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

        public static TValue ComputeIfAbsent<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TKey, TValue> func)
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
