// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.IO;
using DotNetty.Buffers;
using DotNetty.Common;
using TnyFramework.Codec.ProtobufNet.TypeProtobuf;
using TnyFramework.Net.DotNetty.Common;
using TnyFramework.Net.Message;

namespace TnyFramework.Net.DotNetty.Codec
{

    public class MessageHeaderCodec : IMessageHeaderCodec
    {
        private readonly TypeProtobufObjectCodecFactory codecFactory = new();

        private static readonly FastThreadLocal<MemoryStream> STEAM_LOCAL = new();

        static MessageHeaderCodec()
        {
            var factory = TypeProtobufSchemeFactory.Factory;
            factory.Load<RpcForwardHeader>();
            factory.Load<RpcTracingHeader>();
            factory.Load<RpcOriginalMessageIdHeader>();
        }

        public MessageHeader? Decode(IByteBuffer buffer)
        {
            var codec = codecFactory.CreateCodec(typeof(object));
            buffer.ReadVariant(out int bodyLength);
            var body = buffer.ReadBytes(bodyLength);
            using var stream = new ReadOnlyByteBufferStream(body, true);
            return (MessageHeader?) codec.Decode(stream);
        }

        public void Encode(MessageHeader? body, IByteBuffer buffer)
        {
            var type = body?.GetType();
            var codec = type == null ? null : codecFactory.CreateCodec(type);
            if (codec == null)
                throw new Exception($"不存在该DTO:{type}");
            var stream = Stream();
            try
            {
                codec.Encode(body, stream);
                if (stream.Position <= 0)
                    return;
                ByteBufferVariantExtensions.WriteVariant(stream.Position, buffer);
                buffer.WriteBytes(stream.GetBuffer(), 0, (int) stream.Position);
            } finally
            {
                stream.Seek(0, SeekOrigin.Begin);
            }
        }

        public void Encode(object? body, IByteBuffer buffer)
        {
            if (body is MessageHeader header)
                Encode(header, buffer);
        }

        object? INetContentCodec.Decode(IByteBuffer buffer)
        {
            return Decode(buffer);
        }

        protected static MemoryStream Stream(long length = -1)
        {
            var stream = STEAM_LOCAL.Value;
            if (stream == null)
            {
                stream = new MemoryStream();
                STEAM_LOCAL.Value = stream;
            } else
            {
                if (stream.Position != 0)
                {
                    stream.Seek(0, SeekOrigin.Begin);
                }
            }
            if (length > 0 && stream.Length < length)
            {
                stream.SetLength(length);
            }
            return stream;
        }
    }

}
