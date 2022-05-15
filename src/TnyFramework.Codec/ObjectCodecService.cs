using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Reflection;
using TnyFramework.Codec.Attributes;
using TnyFramework.Codec.Execptions;

namespace TnyFramework.Codec
{

    /// <summary>
    /// 对象编解码服务
    /// </summary>
    public class ObjectCodecService
    {
        private IDictionary<IMimeType, ObjectCodecFactory> codecFactoryMap;

        private ConcurrentDictionary<Type, ObjectCodecHolder> objectCodecHolderMap = new ConcurrentDictionary<Type, ObjectCodecHolder>();

        public ObjectCodecService(IEnumerable<ObjectCodecFactory> codecFactories)
        {
            var factoryMap = new Dictionary<IMimeType, ObjectCodecFactory>();
            foreach (var objectCodecFactory in codecFactories)
            {
                foreach (var mimeType in objectCodecFactory.MediaTypes)
                {
                    factoryMap.Add(mimeType, objectCodecFactory);
                }
            }
            codecFactoryMap = factoryMap.ToImmutableDictionary();
        }

        public bool IsSupported(IMimeType type)
        {
            return codecFactoryMap.ContainsKey(type);
        }

        public IObjectCodec<T> Codec<T>()
        {
            var objectType = typeof(T);
            return CastCodec<T>(Codec(objectType));
        }

        public IObjectCodec Codec(Type objectType)
        {
            var holder = GetHolder(objectType);
            return holder.DefaultCodec;
        }

        public IObjectCodec<T> Codec<T>(IMimeType mimeType)
        {
            var objectType = typeof(T);
            return CastCodec<T>(Codec(objectType, mimeType));
        }

        public IObjectCodec Codec(Type objectType, IMimeType mimeType)
        {
            var holder = GetHolder(objectType);
            return holder.LoadObjectCodec(mimeType, CreateObjectCodec);
        }

        private ObjectCodecHolder GetHolder(Type objectType)
        {
            return objectCodecHolderMap.GetOrAdd(objectType, type => new ObjectCodecHolder(type, CreateObjectCodec));
        }

        private static IObjectCodec<T> CastCodec<T>(IObjectCodec target)
        {
            if (target is IObjectCodec<T> codec)
                return codec;
            throw new InvalidCastException($"{target} cast {typeof(T)} exception");
        }

        private IObjectCodec CreateObjectCodec(IMimeType format, Type type)
        {

            if (codecFactoryMap.TryGetValue(format, out var factory))
            {
                return factory.CreateCodec(type);
            }
            throw new NullReferenceException($"Type {type} get {format} ObjectCodecFactory is null");
        }

        public byte[] EncodeToBytes(object value)
        {
            var holder = GetHolder(value.GetType());
            try
            {
                return holder.DefaultCodec.Encode(value);
            } catch (Exception e)
            {
                throw new ObjectCodecException($"encode {value.GetType()} to default format {holder.DefaultFormat} exception", e);
            }
        }

        public byte[] EncodeToBytes(object value, IMimeType mineType)
        {
            var holder = GetHolder(value.GetType());
            try
            {
                var codec = holder.LoadObjectCodec(mineType, CreateObjectCodec);
                return codec.Encode(value);
            } catch (Exception e)
            {
                throw new ObjectCodecException($"encode {value.GetType()} to default format {mineType} exception", e);
            }
        }

        public T DecodeByBytes<T>(byte[] data)
        {
            var holder = GetHolder(typeof(T));
            try
            {
                var value = holder.DefaultCodec.Decode(data);
                switch (value)
                {
                    case null:
                        return default;
                    case T result:
                        return result;
                    default:
                        throw new InvalidCastException($"{value} cast to {typeof(T)} exception");
                }
            } catch (IOException e)
            {
                throw new ObjectCodecException($"decode {typeof(T)} to default format {holder.DefaultFormat} exception", e);
            }
        }

        public T DecodeByBytes<T>(byte[] data, IMimeType mimeType)
        {
            var holder = GetHolder(typeof(T));
            try
            {
                var value = holder.LoadObjectCodec(mimeType, CreateObjectCodec).Decode(data);
                switch (value)
                {
                    case null:
                        return default;
                    case T result:
                        return result;
                    default:
                        throw new InvalidCastException($"{value} cast to {typeof(T)} exception");
                }
            } catch (IOException e)
            {
                throw new ObjectCodecException($"decode {typeof(T)} to default format {holder.DefaultFormat} exception", e);
            }
        }

        private class ObjectCodecHolder
        {
            private Type Type { get; }

            public IMimeType DefaultFormat { get; }

            public IObjectCodec DefaultCodec { get; }

            private readonly IDictionary<IMimeType, IObjectCodec> objectCodes = new Dictionary<IMimeType, IObjectCodec>();

            public ObjectCodecHolder(Type type, Func<IMimeType, Type, IObjectCodec> factory)
            {
                Type = type;
                var attribute = type.GetCustomAttribute<CodableAttribute>();
                DefaultFormat = MimeType.ForMetaType(attribute.MimeType);
                DefaultCodec = factory(DefaultFormat, type);
                AddCodec(DefaultFormat, DefaultCodec);
            }

            public IObjectCodec LoadObjectCodec(IMimeType mimeType, Func<IMimeType, Type, IObjectCodec> factory)
            {
                return objectCodes.TryGetValue(mimeType, out var codec) ? codec : Load(mimeType, factory);
            }

            private IObjectCodec Load(IMimeType mimeType, Func<IMimeType, Type, IObjectCodec> factory)
            {
                lock (this)
                {
                    if (objectCodes.TryGetValue(mimeType, out var codec))
                    {
                        return codec;
                    }
                    codec = factory(mimeType, Type);
                    objectCodes.Add(mimeType, codec);
                    return codec;
                }
            }

            private void AddCodec(IMimeType mimeType, IObjectCodec codec)
            {
                lock (this)
                {
                    if (objectCodes.ContainsKey(mimeType))
                        return;
                    objectCodes.Add(mimeType, codec);
                }
            }
        }
    }

}
