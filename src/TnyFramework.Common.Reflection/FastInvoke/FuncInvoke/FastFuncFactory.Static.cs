// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TnyFramework.Common.Reflection.FastInvoke.FuncInvoke;

public static class FastFuncFactory
{
    public static IFastFuncFactory CreateFactory(PropertyInfo property, Type? funcInvoker = null, Type? funcReturnType = null)
    {
        return CreateFuncFactory(funcInvoker ?? property.DeclaringType, funcReturnType ?? property.PropertyType);
    }

    public static IFastFuncFactory CreateFactory(FieldInfo fieldInfo, Type? funcInvoker = null, Type? funcReturnType = null)
    {
        return CreateFuncFactory(funcInvoker ?? fieldInfo.DeclaringType, funcReturnType ?? fieldInfo.FieldType);
    }

    public static IFastFuncFactory CreateFactory(ConstructorInfo constructor)
    {
        return CreateFactory(constructor, constructor.DeclaringType, constructor.GetParameters());
    }

    public static IFastFuncFactory CreateFactory(ConstructorInfo constructor, Type? funcReturnType, params Type[] funcParamTypes)
    {
        return CreateFactory(constructor, funcReturnType, funcParamTypes.AsEnumerable());
    }

    public static IFastFuncFactory CreateFactory(ConstructorInfo constructor, Type? funcReturnType, IEnumerable<ParameterInfo> funcParamTypes)
    {
        return CreateFactory(constructor, funcReturnType, funcParamTypes.Select(param => param.ParameterType).ToList());
    }

    public static IFastFuncFactory CreateFactory(ConstructorInfo constructor, Type? funcReturnType, IEnumerable<Type> funcParamTypes)
    {
        var genericTypes = new List<Type> {typeof(object)};
        genericTypes.AddRange(funcParamTypes);
        var returnType = funcReturnType ?? constructor.DeclaringType;
        genericTypes.Add(returnType!);
        return CreateFuncFactory(constructor, genericTypes);
    }

    public static IFastFuncFactory CreateFactory(MethodInfo method)
    {
        return CreateFactory(method, method.DeclaringType, method.ReturnType, method.GetParameters());
    }

    public static IFastFuncFactory CreateFactory(MethodInfo method, Type? funcInvokerType, Type? funcReturnType, params Type[] funcParamTypes)
    {
        return CreateFactory(method, funcInvokerType, funcReturnType, funcParamTypes.AsEnumerable());
    }

    public static IFastFuncFactory CreateFactory(MethodInfo method, Type? funcInvokerType, Type? funcReturnType,
        IEnumerable<ParameterInfo> funcParamTypes)
    {
        return CreateFactory(method, funcInvokerType, funcReturnType, funcParamTypes.Select(param => param.ParameterType).ToList());
    }

    public static IFastFuncFactory CreateFactory(MethodInfo method, Type? funcInvokerType, Type? funcReturnType, IEnumerable<Type> funcParamTypes)
    {
        var genericTypes = new List<Type> {funcInvokerType ?? method.DeclaringType!};
        genericTypes.AddRange(funcParamTypes);
        genericTypes.Add(funcReturnType ?? method.ReturnType);
        return CreateFuncFactory(method, genericTypes);
    }

    public static IFastInvoker Invoker(PropertyInfo property, Type? funcInvoker = null, Type? funcReturnType = null)
    {
        return CreateFactory(property, funcInvoker, funcReturnType).CreateInvoker(property);
    }

    public static IFastInvoker Invoker(ConstructorInfo constructor, Type? funcReturnType = null)
    {
        return CreateFactory(constructor, funcReturnType!, constructor.GetParameters()).CreateInvoker(constructor);
    }

    public static IFastInvoker Invoker(ConstructorInfo constructor, Type funcReturnType, params Type[] funParamTypes)
    {
        return CreateFactory(constructor, funcReturnType, funParamTypes).CreateInvoker(constructor);
    }

    public static IFastInvoker Invoker(MethodInfo method)
    {
        return CreateFactory(method).CreateInvoker(method);
    }

    public static IFastInvoker Invoker(MethodInfo method, Type funcInvokerType, Type funcReturnType)
    {
        return CreateFactory(method, funcInvokerType, funcReturnType, method.GetParameters()).CreateInvoker(method);
    }

    public static IFastInvoker Invoker(MethodInfo method, Type funcInvokerType, Type funcReturnType, params Type[] funcParamTypes)
    {
        return CreateFactory(method, funcInvokerType, funcReturnType, funcParamTypes).CreateInvoker(method);
    }

    private static IFastFuncFactory CreateFuncFactory(Type? funcInvokerType, Type funcReturnType)
    {
        var value = Activator.CreateInstance(typeof(FastFuncFactory<,>).MakeGenericType(funcInvokerType!, funcReturnType));
        if (value is IFastFuncFactory factory)
        {
            return factory;
        }
        throw new NullReferenceException();
    }

    private static IFastFuncFactory CreateFuncFactory(Type type)
    {
        var value = Activator.CreateInstance(type);
        if (value is IFastFuncFactory factory)
        {
            return factory;
        }
        throw new NullReferenceException();
    }

    private static IFastFuncFactory CreateFuncFactory(MemberInfo member, List<Type> genericTypes)
    {
        var types = genericTypes.ToArray();
        switch (types.Length)
        {
            case 2:
                return CreateFuncFactory(typeof(FastFuncFactory<,>).MakeGenericType(types));
            case 3:
                return CreateFuncFactory(typeof(FastFuncFactory<,,>).MakeGenericType(types));
            case 4:
                return CreateFuncFactory(typeof(FastFuncFactory<,,,>).MakeGenericType(types));
            case 5:
                return CreateFuncFactory(typeof(FastFuncFactory<,,,,>).MakeGenericType(types));
            case 6:
                return CreateFuncFactory(typeof(FastFuncFactory<,,,,,>).MakeGenericType(types));
            case 7:
                return CreateFuncFactory(typeof(FastFuncFactory<,,,,,,>).MakeGenericType(types));
            case 8:
                return CreateFuncFactory(typeof(FastFuncFactory<,,,,,,,>).MakeGenericType(types));
            case 9:
                return CreateFuncFactory(typeof(FastFuncFactory<,,,,,,,,>).MakeGenericType(types));
            case 10:
                return CreateFuncFactory(typeof(FastFuncFactory<,,,,,,,,,>).MakeGenericType(types));
            case 11:
                return CreateFuncFactory(typeof(FastFuncFactory<,,,,,,,,,,>).MakeGenericType(types));
            case 12:
                return CreateFuncFactory(typeof(FastFuncFactory<,,,,,,,,,,,>).MakeGenericType(types));
            case 13:
                return CreateFuncFactory(typeof(FastFuncFactory<,,,,,,,,,,,,>).MakeGenericType(types));
            case 14:
                return CreateFuncFactory(typeof(FastFuncFactory<,,,,,,,,,,,,,>).MakeGenericType(types));
            case 15:
                return CreateFuncFactory(typeof(FastFuncFactory<,,,,,,,,,,,,,,>).MakeGenericType(types));
            case 16:
                return CreateFuncFactory(typeof(FastFuncFactory<,,,,,,,,,,,,,,,>).MakeGenericType(types));
            case 17:
                return CreateFuncFactory(typeof(FastFuncFactory<,,,,,,,,,,,,,,,,>).MakeGenericType(types));
            default:
                throw new ArgumentException($"{member} 参数数量 > 15");
        }
    }
}
