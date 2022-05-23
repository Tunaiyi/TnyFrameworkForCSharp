using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TnyFramework.Common.FastInvoke.FuncInvoke
{

    public static class FastFuncFactory
    {
        public static IFastFuncFactory CreateFactory(PropertyInfo property, Type funcInvoker = null, Type funcReturnType = null)
        {
            return CreateFuncFactory(funcInvoker ?? property.DeclaringType, funcReturnType ?? property.PropertyType);
        }

        public static IFastFuncFactory CreateFactory(FieldInfo fieldInfo, Type funcInvoker = null, Type funcReturnType = null)
        {
            return CreateFuncFactory(funcInvoker ?? fieldInfo.DeclaringType, funcReturnType ?? fieldInfo.FieldType);
        }

        public static IFastFuncFactory CreateFactory(ConstructorInfo constructor)
        {
            return CreateFactory(constructor, constructor.DeclaringType, constructor.GetParameters());
        }

        public static IFastFuncFactory CreateFactory(ConstructorInfo constructor, Type funcReturnType, params Type[] funcParamTypes)
        {
            return CreateFactory(constructor, funcReturnType, funcParamTypes.AsEnumerable());
        }

        public static IFastFuncFactory CreateFactory(ConstructorInfo constructor, Type funcReturnType, IEnumerable<ParameterInfo> funcParamTypes)
        {
            return CreateFactory(constructor, funcReturnType, funcParamTypes.Select(param => param.ParameterType).ToList());
        }

        public static IFastFuncFactory CreateFactory(ConstructorInfo constructor, Type funcReturnType, IEnumerable<Type> funcParamTypes)
        {
            var genericTypes = new List<Type> {typeof(object)};
            genericTypes.AddRange(funcParamTypes);
            genericTypes.Add(funcReturnType ?? constructor.DeclaringType);
            return CreateFuncFactory(constructor, genericTypes);
        }

        public static IFastFuncFactory CreateFactory(MethodInfo method)
        {
            return CreateFactory(method, method.DeclaringType, method.ReturnType, method.GetParameters());
        }

        public static IFastFuncFactory CreateFactory(MethodInfo method, Type funcInvokerType, Type funcReturnType, params Type[] funcParamTypes)
        {
            return CreateFactory(method, funcInvokerType, funcReturnType, funcParamTypes.AsEnumerable());
        }

        public static IFastFuncFactory CreateFactory(MethodInfo method, Type funcInvokerType, Type funcReturnType,
            IEnumerable<ParameterInfo> funcParamTypes)
        {
            return CreateFactory(method, funcInvokerType, funcReturnType, funcParamTypes.Select(param => param.ParameterType).ToList());
        }

        public static IFastFuncFactory CreateFactory(MethodInfo method, Type funcInvokerType, Type funcReturnType, IEnumerable<Type> funcParamTypes)
        {
            var genericTypes = new List<Type> {funcInvokerType ?? method.DeclaringType};
            genericTypes.AddRange(funcParamTypes);
            genericTypes.Add(funcReturnType ?? method.ReturnType);
            return CreateFuncFactory(method, genericTypes);
        }

        public static IFastInvoker Invoker(PropertyInfo property, Type funcInvoker = null, Type funcReturnType = null)
        {
            return CreateFactory(property, funcInvoker, funcReturnType).CreateInvoker(property);
        }

        public static IFastInvoker Invoker(ConstructorInfo constructor, Type funcReturnType = null)
        {
            return CreateFactory(constructor, funcReturnType, constructor.GetParameters()).CreateInvoker(constructor);
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

        private static IFastFuncFactory CreateFuncFactory(Type funcInvokerType, Type funcReturnType)
        {
            return (IFastFuncFactory) Activator.CreateInstance(typeof(FastFuncFactory<,>).MakeGenericType(funcInvokerType, funcReturnType));
        }

        private static IFastFuncFactory CreateFuncFactory(MemberInfo member, List<Type> genericTypes)
        {
            var types = genericTypes.ToArray();
            switch (types.Length)
            {
                case 2:
                    return (IFastFuncFactory) Activator.CreateInstance(typeof(FastFuncFactory<,>).MakeGenericType(types));
                case 3:
                    return (IFastFuncFactory) Activator.CreateInstance(typeof(FastFuncFactory<,,>).MakeGenericType(types));
                case 4:
                    return (IFastFuncFactory) Activator.CreateInstance(typeof(FastFuncFactory<,,,>).MakeGenericType(types));
                case 5:
                    return (IFastFuncFactory) Activator.CreateInstance(typeof(FastFuncFactory<,,,,>).MakeGenericType(types));
                case 6:
                    return (IFastFuncFactory) Activator.CreateInstance(typeof(FastFuncFactory<,,,,,>).MakeGenericType(types));
                case 7:
                    return (IFastFuncFactory) Activator.CreateInstance(typeof(FastFuncFactory<,,,,,,>).MakeGenericType(types));
                case 8:
                    return (IFastFuncFactory) Activator.CreateInstance(typeof(FastFuncFactory<,,,,,,,>).MakeGenericType(types));
                case 9:
                    return (IFastFuncFactory) Activator.CreateInstance(typeof(FastFuncFactory<,,,,,,,,>).MakeGenericType(types));
                case 10:
                    return (IFastFuncFactory) Activator.CreateInstance(typeof(FastFuncFactory<,,,,,,,,,>).MakeGenericType(types));
                case 11:
                    return (IFastFuncFactory) Activator.CreateInstance(typeof(FastFuncFactory<,,,,,,,,,,>).MakeGenericType(types));
                case 12:
                    return (IFastFuncFactory) Activator.CreateInstance(typeof(FastFuncFactory<,,,,,,,,,,,>).MakeGenericType(types));
                case 13:
                    return (IFastFuncFactory) Activator.CreateInstance(typeof(FastFuncFactory<,,,,,,,,,,,,>).MakeGenericType(types));
                case 14:
                    return (IFastFuncFactory) Activator.CreateInstance(typeof(FastFuncFactory<,,,,,,,,,,,,,>).MakeGenericType(types));
                case 15:
                    return (IFastFuncFactory) Activator.CreateInstance(typeof(FastFuncFactory<,,,,,,,,,,,,,,>).MakeGenericType(types));
                case 16:
                    return (IFastFuncFactory) Activator.CreateInstance(typeof(FastFuncFactory<,,,,,,,,,,,,,,,>).MakeGenericType(types));
                case 17:
                    return (IFastFuncFactory) Activator.CreateInstance(typeof(FastFuncFactory<,,,,,,,,,,,,,,,,>).MakeGenericType(types));
                default:
                    throw new ArgumentException($"{member} 参数数量 > 15");
            }
        }
    }

}
