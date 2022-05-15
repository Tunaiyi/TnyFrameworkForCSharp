using System;

namespace TnyFramework.Codec.ProtobufNet.TypeProtobuf
{

    /// <summary>
    /// ProtobufObjectCodec 工厂
    /// </summary>
    public class TypeProtobufObjectCodecFactory : ObjectCodecFactory
    {
        public TypeProtobufObjectCodecFactory() : base(MimeTypes.TYPE_PROTOBUF)
        {
        }

        protected override IObjectCodec<T> Create<T>()
        {
            return new TypeProtobufObjectCodec<T>(TypeProtobufSchemeFactory.Factory);
        }

        protected override IObjectCodec Create(Type type)
        {
            var makeGenericType = typeof(TypeProtobufObjectCodec<>).MakeGenericType(type);
            return (IObjectCodec) Activator.CreateInstance(makeGenericType);
        }
    }

}
