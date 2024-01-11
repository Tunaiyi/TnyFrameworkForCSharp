// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.Collections.Concurrent;
using System.Reflection;
using TnyFramework.Codec.ProtobufNet.Attributes;
using TnyFramework.Common.Exceptions;

namespace TnyFramework.Codec.ProtobufNet.TypeProtobuf;

/// <summary>
/// ProtobufObjectCodec 工厂
/// </summary>
public class TypeProtobufSchemeFactory
{
    private readonly ConcurrentDictionary<int, TypeProtobufScheme> idSchemeInfoMap = new ConcurrentDictionary<int, TypeProtobufScheme>();

    private readonly ConcurrentDictionary<Type, TypeProtobufScheme> typeSchemeInfoMap = new ConcurrentDictionary<Type, TypeProtobufScheme>();

    public static TypeProtobufSchemeFactory Factory { get; } = new TypeProtobufSchemeFactory();

    private TypeProtobufSchemeFactory()
    {
    }

    public TypeProtobufScheme Load<T>() where T : new()
    {
        var type = typeof(T);
        var attribute = type.GetCustomAttribute<TypeProtobufAttribute>();
        if (attribute == null)
        {
            throw new NullReferenceException($"{type} 没有 TypeProtobufAttribute 特性, 无法进行注册");
        }
        return Load<T>(attribute.Id);
    }

    public TypeProtobufScheme Load<T>(int id) where T : new()
    {
        return Register(new TypeProtobufScheme(id, typeof(T), () => new T()));
    }

    public TypeProtobufScheme Load(Type type)
    {
        var attribute = type.GetCustomAttribute<TypeProtobufAttribute>();
        if (attribute == null)
        {
            throw new NullReferenceException($"{type} 没有 TypeProtobufAttribute 特性, 无法进行注册");
        }
        return Register(new TypeProtobufScheme(attribute.Id, type));
    }

    private TypeProtobufScheme Register(TypeProtobufScheme info)
    {
        if (idSchemeInfoMap.TryAdd(info.Id, info))
        {
            typeSchemeInfoMap.TryAdd(info.Type, info);
            return info;
        }
        var exist = idSchemeInfoMap[info.Id];
        if (exist.Type != info.Type)
        {
            throw new IllegalArgumentException($"{info.Type} 与 {exist.Type} TypeProtobufAttribute Id 都为 {info.Id}");
        }
        return exist;
    }

    public T Create<T>(int id) where T : new()
    {
        if (!idSchemeInfoMap.TryGetValue(id, out var info))
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
        if (typeSchemeInfoMap.TryGetValue(type, out var info))
        {
            id = info.Id;
            return true;
        }
        id = -1;
        return false;
    }

    public bool Get(int id, out TypeProtobufScheme scheme)
    {
        if (idSchemeInfoMap.TryGetValue(id, out var info))
        {
            scheme = info;
            return true;
        }
        scheme = null!;
        return false;
    }
}
