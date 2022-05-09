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
        private readonly TypeProtobufObjectCodecFactory codecFactory;

        private static readonly FastThreadLocal<MemoryStream> STEAM_LOCAL = new FastThreadLocal<MemoryStream>();


        public MessageHeaderCodec()
        {
            codecFactory = new TypeProtobufObjectCodecFactory();
        }


        public MessageHeader Decode(IByteBuffer buffer)
        {
            var codec = codecFactory.CreateCodec(typeof(object));
            ByteBufferUtils.ReadVariant(buffer, out int bodyLength);
            var body = buffer.ReadBytes(bodyLength);
            using (var stream = new ReadOnlyByteBufferStream(body, true))
            {
                return (MessageHeader) codec.Decode(stream);
            }
        }


        public void Encode(MessageHeader body, IByteBuffer buffer)
        {
            var type = body.GetType();
            var codec = codecFactory.CreateCodec(type);
            if (codec == null)
                throw new System.Exception($"不存在该DTO:{type}");
            var stream = Stream();
            try
            {
                codec.Encode(body, stream);
                if (stream.Position <= 0)
                    return;
                ByteBufferUtils.WriteVariant(stream.Position, buffer);
                buffer.WriteBytes(stream.GetBuffer(), 0, (int) stream.Position);
            } finally
            {
                stream.Seek(0, SeekOrigin.Begin);
            }
        }


        public void Encode(object body, IByteBuffer buffer)
        {
            if (body is MessageHeader header)
                Encode(header, buffer);
        }


        object INetContentCodec.Decode(IByteBuffer buffer)
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
