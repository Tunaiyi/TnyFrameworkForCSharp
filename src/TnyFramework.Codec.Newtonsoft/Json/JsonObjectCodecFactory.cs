using System;
using Newtonsoft.Json;

namespace TnyFramework.Codec.Newtonsoft.Json
{

    /// <summary>
    /// ProtobufObjectCodec 工厂
    /// </summary>
    public class JsonObjectCodecFactory : ObjectCodecFactory
    {
        private readonly JsonSerializerSettings formatting;

        public JsonObjectCodecFactory() : base(MimeTypes.JSON)
        {
            formatting = null;
        }

        public JsonObjectCodecFactory(JsonSerializerSettings formatting) : base(MimeTypes.JSON)
        {
            this.formatting = formatting;
        }

        protected override IObjectCodec<T> Create<T>()
        {
            return new JsonObjectCodec<T>(formatting);
        }

        protected override IObjectCodec Create(Type type)
        {
            var makeGenericType = typeof(JsonObjectCodec<>).MakeGenericType(type);
            return (IObjectCodec) Activator.CreateInstance(makeGenericType, formatting);
        }
    }

}
