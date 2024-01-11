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

namespace TnyFramework.Common.Reflection.FastInvoke.ActionInvoke;

public static class FastActionFactory
{
    public static IFastActionFactory CreateFactory(PropertyInfo property, Type? actionInvokerType = null, Type? actionParamType = null)
    {
        return CreateActionFactory(actionInvokerType ?? property.DeclaringType, actionParamType ?? property.PropertyType);
    }

    public static IFastActionFactory CreateFactory(FieldInfo field, Type? actionInvokerType = null, Type? actionParamType = null)
    {
        return CreateActionFactory(actionInvokerType ?? field.DeclaringType, actionParamType ?? field.FieldType);
    }

    public static IFastActionFactory CreateFactory(MethodInfo method)
    {
        return CreateFactory(method, method.DeclaringType, method.GetParameters());
    }

    public static IFastActionFactory CreateFactory(MethodInfo method, Type? actionInvokerType, params Type[] actionParamTypes)
    {
        return CreateFactory(method, actionInvokerType, actionParamTypes.AsEnumerable());
    }

    public static IFastActionFactory CreateFactory(MethodInfo method, Type? actionInvokerType, IEnumerable<ParameterInfo> actionParamTypes)
    {
        return CreateFactory(method, actionInvokerType, actionParamTypes.Select(param => param.ParameterType));
    }

    public static IFastActionFactory CreateFactory(MethodInfo method, Type? actionInvokerType, IEnumerable<Type> actionParamTypes)
    {
        var genericTypes = new List<Type> {actionInvokerType ?? method.DeclaringType!};
        genericTypes.AddRange(actionParamTypes);
        return CreateActionFactory(method, genericTypes);
    }

    public static IFastInvoker Invoker(PropertyInfo property, Type? actionInvokerType = null, Type? valueType = null)
    {
        return CreateFactory(property, actionInvokerType, valueType).CreateInvoker(property);
    }

    public static IFastInvoker Invoker(FieldInfo field, Type? actionInvokerType = null, Type? valueType = null)
    {
        return CreateFactory(field, actionInvokerType, valueType).CreateInvoker(field);
    }

    public static IFastInvoker Invoker(MethodInfo method)
    {
        return CreateFactory(method).CreateInvoker(method);
    }

    public static IFastInvoker Invoker(MethodInfo method, Type? actionInvokerType)
    {
        return CreateFactory(method, actionInvokerType, method.GetParameters()).CreateInvoker(method);
    }

    public static IFastInvoker Invoker(MethodInfo method, Type? actionInvokerType, params Type[] actionParamTypes)
    {
        return CreateFactory(method, actionInvokerType, actionParamTypes).CreateInvoker(method);
    }

    private static IFastActionFactory CreateActionFactory(Type? invoker, Type property)
    {
        var value = Activator.CreateInstance(typeof(FastActionFactory<,>).MakeGenericType(invoker!, property));
        if (value is IFastActionFactory factory)
        {
            return factory;
        }
        throw new NullReferenceException();
    }

    private static IFastActionFactory CreateActionFactory(Type invoker)
    {
        var value = Activator.CreateInstance(invoker);
        if (value is IFastActionFactory factory)
        {
            return factory;
        }
        throw new NullReferenceException();
    }

    private static IFastActionFactory CreateActionFactory(MemberInfo member, List<Type> genericTypes)
    {
        var types = genericTypes.ToArray();
        switch (types.Length)
        {
            case 1:
                return CreateActionFactory(typeof(FastActionFactory<>).MakeGenericType(types));
            case 2:
                return CreateActionFactory(typeof(FastActionFactory<,>).MakeGenericType(types));
            case 3:
                return CreateActionFactory(typeof(FastActionFactory<,,>).MakeGenericType(types));
            case 4:
                return CreateActionFactory(typeof(FastActionFactory<,,,>).MakeGenericType(types));
            case 5:
                return CreateActionFactory(typeof(FastActionFactory<,,,,>).MakeGenericType(types));
            case 6:
                return CreateActionFactory(typeof(FastActionFactory<,,,,,>).MakeGenericType(types));
            case 7:
                return CreateActionFactory(typeof(FastActionFactory<,,,,,,>).MakeGenericType(types));
            case 8:
                return CreateActionFactory(typeof(FastActionFactory<,,,,,,,>).MakeGenericType(types));
            case 9:
                return CreateActionFactory(typeof(FastActionFactory<,,,,,,,,>).MakeGenericType(types));
            case 10:
                return CreateActionFactory(typeof(FastActionFactory<,,,,,,,,,>).MakeGenericType(types));
            case 11:
                return CreateActionFactory(typeof(FastActionFactory<,,,,,,,,,,>).MakeGenericType(types));
            case 12:
                return CreateActionFactory(typeof(FastActionFactory<,,,,,,,,,,,>).MakeGenericType(types));
            case 13:
                return CreateActionFactory(typeof(FastActionFactory<,,,,,,,,,,,,>).MakeGenericType(types));
            case 14:
                return CreateActionFactory(typeof(FastActionFactory<,,,,,,,,,,,,,>).MakeGenericType(types));
            case 15:
                return CreateActionFactory(typeof(FastActionFactory<,,,,,,,,,,,,,,>).MakeGenericType(types));
            case 16:
                return CreateActionFactory(typeof(FastActionFactory<,,,,,,,,,,,,,,,>).MakeGenericType(types));
            default:
                throw new ArgumentException($"{member} 参数数量 > 15");
        }
    }
}
