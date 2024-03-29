// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.Collections.Concurrent;

namespace TnyFramework.Common.Attribute
{

    public interface IAttrKey
    {
        string Name { get; }
    }

    public class AttrKeys
    {
        /// <summary>
        /// Key Map
        /// </summary>
        private static readonly ConcurrentDictionary<object, IAttrKey> KEY_MAP = new ConcurrentDictionary<object, IAttrKey>();

        /// <summary>
        /// 获取 key
        /// </summary>
        /// <param name="type">key 作用域类(重复定义)</param>
        /// <param name="key">key 值</param>
        /// <typeparam name="TValue">返回 AttrKey</typeparam>
        /// <returns></returns>
        public static AttrKey<TValue> Key<TValue>(Type type, string key)
        {
            return LoadOrCreate<TValue>($"{type.FullName}.{key}");
        }

        /// <summary>
        /// 获取 key
        /// </summary>
        /// <param name="key">key 值</param>
        /// <typeparam name="TValue">返回 AttrKey</typeparam>
        /// <returns></returns>
        public static AttrKey<TValue> Key<TValue>(string key)
        {
            return LoadOrCreate<TValue>($"{typeof(TValue).FullName}.{key}");
        }

        private static AttrKey<TValue> LoadOrCreate<TValue>(string key)
        {
            KEY_MAP.TryGetValue(key, out var attrKey);
            if (attrKey != null)
                return (AttrKey<TValue>) attrKey;
            attrKey = new AttrKey<TValue>(key);
            if (!KEY_MAP.TryAdd(key, attrKey))
            {
                attrKey = KEY_MAP[key];
            }
            return (AttrKey<TValue>) attrKey;
        }
    }

    public sealed class AttrKey<T> : IAttrKey
    {
        internal AttrKey(string name)
        {
            Name = name;
        }

        public string Name { get; }

        private bool Equals(IAttrKey other)
        {
            return Name == other.Name;
        }

        public override bool Equals(object? obj)
        {
            return ReferenceEquals(this, obj) || obj is AttrKey<T> other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }

}
