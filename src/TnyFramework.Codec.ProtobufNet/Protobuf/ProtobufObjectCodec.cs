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
#if NET
using System.Buffers;
#endif

namespace TnyFramework.Codec.ProtobufNet.Protobuf
{

    public class ProtobufObjectCodec<T> : BytesObjectCodec<T>
    {
        public override byte[] Encode(T? value)
        {
            var stream = new MemoryStream();
            Serializer.Serialize(stream, value);
            return stream.ToArray();
        }

        public override void Encode(T? value, Stream output)
        {
            var stream = new MemoryStream();
            Serializer.Serialize(stream, value);
        }

        public override T Decode(byte[]? bytes)
        {
            var memory = new ReadOnlyMemory<byte>(bytes);
            return Serializer.Deserialize<T>(memory);
        }

        public override T Decode(Stream input)
        {
            return Serializer.Deserialize<T>(input);
        }

#if NET
        public override void Encode(T? value, IBufferWriter<byte> output)
        {
            Serializer.Serialize(output, value);
        }

        public override T? Decode(ReadOnlySequence<byte> input)
        {
            return Serializer.Deserialize<T>(input);
        }
#endif
    }

}
