using System;
using System.IO;
using ProtoBuf;

namespace TnyFramework.Codec.ProtobufNet.Protobuf
{

    public class ProtobufObjectCodec<T> : BytesObjectCodec<T>
    {
        public ProtobufObjectCodec()
        {
        }


        public override byte[] Encode(T value)
        {
            var stream = new MemoryStream();
            Serializer.Serialize(stream, value);
            return stream.ToArray();
        }


        public override void Encode(T value, Stream output)
        {
            var stream = new MemoryStream();
            Serializer.Serialize(stream, value);
        }


        public override T Decode(byte[] bytes)
        {
            var memory = new ReadOnlyMemory<byte>(bytes);
            return Serializer.Deserialize<T>(memory);
        }


        public override T Decode(Stream input)
        {
            return Serializer.Deserialize<T>(input);
        }
    }

}
