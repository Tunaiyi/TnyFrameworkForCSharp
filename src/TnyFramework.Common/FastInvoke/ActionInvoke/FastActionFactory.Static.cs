using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TnyFramework.Common.FastInvoke.ActionInvoke
{

    public static class FastActionFactory
    {
        public static IFastActionFactory CreateFactory(PropertyInfo property, Type actionInvokerType = null, Type actionParamType = null)
        {
            return CreateActionFactory(actionInvokerType ?? property.DeclaringType, actionParamType ?? property.PropertyType);
        }

        public static IFastActionFactory CreateFactory(FieldInfo field, Type actionInvokerType = null, Type actionParamType = null)
        {
            return CreateActionFactory(actionInvokerType ?? field.DeclaringType, actionParamType ?? field.FieldType);
        }

        public static IFastActionFactory CreateFactory(MethodInfo method)
        {
            return CreateFactory(method, method.DeclaringType, method.GetParameters());
        }

        public static IFastActionFactory CreateFactory(MethodInfo method, Type actionInvokerType, params Type[] actionParamTypes)
        {
            return CreateFactory(method, actionInvokerType, actionParamTypes.AsEnumerable());
        }

        public static IFastActionFactory CreateFactory(MethodInfo method, Type actionInvokerType, IEnumerable<ParameterInfo> actionParamTypes)
        {
            return CreateFactory(method, actionInvokerType, actionParamTypes.Select(param => param.ParameterType));
        }

        public static IFastActionFactory CreateFactory(MethodInfo method, Type actionInvokerType, IEnumerable<Type> actionParamTypes)
        {
            var genericTypes = new List<Type> {actionInvokerType ?? method.DeclaringType};
            genericTypes.AddRange(actionParamTypes);
            return CreateActionFactory(method, genericTypes);
        }

        public static IFastInvoker Invoker(PropertyInfo property, Type actionInvokerType = null, Type valueType = null)
        {
            return CreateFactory(property, actionInvokerType, valueType).CreateInvoker(property);
        }

        public static IFastInvoker Invoker(FieldInfo field, Type actionInvokerType = null, Type valueType = null)
        {
            return CreateFactory(field, actionInvokerType, valueType).CreateInvoker(field);
        }

        public static IFastInvoker Invoker(MethodInfo method)
        {
            return CreateFactory(method).CreateInvoker(method);
        }

        public static IFastInvoker Invoker(MethodInfo method, Type actionInvokerType)
        {
            return CreateFactory(method, actionInvokerType, method.GetParameters()).CreateInvoker(method);
        }

        public static IFastInvoker Invoker(MethodInfo method, Type actionInvokerType, params Type[] actionParamTypes)
        {
            return CreateFactory(method, actionInvokerType, actionParamTypes).CreateInvoker(method);
        }

        private static IFastActionFactory CreateActionFactory(Type invoker, Type property)
        {
            return (IFastActionFactory) Activator.CreateInstance(typeof(FastActionFactory<,>).MakeGenericType(invoker, property));
        }

        private static IFastActionFactory CreateActionFactory(MemberInfo member, List<Type> genericTypes)
        {
            var types = genericTypes.ToArray();
            switch (types.Length)
            {
                case 1:
                    return (IFastActionFactory) Activator.CreateInstance(typeof(FastActionFactory<>).MakeGenericType(types));
                case 2:
                    return (IFastActionFactory) Activator.CreateInstance(typeof(FastActionFactory<,>).MakeGenericType(types));
                case 3:
                    return (IFastActionFactory) Activator.CreateInstance(typeof(FastActionFactory<,,>).MakeGenericType(types));
                case 4:
                    return (IFastActionFactory) Activator.CreateInstance(typeof(FastActionFactory<,,,>).MakeGenericType(types));
                case 5:
                    return (IFastActionFactory) Activator.CreateInstance(typeof(FastActionFactory<,,,,>).MakeGenericType(types));
                case 6:
                    return (IFastActionFactory) Activator.CreateInstance(typeof(FastActionFactory<,,,,,>).MakeGenericType(types));
                case 7:
                    return (IFastActionFactory) Activator.CreateInstance(typeof(FastActionFactory<,,,,,,>).MakeGenericType(types));
                case 8:
                    return (IFastActionFactory) Activator.CreateInstance(typeof(FastActionFactory<,,,,,,,>).MakeGenericType(types));
                case 9:
                    return (IFastActionFactory) Activator.CreateInstance(typeof(FastActionFactory<,,,,,,,,>).MakeGenericType(types));
                case 10:
                    return (IFastActionFactory) Activator.CreateInstance(typeof(FastActionFactory<,,,,,,,,,>).MakeGenericType(types));
                case 11:
                    return (IFastActionFactory) Activator.CreateInstance(typeof(FastActionFactory<,,,,,,,,,,>).MakeGenericType(types));
                case 12:
                    return (IFastActionFactory) Activator.CreateInstance(typeof(FastActionFactory<,,,,,,,,,,,>).MakeGenericType(types));
                case 13:
                    return (IFastActionFactory) Activator.CreateInstance(typeof(FastActionFactory<,,,,,,,,,,,,>).MakeGenericType(types));
                case 14:
                    return (IFastActionFactory) Activator.CreateInstance(typeof(FastActionFactory<,,,,,,,,,,,,,>).MakeGenericType(types));
                case 15:
                    return (IFastActionFactory) Activator.CreateInstance(typeof(FastActionFactory<,,,,,,,,,,,,,,>).MakeGenericType(types));
                case 16:
                    return (IFastActionFactory) Activator.CreateInstance(typeof(FastActionFactory<,,,,,,,,,,,,,,,>).MakeGenericType(types));
                default:
                    throw new ArgumentException($"{member} 参数数量 > 15");
            }
        }
    }

}
