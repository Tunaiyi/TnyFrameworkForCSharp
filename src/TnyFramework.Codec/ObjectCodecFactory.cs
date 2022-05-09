using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace TnyFramework.Codec
{

    public abstract class ObjectCodecFactory : IObjectCodecFactory
    {
        private readonly ConcurrentDictionary<Type, IObjectCodec> codecs = new ConcurrentDictionary<Type, IObjectCodec>();


        public IObjectCodec<T> CreateCodec<T>()
        {
            return (IObjectCodec<T>) codecs.GetOrAdd(typeof(T), _ => Create<T>());
        }


        public IObjectCodec CreateCodec(Type type)
        {
            return codecs.GetOrAdd(type, Create);
        }


        public IReadOnlyList<IMimeType> MediaTypes { get; }


        public ObjectCodecFactory(IMimeType mimeType, params IMimeType[] mediaTypes)
        {
            var types = new List<IMimeType> {mimeType};
            types.AddRange(mediaTypes);
            MediaTypes = types.ToImmutableList();
        }


        protected abstract IObjectCodec<T> Create<T>();

        protected abstract IObjectCodec Create(Type type);
    }

}
