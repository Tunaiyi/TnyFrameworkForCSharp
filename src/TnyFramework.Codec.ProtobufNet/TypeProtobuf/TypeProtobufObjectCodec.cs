using System;
using System.IO;
using ProtoBuf;
using TnyFramework.Codec.Execptions;

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

        public override byte[] Encode(T value)
        {
            if (value == null)
            {
                return Array.Empty<byte>();
            }
            var scheme = factory.Load(value.GetType());
            if (scheme == null)
            {
                return Array.Empty<byte>();
            }
            var stream = new MemoryStream(256);
            ByteUtils.WriteFixed32(scheme.Id, stream);
            Serializer.Serialize(stream, value);
            return stream.ToArray();
        }

        public override void Encode(T value, Stream output)
        {
            if (value == null)
            {
                return;
            }
            var scheme = factory.Load(value.GetType());
            if (scheme == null)
                return;
            ByteUtils.WriteFixed32(scheme.Id, output);
            Serializer.Serialize(output, value);
        }

        public override T Decode(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0)
            {
                return default;
            }
            var memory = new MemoryStream(bytes);
            ByteUtils.ReadFixed32(memory, out int id);
            if (factory.Get(id, out var scheme))
            {
                return (T) Serializer.Deserialize(scheme.Type, memory);
            }
            throw new ObjectCodecException($"Unknown ${id} TypeProtobufScheme");
        }

        public override T Decode(Stream input)
        {
            if (input == null || input.Length == 0)
            {
                return default;
            }
            ByteUtils.ReadFixed32(input, out int id);
            if (factory.Get(id, out var scheme))
            {
                return (T) Serializer.Deserialize(scheme.Type, input);
            }
            throw new ObjectCodecException($"Unknown ${id} TypeProtobufScheme");
        }
    }

}
