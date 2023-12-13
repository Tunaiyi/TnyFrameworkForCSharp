// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.Collections.Concurrent;
using System.Reflection;
using TnyFramework.Codec;
using TnyFramework.Codec.Attributes;
using TnyFramework.Codec.Exceptions;
using TnyFramework.Common.Extensions;
using TnyFramework.FreeRedis.NetCore.Attributes;

namespace TnyFramework.FreeRedis.NetCore
{

    public class RedisObjectScheme
    {
        private static readonly ConcurrentDictionary<Type, RedisObjectScheme> SCHEME_MAP = new ConcurrentDictionary<Type, RedisObjectScheme>();

        private static readonly RedisObjectScheme EMPTY = new RedisObjectScheme(null!, null!);

        public string Source { get; }

        public Type Type { get; }

        public IMimeType Mime { get; }

        public static RedisObjectScheme? SchemeOf<TValue>(bool throwOnNull = false)
        {
            return SchemeOf(typeof(TValue), throwOnNull);
        }

        private static RedisObjectScheme CreateScheme(Type current)
        {
            var redisObject = current.GetCustomAttribute<RedisObjectAttribute>();
            if (redisObject != null)
            {
                var mimeType = MimeTypeOf(redisObject.Mime);
                if (mimeType != null)
                {
                    return new RedisObjectScheme(current, mimeType, redisObject.Source);
                }
            }
            var codable = current.GetCustomAttribute<CodableAttribute>();
            if (codable != null)
            {
                var mimeType = MimeTypeOf(codable.Mime);
                if (mimeType != null)
                {
                    return new RedisObjectScheme(current, mimeType);
                }
            }
            return EMPTY;
        }

        public static RedisObjectScheme? SchemeOf(Type type, bool throwOnNull = false)
        {
            var result = SCHEME_MAP.Get(type);
            if (result != null)
            {
                return result;
            }
            var value = SCHEME_MAP.GetOrAdd(type, CreateScheme);
            if (!ReferenceEquals(value, EMPTY))
                return value;
            if (throwOnNull)
            {
                throw new ObjectCodecException($"Type {type} 没有存在 {nameof(RedisObjectAttribute)} 或 {nameof(CodableAttribute)}");
            }
            return null;
        }

        private static IMimeType? MimeTypeOf(string value)
        {
            return !value.IsNotBlank() ? null : MimeType.ForMimeType(value);
        }

        private RedisObjectScheme(Type type, IMimeType mime, string source = "")
        {
            Type = type;
            Mime = mime;
            Source = source;
        }
    }

}
