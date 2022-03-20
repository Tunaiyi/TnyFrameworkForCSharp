#region

using System;
using System.Collections.Concurrent;
using System.Reflection;
using TnyFramework.Common.Exception;
using TnyFramework.Net.TypeProtobuf.Attributes;

#endregion

namespace TnyFramework.Net.TypeProtobuf
{

    public class TypeProtobufObjectFactory : ITypeProtobufObjectFactory
    {
        private readonly ConcurrentDictionary<int, TypeProtobufObject> idObjectInfoMap = new ConcurrentDictionary<int, TypeProtobufObject>();

        private readonly ConcurrentDictionary<Type, TypeProtobufObject> typeObjectInfoMap = new ConcurrentDictionary<Type, TypeProtobufObject>();


        private TypeProtobufObjectFactory()
        {
        }


        public ITypeProtobufObjectFactory Register<T>() where T : new()
        {
            var type = typeof(T);
            var attribute = type.GetCustomAttribute<TypeProtobufAttribute>();
            if (attribute == null)
            {
                throw new NullReferenceException($"{type} 没有 TypeProtobufAttribute 特性, 无法进行注册");
            }
            return Register<T>(attribute.Id);
        }


        public ITypeProtobufObjectFactory Register<T>(int id) where T : new()
        {
            return Register(new TypeProtobufObject(id, typeof(T), () => new T()));
        }


        public ITypeProtobufObjectFactory Register(Type type)
        {
            var attribute = type.GetCustomAttribute<TypeProtobufAttribute>();
            if (attribute == null)
            {
                throw new NullReferenceException($"{type} 没有 TypeProtobufAttribute 特性, 无法进行注册");
            }
            return Register(new TypeProtobufObject(attribute.Id, type));
        }


        private ITypeProtobufObjectFactory Register(TypeProtobufObject info)
        {
            if (idObjectInfoMap.TryAdd(info.Id, info))
            {
                typeObjectInfoMap.TryAdd(info.Type, info);
                return this;
            }
            var exist = idObjectInfoMap[info.Id];
            if (exist.Type != info.Type)
            {
                throw new IllegalArgumentException($"{info.Type} 与 {exist.Type} TypeProtobufAttribute Id 都为 {info.Id}");
            }
            return this;
        }


        public static ITypeProtobufObjectFactory Factory { get; } = new TypeProtobufObjectFactory();


        public T Create<T>(int id) where T : new()
        {
            if (!idObjectInfoMap.TryGetValue(id, out var info))
            {
                throw new NullReferenceException($"未知 TypeProtobufAttribute Id {id}");
            }
            return (T) info.Create();
        }


        public bool Id<T>(out int id) where T : new()
        {
            return Id(typeof(T), out id);
        }


        public bool Id(Type type, out int id)
        {
            var info = typeObjectInfoMap[type];
            if (info == null)
            {
                id = -1;
                return false;
            }
            id = info.Id;
            return true;
        }
    }

}
