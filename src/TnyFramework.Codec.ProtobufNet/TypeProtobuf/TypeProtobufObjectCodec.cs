// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.IO;
using ProtoBuf;
using TnyFramework.Codec.Exceptions;
using TnyFramework.Common.Binary.Extensions;
using TnyFramework.Common.Extensions;

namespace TnyFramework.Codec.ProtobufNet.TypeProtobuf
{

    public class TypeProtobufObjectCodec<T> : BytesObjectCodec<T>
    {
        private readonly TypeProtobufSchemeFactory factory;

        public TypeProtobufObjectCodec()
        {
            factory = TypeProtobufSchemeFactory.Factory;
        }

        public TypeProtobufObjectCodec(TypeProtobufSchemeFactory factory)
        {
            this.factory = factory;
        }

        public override byte[] Encode(T? value)
        {
            if (value == null)
            {
                return Array.Empty<byte>();
            }
            var scheme = factory.Load(value.GetType());
            if (scheme.IsNull())
            {
                return Array.Empty<byte>();
            }
            var stream = new MemoryStream(256);
            stream.WriteFixed32(scheme.Id);
            Serializer.Serialize(stream, value);
            return stream.ToArray();
        }

        public override void Encode(T? value, Stream output)
        {
            if (value == null)
            {
                return;
            }
            var scheme = factory.Load(value.GetType());
            if (scheme.IsNull())
                return;
            output.WriteFixed32(scheme.Id);
            Serializer.Serialize(output, value);
        }

        public override T? Decode(byte[]? bytes)
        {
            if (bytes == null || bytes.Length == 0)
            {
                return default;
            }
            var memory = new MemoryStream(bytes);
            memory.ReadFixed32(out int id);
            if (factory.Get(id, out var scheme))
            {
                return (T) Serializer.Deserialize(scheme.Type, memory);
            }
            throw new ObjectCodecException($"Unknown ${id} TypeProtobufScheme");
        }

        public override T? Decode(Stream input)
        {
            if (input.IsNull() || input.Length == 0)
            {
                return default;
            }
            input.ReadFixed32(out int id);
            if (factory.Get(id, out var scheme))
            {
                return (T) Serializer.Deserialize(scheme.Type, input);
            }
            throw new ObjectCodecException($"Unknown ${id} TypeProtobufScheme");
        }
    }

}
