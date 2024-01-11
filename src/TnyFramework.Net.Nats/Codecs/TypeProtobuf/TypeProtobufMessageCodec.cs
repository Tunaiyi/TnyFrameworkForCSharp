// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System.Buffers;
using ProtoBuf;
using TnyFramework.Common.Binary.Extensions;
using TnyFramework.Net.Message;
using TnyFramework.Net.Nats.Codecs.TypeProtobuf.Proto;
using TnyFramework.Net.Nats.Message;
using TnyFramework.Net.Nats.Transports;

namespace TnyFramework.Net.Nats.Codecs.TypeProtobuf
{

    public class TypeProtobufMessageCodec : IMessageCodec
    {
        private readonly TypeProtobufMessageBodyCodec bodyCodec;

        public TypeProtobufMessageCodec()
        {
            bodyCodec = new TypeProtobufMessageBodyCodec();
        }

        public TypeProtobufMessageCodec(TypeProtobufMessageBodyCodec bodyCodec)
        {
            this.bodyCodec = bodyCodec;
        }

        public void Encode(INetMessage message, IBufferWriter<byte> buffer)
        {
            var lengthSpan = buffer.GetSpan(sizeof(int));
            buffer.Advance(sizeof(int));
            var header = MessageHeadProto.Get();
            header.Copy(message);
            var headWriter = buffer.AsByteBufferWriter();
            try
            {
                Serializer.Serialize(headWriter, header);
                lengthSpan.WriteInt32LittleEndian(headWriter.Length);
                bodyCodec.Encode(message.Body, headWriter);
            } finally
            {
                header.Dispose();
            }
        }

        public INetMessage Decode(string? relay, ref ReadOnlySequence<byte> data, IMessageFactory messageFactory)
        {
            var reader = new SequenceReader<byte>(data);
            var headLength = reader.ReadInt32LittleEndian();
            var headSequence = reader.UnreadSequence.Slice(0, headLength);
            var headProto = MessageHeadProto.Get();
            Serializer.Deserialize(headSequence, headProto);
            reader.Advance(headLength);

            var header = headProto.ToMessageHead();
            if (!string.IsNullOrEmpty(relay))
            {
                header.PutHeader(new NatsRelayMessageHeader(relay));
            }
            var body = bodyCodec.Decode(reader.UnreadSequence);
            return messageFactory.Create(header, body);
        }
    }

}
