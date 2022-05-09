using System;

namespace TnyFramework.Codec.ProtobufNet.Protobuf
{

    /// <summary>
    /// ProtobufObjectCodec 工厂
    /// </summary>
    public class ProtobufObjectCodecFactory : ObjectCodecFactory
    {
        public ProtobufObjectCodecFactory() : base(MimeTypes.PROTOBUF)
        {
        }


        protected override IObjectCodec<T> Create<T>()
        {
            return new ProtobufObjectCodec<T>();
        }


        protected override IObjectCodec Create(Type type)
        {
            var makeGenericType = typeof(ProtobufObjectCodec<>).MakeGenericType(type);
            return (IObjectCodec) Activator.CreateInstance(makeGenericType);
        }
    }

}
