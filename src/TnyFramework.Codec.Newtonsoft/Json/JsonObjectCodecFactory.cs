using System;

namespace TnyFramework.Codec.Newtonsoft.Json
{

    /// <summary>
    /// ProtobufObjectCodec 工厂
    /// </summary>
    public class JsonObjectCodecFactory : ObjectCodecFactory
    {
        public JsonObjectCodecFactory() : base(MimeTypes.JSON)
        {
        }


        protected override IObjectCodec<T> Create<T>()
        {
            return new JsonObjectCodec<T>();
        }


        protected override IObjectCodec Create(Type type)
        {
            var makeGenericType = typeof(JsonObjectCodec<>).MakeGenericType(type);
            return (IObjectCodec) Activator.CreateInstance(makeGenericType);
        }
    }

}
