// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Reflection;
using System.Threading;
using TnyFramework.Codec.Attributes;
using TnyFramework.Codec.Exceptions;
using TnyFramework.Common.Extensions;

namespace TnyFramework.Codec
{

    /// <summary>
    /// 对象编解码服务
    /// </summary>
    public class ObjectCodecAdapter
    {
        private readonly IDictionary<IMimeType, ObjectCodecFactory> codecFactoryMap;

        private readonly ConcurrentDictionary<Type, ObjectCodecHolder> objectCodecHolderMap = new ConcurrentDictionary<Type, ObjectCodecHolder>();

        public ObjectCodecAdapter(IEnumerable<ObjectCodecFactory> codecFactories)
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

        public IObjectCodec<T> Codec<T>(ObjectMimeType<T> objectMimeType)
        {
            return objectMimeType.HasMimeType() ? Codec<T>(objectMimeType.MimeType) : Codec<T>();
        }

        public IObjectCodec Codec(Type objectType, IMimeType mimeType)
        {
            var holder = GetHolder(objectType);
            return holder.LoadObjectCodec(mimeType, CreateObjectCodec);
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

        public byte[] EncodeToBytes(object value)
        {
            var codec = Codec(value.GetType());
            try
            {
                return codec.Encode(value);
            } catch (Exception e)
            {
                throw new ObjectCodecException($"encode {value.GetType()} to default format {codec} exception", e);
            }
        }

        public byte[] EncodeToBytes<T>(ObjectMimeType<T> mimeType, T value)
        {
            var codec = Codec(mimeType);
            try
            {
                return codec.Encode(value);
            } catch (Exception e)
            {
                throw new ObjectCodecException($"encode {value?.GetType()} to default format {codec} exception", e);
            }
        }

        public byte[] EncodeToBytes(object value, IMimeType mineType)
        {
            var codec = Codec(value.GetType(), mineType);
            try
            {
                return codec.Encode(value);
            } catch (Exception e)
            {
                throw new ObjectCodecException($"encode {value.GetType()} to default format {mineType} exception", e);
            }
        }

        public T? DecodeByBytes<T>(byte[] data)
        {
            var codec = Codec<T>();
            try
            {
                var value = codec.Decode(data);
                if (value == null)
                {
                    return default;
                }
                return value switch {
                    not null => value,
                    _ => throw new InvalidCastException($"{value} cast to {typeof(T)} exception")
                };
            } catch (IOException e)
            {
                throw new ObjectCodecException($"decode {typeof(T)} to default format {codec} exception", e);
            }
        }

        public T? DecodeByBytes<T>(ObjectMimeType<T> mimeType, byte[] data)
        {
            var codec = Codec(mimeType);
            try
            {
                var value = codec.Decode(data);
                if (value == null)
                {
                    return default;
                }
                return value switch {
                    not null => value,
                    _ => throw new InvalidCastException($"{value} cast to {typeof(T)} exception")
                };
            } catch (IOException e)
            {
                throw new ObjectCodecException($"decode {typeof(T)} to default format {codec} exception", e);
            }
        }

        public T? DecodeByBytes<T>(IMimeType mimeType, byte[] data)
        {
            var codec = Codec<T>(mimeType);
            try
            {
                var value = codec.Decode(data);
                if (value == null)
                {
                    return default;
                }
                return value switch {
                    { } => value,
                    _ => throw new InvalidCastException($"{value} cast to {typeof(T)} exception")
                };
            } catch (IOException e)
            {
                throw new ObjectCodecException($"decode {typeof(T)} to default format {codec} exception", e);
            }
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

        private class ObjectCodecHolder
        {
            private Type Type { get; }

            public IMimeType DefaultFormat { get; }

            public IObjectCodec DefaultCodec { get; }

            private readonly IDictionary<IMimeType, IObjectCodec> objectCodes = new Dictionary<IMimeType, IObjectCodec>();

            private readonly ReaderWriterLockSlim lockSlim = new ReaderWriterLockSlim();

            public ObjectCodecHolder(Type type, Func<IMimeType, Type, IObjectCodec> factory)
            {
                Type = type;
                var attribute = type.GetCustomAttribute<CodableAttribute>();
                var defaultType = attribute?.Mime;
                if (defaultType.IsBlank())
                {
                    DefaultFormat = null!;
                    DefaultCodec = null!;
                    return;
                }
                DefaultFormat = MimeType.ForMimeType(defaultType!);
                DefaultCodec = factory(DefaultFormat, type);
                AddCodec(DefaultFormat, DefaultCodec);
            }

            public IObjectCodec LoadObjectCodec(IMimeType mimeType, Func<IMimeType, Type, IObjectCodec> factory)
            {
                lockSlim.EnterUpgradeableReadLock();
                try
                {
                    return objectCodes.TryGetValue(mimeType, out var codec) ? codec : Load(mimeType, factory);
                } finally
                {
                    lockSlim.ExitUpgradeableReadLock();
                }
            }

            private IObjectCodec Load(IMimeType mimeType, Func<IMimeType, Type, IObjectCodec> factory)
            {
                lockSlim.EnterWriteLock();
                try
                {
                    if (objectCodes.TryGetValue(mimeType, out var codec))
                    {
                        return codec;
                    }
                    codec = factory(mimeType, Type);
                    objectCodes.Add(mimeType, codec);
                    return codec;
                } finally
                {
                    lockSlim.ExitWriteLock();
                }
            }

            private void AddCodec(IMimeType mimeType, IObjectCodec codec)
            {

                lockSlim.EnterUpgradeableReadLock();
                try
                {
                    if (objectCodes.ContainsKey(mimeType))
                        return;
                    lockSlim.EnterWriteLock();
                    try
                    {
                        if (objectCodes.ContainsKey(mimeType))
                            return;
                        objectCodes.Add(mimeType, codec);
                    } finally
                    {
                        lockSlim.ExitWriteLock();
                    }
                } finally
                {
                    lockSlim.ExitUpgradeableReadLock();
                }
            }
        }
    }

}
