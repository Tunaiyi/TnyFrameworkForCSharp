// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.Text;
using TnyFramework.Codec;
using TnyFramework.Codec.Attributes;
using TnyFramework.Codec.Exceptions;
using TnyFramework.FreeRedis.Hosting.Attributes;

namespace TnyFramework.FreeRedis.Hosting
{

    public class FreeRedisCodecs
    {
        private readonly ObjectCodecAdapter objectCodecAdapter;

        private readonly MimeType? defaultMimeType;

        private readonly Encoding encoding;

        public FreeRedisCodecs(ObjectCodecAdapter objectCodecAdapter, MimeType? defaultMimeType = null, Encoding? encoding = null)
        {
            this.objectCodecAdapter = objectCodecAdapter;
            this.defaultMimeType = defaultMimeType;
            this.encoding = encoding ?? Encoding.UTF8;
        }

        private IObjectCodec Codec(Type type)
        {
            var scheme = RedisObjectScheme.SchemeOf(type);
            var mimeType = scheme != null ? scheme.Mime : defaultMimeType;
            if (mimeType == null)
            {
                throw new ObjectCodecException($"Type {type} 没有存在 {nameof(RedisObjectAttribute)} 或 {nameof(CodableAttribute)}");
            }
            return objectCodecAdapter.Codec(type, mimeType);

        }

        public object? Serialize(object? value)
        {
            return value == null ? null : Codec(value.GetType()).Encode(value);
        }

        public object? Deserialize(string? data, Type type)
        {
            return data == null ? null : Codec(type).Decode(encoding.GetBytes(data));
        }

        public object? DeserializeRaw(byte[]? data, Type type)
        {
            return data == null ? null : Codec(type).Decode(data);
        }
    }

}
