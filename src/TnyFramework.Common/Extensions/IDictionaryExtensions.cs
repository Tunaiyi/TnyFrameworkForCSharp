using System.Collections.Generic;

namespace TnyFramework.Common.Extensions
{

    public static class DictionaryExtensions
    {
        public static TValue Get<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            dictionary.TryGetValue(key, out var value);
            return value;
        }
    }

}