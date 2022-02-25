using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using TnyFramework.Common.Exception;
namespace TnyFramework.Common.Invoke
{
    public class FastFuncExpression
    {
        public ParameterExpression[] ParameterExpressions { get; }

        public Expression BodyExpression { get; }


        public FastFuncExpression(ParameterExpression invoker, IReadOnlyList<ParameterExpression> paramExpressions, Expression bodyExpression)
        {
            var hasInvoker = invoker != null ? 1 : 0;
            ParameterExpressions = new ParameterExpression[paramExpressions.Count + hasInvoker];
            if (hasInvoker == 1)
            {
                ParameterExpressions[0] = invoker;
            }
            for (var index = 0; index < paramExpressions.Count; index++)
            {
                ParameterExpressions[index + hasInvoker] = paramExpressions[index];
            }
            BodyExpression = bodyExpression;
        }
    }

    public abstract class FastFuncFactory
    {
        protected Type T<T>()
        {
            return typeof(T);
        }



        private static FastFuncFactory CreateFuncFactory(Type invoker, Type property)
        {
            return (FastFuncFactory)Activator.CreateInstance(typeof(FastFuncFactory<,>).MakeGenericType(invoker, property));
        }


        private static FastFuncFactory CreateFuncFactory(MemberInfo member, List<Type> genericTypes)
        {
            var types = genericTypes.ToArray();
            switch (types.Length)
            {
                case 1:
                    return (FastFuncFactory)Activator.CreateInstance(typeof(FastFuncFactory<>).MakeGenericType(types));
                case 2:
                    return (FastFuncFactory)Activator.CreateInstance(typeof(FastFuncFactory<,>).MakeGenericType(types));
                case 3:
                    return (FastFuncFactory)Activator.CreateInstance(typeof(FastFuncFactory<,,>).MakeGenericType(types));
                case 4:
                    return (FastFuncFactory)Activator.CreateInstance(typeof(FastFuncFactory<,,,>).MakeGenericType(types));
                case 5:
                    return (FastFuncFactory)Activator.CreateInstance(typeof(FastFuncFactory<,,,,>).MakeGenericType(types));
                case 6:
                    return (FastFuncFactory)Activator.CreateInstance(typeof(FastFuncFactory<,,,,,>).MakeGenericType(types));
                case 7:
                    return (FastFuncFactory)Activator.CreateInstance(typeof(FastFuncFactory<,,,,,,>).MakeGenericType(types));
                case 8:
                    return (FastFuncFactory)Activator.CreateInstance(typeof(FastFuncFactory<,,,,,,,>).MakeGenericType(types));
                case 9:
                    return (FastFuncFactory)Activator.CreateInstance(typeof(FastFuncFactory<,,,,,,,,>).MakeGenericType(types));
                case 10:
                    return (FastFuncFactory)Activator.CreateInstance(typeof(FastFuncFactory<,,,,,,,,,>).MakeGenericType(types));
                case 11:
                    return (FastFuncFactory)Activator.CreateInstance(typeof(FastFuncFactory<,,,,,,,,,,>).MakeGenericType(types));
                case 12:
                    return (FastFuncFactory)Activator.CreateInstance(typeof(FastFuncFactory<,,,,,,,,,,,>).MakeGenericType(types));
                case 13:
                    return (FastFuncFactory)Activator.CreateInstance(typeof(FastFuncFactory<,,,,,,,,,,,,>).MakeGenericType(types));
                case 14:
                    return (FastFuncFactory)Activator.CreateInstance(typeof(FastFuncFactory<,,,,,,,,,,,,,>).MakeGenericType(types));
                case 15:
                    return (FastFuncFactory)Activator.CreateInstance(typeof(FastFuncFactory<,,,,,,,,,,,,,,>).MakeGenericType(types));
                case 16:
                    return (FastFuncFactory)Activator.CreateInstance(typeof(FastFuncFactory<,,,,,,,,,,,,,,,>).MakeGenericType(types));
                case 17:
                    return (FastFuncFactory)Activator.CreateInstance(typeof(FastFuncFactory<,,,,,,,,,,,,,,,,>).MakeGenericType(types));
                default:
                    throw new ArgumentException($"{member} 参数数量 > 15");
            }
        }


        public static FastFuncFactory CreateFactory(PropertyInfo property)
        {
            return CreateFuncFactory(property.DeclaringType, property.PropertyType);
        }


        public static FastFuncFactory CreateFactory(MethodInfo method)
        {
            var returnType = method.ReturnType;
            var parameters = method.GetParameters();
            var genericTypes = new List<Type>();
            if (!method.IsStatic)
            {
                genericTypes.Add(method.DeclaringType);
            }
            genericTypes.AddRange(parameters.Select(param => param.ParameterType));
            genericTypes.Add(returnType);
            return CreateFuncFactory(method, genericTypes);
        }


        public static FastFuncFactory CreateFactory(ConstructorInfo constructor)
        {
            var returnType = constructor.DeclaringType;
            var parameters = constructor.GetParameters();
            var genericTypes = parameters.Select(param => param.ParameterType).ToList();
            genericTypes.Add(returnType);
            return CreateFuncFactory(constructor, genericTypes);
        }




        public static IFastInvoker Invoker(PropertyInfo property)
        {
            return CreateFactory(property).CreateInvoker(property);
        }


        public static IFastInvoker Invoker(ConstructorInfo constructor)
        {
            return CreateFactory(constructor).CreateInvoker(constructor);
        }


        public static IFastInvoker Invoker(ConstructorInfo constructor, params Type[] funParamTypes)
        {
            return CreateFactory(constructor).CreateInvoker(constructor, null, funParamTypes);
        }


        public static IFastInvoker Invoker(MethodInfo memberInfo)
        {
            return CreateFactory(memberInfo).CreateInvoker(memberInfo);
        }


        private static IFastInvoker Invoker(MethodInfo memberInfo, Type invokerType, params Type[] funParamTypes)
        {
            return CreateFactory(memberInfo).CreateInvoker(memberInfo, invokerType, funParamTypes);
        }


        private static FastFuncExpression CreateFuncExpression(MemberInfo memberInfo, Type invokerType, params Type[] funParamTypes)
        {
            switch (memberInfo)
            {
                case ConstructorInfo constructor:
                    return CreateConstructorFuncExpression(constructor, funParamTypes);
                case PropertyInfo pro:
                    return CreatePropertyFuncExpression(pro, invokerType, pro.PropertyType);
                case MethodInfo method:
                    return CreateMethodFuncExpression(method, invokerType, funParamTypes);
                default:
                    throw new CommonException($"不支持类型 ${memberInfo.GetType()} 代理");
            }
        }


        private static FastFuncExpression CreateConstructorFuncExpression(ConstructorInfo constructor, Type[] funParamTypes)
        {
            var paramTypes = constructor.GetParameters();
            var paramExprList = new List<ParameterExpression>(); // 申明参数
            var invokeParamExprList = new List<Expression>(); // 调用传参
            for (var index = 0; index < paramTypes.Length; index++)
            {
                var param = paramTypes[index];
                var funParamType = funParamTypes[index];
                var methodParamType = param.ParameterType;

                var paramExpr = Expression.Parameter(funParamType, $"p{index}");
                paramExprList.Add(paramExpr);
                if (methodParamType == funParamType)
                {
                    invokeParamExprList.Add(paramExpr);
                    continue;
                }
                if (funParamType != typeof(object) && !methodParamType.IsAssignableFrom(funParamType))
                {
                    throw new CommonException($"{constructor} 创建异常, 第{index}个参数 method 参数类型 {methodParamType} 与 Func 参数类型 {funParamType} 不同");
                }
                var paramCastExpr = Expression.Convert(paramExpr, methodParamType);
                invokeParamExprList.Add(paramCastExpr);
            }
            var bodyExpr = Expression.New(constructor, invokeParamExprList);
            return new FastFuncExpression(null, paramExprList, bodyExpr);
        }


        private static FastFuncExpression CreatePropertyFuncExpression(PropertyInfo property, Type invokerType, Type returnType)
        {
            var invokerExpr = Expression.Parameter(invokerType, "invoker");
            var propertyType = property.PropertyType;
            var propertyExpr = Expression.Property(invokerExpr, property);
            if (propertyType == returnType)
            {
                return new FastFuncExpression(invokerExpr, new List<ParameterExpression>(), propertyExpr);
            }
            if (!propertyType.IsAssignableFrom(returnType))
            {
                return new FastFuncExpression(invokerExpr, new List<ParameterExpression>(), Expression.Convert(propertyExpr, returnType));
            }
            throw new CommonException($"{property} 创建异常, 属性类型 ${propertyType} 与 Func 参数类型 ${returnType} 不同");
        }


        private static FastFuncExpression CreateMethodFuncExpression(MethodInfo method, Type invokerType, params Type[] funParamTypes)
        {
            var paramTypes = method.GetParameters();
            ParameterExpression invokerExpr = null;
            if (!method.IsStatic)
            {
                invokerExpr = Expression.Parameter(invokerType, "invoker");
            }
            var paramExprList = new List<ParameterExpression>(); // 申明参数
            var invokeParamExprList = new List<Expression>(); // 调用传参
            for (var index = 0; index < paramTypes.Length; index++)
            {
                var param = paramTypes[index];
                var funParamType = funParamTypes[index];
                var methodParamType = param.ParameterType;

                var paramExpr = Expression.Parameter(funParamType, $"p{index}");
                paramExprList.Add(paramExpr);
                if (methodParamType == funParamType)
                {
                    invokeParamExprList.Add(paramExpr);
                    continue;
                }
                if (funParamType != typeof(object) && !methodParamType.IsAssignableFrom(funParamType))
                {
                    throw new CommonException($"{method} 创建异常, 第{index}个参数 method 参数类型 {methodParamType} 与 Func 参数类型 {funParamType} 不同");
                }
                var paramCastExpr = Expression.Convert(paramExpr, methodParamType);
                invokeParamExprList.Add(paramCastExpr);
            }
            var bodyExpr = invokerExpr != null
                ? Expression.Call(invokerExpr, method, invokeParamExprList)
                : Expression.Call(method, invokeParamExprList);
            return new FastFuncExpression(invokerExpr, paramExprList, bodyExpr);
        }


        protected static TFunc CreateFunc<TFunc>(MemberInfo member, Type invokerType, params Type[] funParamTypes)
        {
            var expressions = CreateFuncExpression(member, invokerType, funParamTypes);
            return Expression.Lambda<TFunc>(expressions.BodyExpression, expressions.ParameterExpressions).Compile();
        }


        public abstract IFastInvoker CreateInvoker(MemberInfo method, Type invokerType, params Type[] funParamTypes);


        public IFastInvoker CreateInvoker(MethodInfo method)
        {
            return CreateInvoker(method, method.DeclaringType, method.GetParameters().Select(parameterInfo => parameterInfo.ParameterType).ToArray());
        }


        public IFastInvoker CreateInvoker(PropertyInfo property)
        {
            return CreateInvoker(property, property.DeclaringType);
        }


        public IFastInvoker CreateInvoker(ConstructorInfo constructor)
        {
            return CreateInvoker(constructor, null, constructor.GetParameters().Select(parameterInfo => parameterInfo.ParameterType).ToArray());
        }
    }



    public class FastFuncFactory<TResult>
        : FastFuncFactory
    {
        public Func<TResult> Create(MethodInfo method)
        {
            return CreateFunc<Func<TResult>>(method, null);
        }


        public override IFastInvoker CreateInvoker(MemberInfo method, Type invokerType, params Type[] funParamTypes)
        {
            return new FastFunc<TResult>(CreateFunc<Func<TResult>>(
                method, invokerType, funParamTypes));
        }
    }

    public class FastFuncFactory<TFunc, TResult>
        : FastFuncFactory
    {
        public Func<TFunc, TResult> Create(MethodInfo method)
        {
            return CreateFunc<Func<TFunc, TResult>>(method, T<TFunc>());
        }


        public override IFastInvoker CreateInvoker(MemberInfo method, Type invokerType, params Type[] funParamTypes)
        {
            return new FastFunc<TFunc, TResult>(CreateFunc<Func<TFunc, TResult>>(method, invokerType, funParamTypes));
        }
    }

    public class FastFuncFactory<TFunc, TP1, TResult>
        : FastFuncFactory
    {
        public Func<TFunc, TP1, TResult> Create(MethodInfo method)
        {
            return CreateFunc<Func<TFunc, TP1, TResult>>(
                method, T<TFunc>(), T<TP1>());
        }


        public override IFastInvoker CreateInvoker(MemberInfo method, Type invokerType, params Type[] funParamTypes)
        {
            return new FastFunc<TFunc, TP1, TResult>(CreateFunc<Func<TFunc, TP1, TResult>>(method, invokerType, funParamTypes));
        }
    }

    public class FastFuncFactory<TFunc, TP1, TP2, TResult>
        : FastFuncFactory
    {
        public Func<TFunc, TP1, TP2, TResult> Create(MethodInfo method)
        {
            return CreateFunc<Func<TFunc, TP1, TP2, TResult>>(
                method, T<TFunc>(), T<TP1>(), T<TP2>());
        }


        public override IFastInvoker CreateInvoker(MemberInfo method, Type invokerType, params Type[] funParamTypes)
        {
            return new FastFunc<TFunc, TP1, TP2, TResult>(
                CreateFunc<Func<TFunc, TP1, TP2, TResult>>(
                    method, invokerType, funParamTypes));
        }
    }


    public class FastFuncFactory<TFunc, TP1, TP2, TP3, TResult>
        : FastFuncFactory
    {
        public Func<TFunc, TP1, TP2, TP3, TResult> Create(MethodInfo method)
        {
            return CreateFunc<Func<TFunc, TP1, TP2, TP3, TResult>>(
                method, T<TFunc>(), T<TP1>(), T<TP2>(), T<TP3>());
        }


        public override IFastInvoker CreateInvoker(MemberInfo method, Type invokerType, params Type[] funParamTypes)
        {
            return new FastFunc<TFunc, TP1, TP2, TP3, TResult>(
                CreateFunc<Func<TFunc, TP1, TP2, TP3, TResult>>(
                    method, invokerType, funParamTypes));
        }
    }

    public class FastFuncFactory<TFunc, TP1, TP2, TP3, TP4, TResult>
        : FastFuncFactory
    {
        public Func<TFunc, TP1, TP2, TP3, TP4, TResult> Create(MethodInfo method)
        {
            return CreateFunc<Func<TFunc, TP1, TP2, TP3, TP4, TResult>>(
                method, T<TFunc>(), T<TP1>(), T<TP2>(), T<TP3>(), T<TP4>());
        }


        public override IFastInvoker CreateInvoker(MemberInfo method, Type invokerType, params Type[] funParamTypes)
        {
            return new FastFunc<TFunc, TP1, TP2, TP3, TP4, TResult>(
                CreateFunc<Func<TFunc, TP1, TP2, TP3, TP4, TResult>>(
                    method, invokerType, funParamTypes));
        }
    }

    public class FastFuncFactory<TFunc, TP1, TP2, TP3, TP4, TP5, TResult>
        : FastFuncFactory
    {
        public Func<TFunc, TP1, TP2, TP3, TP4, TP5, TResult> Create(MethodInfo method)
        {
            return CreateFunc<Func<TFunc, TP1, TP2, TP3, TP4, TP5, TResult>>(
                method, T<TFunc>(), T<TP1>(), T<TP2>(), T<TP3>(), T<TP4>(), T<TP5>());
        }


        public override IFastInvoker CreateInvoker(MemberInfo method, Type invokerType, params Type[] funParamTypes)
        {
            return new FastFunc<TFunc, TP1, TP2, TP3, TP4, TP5, TResult>(
                CreateFunc<Func<TFunc, TP1, TP2, TP3, TP4, TP5, TResult>>(
                    method, invokerType, funParamTypes));
        }
    }

    public class FastFuncFactory<TFunc, TP1, TP2, TP3, TP4, TP5, TP6, TResult>
        : FastFuncFactory
    {
        public Func<TFunc, TP1, TP2, TP3, TP4, TP5, TP6, TResult> Create(MethodInfo method)
        {
            return CreateFunc<Func<TFunc, TP1, TP2, TP3, TP4, TP5, TP6, TResult>>(
                method, T<TFunc>(), T<TP1>(), T<TP2>(), T<TP3>(), T<TP4>(), T<TP5>(), T<TP6>());
        }


        public override IFastInvoker CreateInvoker(MemberInfo method, Type invokerType, params Type[] funParamTypes)
        {
            return new FastFunc<TFunc, TP1, TP2, TP3, TP4, TP5, TP6, TResult>(
                CreateFunc<Func<TFunc, TP1, TP2, TP3, TP4, TP5, TP6, TResult>>(
                    method, invokerType, funParamTypes));
        }
    }

    public class FastFuncFactory<TFunc, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TResult>
        : FastFuncFactory
    {
        public Func<TFunc, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TResult> Create(MethodInfo method)
        {
            return CreateFunc<Func<TFunc, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TResult>>(
                method, T<TFunc>(), T<TP1>(), T<TP2>(), T<TP3>(), T<TP4>(), T<TP5>(), T<TP6>(), T<TP7>());
        }


        public override IFastInvoker CreateInvoker(MemberInfo method, Type invokerType, params Type[] funParamTypes)
        {
            return new FastFunc<TFunc, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TResult>(
                CreateFunc<Func<TFunc, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TResult>>(
                    method, invokerType, funParamTypes));
        }
    }

    public class FastFuncFactory<TFunc, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TResult>
        : FastFuncFactory
    {
        public Func<TFunc, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TResult> Create(MethodInfo method)
        {
            return CreateFunc<Func<TFunc, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TResult>>(
                method, T<TFunc>(), T<TP1>(), T<TP2>(), T<TP3>(), T<TP4>(), T<TP5>(), T<TP6>(), T<TP7>(), T<TP8>());
        }


        public override IFastInvoker CreateInvoker(MemberInfo method, Type invokerType, params Type[] funParamTypes)
        {
            return new FastFunc<TFunc, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TResult>(
                CreateFunc<Func<TFunc, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TResult>>(
                    method, invokerType, funParamTypes));
        }
    }

    public class FastFuncFactory<TFunc, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TResult>
        : FastFuncFactory
    {
        public Func<TFunc, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TResult> Create(MethodInfo method)
        {
            return CreateFunc<Func<TFunc, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TResult>>(
                method, T<TFunc>(), T<TP1>(), T<TP2>(), T<TP3>(), T<TP4>(), T<TP5>(), T<TP6>(), T<TP7>(), T<TP8>(), T<TP9>());
        }


        public override IFastInvoker CreateInvoker(MemberInfo method, Type invokerType, params Type[] funParamTypes)
        {
            return new FastFunc<TFunc, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TResult>(
                CreateFunc<Func<TFunc, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TResult>>(
                    method, invokerType, funParamTypes));
        }
    }

    public class FastFuncFactory<TFunc, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TP10, TResult>
        : FastFuncFactory
    {
        public Func<TFunc, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TP10, TResult> Create(MethodInfo method)
        {
            return CreateFunc<Func<TFunc, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TP10, TResult>>(
                method, T<TFunc>(), T<TP1>(), T<TP2>(), T<TP3>(), T<TP4>(), T<TP5>(), T<TP6>(), T<TP7>(), T<TP8>(), T<TP9>(), T<TP10>());
        }


        public override IFastInvoker CreateInvoker(MemberInfo method, Type invokerType, params Type[] funParamTypes)
        {
            return new FastFunc<TFunc, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TP10, TResult>(
                CreateFunc<Func<TFunc, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TP10, TResult>>(
                    method, invokerType, funParamTypes));
        }
    }

    public class FastFuncFactory<TFunc, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TP10, TP11, TResult>
        : FastFuncFactory
    {
        public Func<TFunc, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TP10, TP11, TResult> Create(MethodInfo method)
        {
            return CreateFunc<Func<TFunc, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TP10, TP11, TResult>>(
                method, T<TFunc>(), T<TP1>(), T<TP2>(), T<TP3>(), T<TP4>(), T<TP5>(), T<TP6>(), T<TP7>(), T<TP8>(), T<TP9>(), T<TP10>(),
                T<TP11>());
        }


        public override IFastInvoker CreateInvoker(MemberInfo method, Type invokerType, params Type[] funParamTypes)
        {
            return new FastFunc<TFunc, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TP10, TP11, TResult>(
                CreateFunc<Func<TFunc, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TP10, TP11, TResult>>(
                    method, invokerType, funParamTypes));
        }
    }

    public class FastFuncFactory<TFunc, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TP10, TP11, TP12, TResult>
        : FastFuncFactory
    {
        public Func<TFunc, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TP10, TP11, TP12, TResult> Create(MethodInfo method)
        {
            return CreateFunc<Func<TFunc, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TP10, TP11, TP12, TResult>>(
                method, T<TFunc>(), T<TP1>(), T<TP2>(), T<TP3>(), T<TP4>(), T<TP5>(), T<TP6>(), T<TP7>(), T<TP8>(), T<TP9>(), T<TP10>(),
                T<TP11>(), T<TP12>());
        }


        public override IFastInvoker CreateInvoker(MemberInfo method, Type invokerType, params Type[] funParamTypes)
        {
            return new FastFunc<TFunc, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TP10, TP11, TP12, TResult>(
                CreateFunc<Func<TFunc, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TP10, TP11, TP12, TResult>>(
                    method, invokerType, funParamTypes));
        }
    }

    public class FastFuncFactory<TFunc, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TP10, TP11, TP12, TP13, TResult>
        : FastFuncFactory
    {
        public Func<TFunc, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TP10, TP11, TP12, TP13, TResult> Create(MethodInfo method)
        {
            return CreateFunc<Func<TFunc, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TP10, TP11, TP12, TP13, TResult>>(
                method, T<TFunc>(), T<TP1>(), T<TP2>(), T<TP3>(), T<TP4>(), T<TP5>(), T<TP6>(), T<TP7>(), T<TP8>(), T<TP9>(), T<TP10>(), T<TP11>(),
                T<TP12>(), T<TP13>());
        }


        public override IFastInvoker CreateInvoker(MemberInfo method, Type invokerType, params Type[] funParamTypes)
        {
            return new FastFunc<TFunc, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TP10, TP11, TP12, TP13, TResult>(
                CreateFunc<Func<TFunc, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TP10, TP11, TP12, TP13, TResult>>(
                    method, invokerType, funParamTypes));
        }
    }

    public class FastFuncFactory<TFunc, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TP10, TP11, TP12, TP13, TP14, TResult>
        : FastFuncFactory
    {
        public Func<TFunc, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TP10, TP11, TP12, TP13, TP14, TResult> Create(MethodInfo method)
        {
            return CreateFunc<Func<TFunc, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TP10, TP11, TP12, TP13, TP14, TResult>>(
                method, T<TFunc>(), T<TP1>(), T<TP2>(), T<TP3>(), T<TP4>(), T<TP5>(), T<TP6>(), T<TP7>(), T<TP8>(), T<TP9>(), T<TP10>(), T<TP11>(),
                T<TP12>(), T<TP13>(), T<TP14>());
        }


        public override IFastInvoker CreateInvoker(MemberInfo method, Type invokerType, params Type[] funParamTypes)
        {
            return new FastFunc<TFunc, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TP10, TP11, TP12, TP13, TP14, TResult>(
                CreateFunc<Func<TFunc, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TP10, TP11, TP12, TP13, TP14, TResult>>(
                    method, invokerType, funParamTypes));
        }
    }

    public class FastFuncFactory<TFunc, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TP10, TP11, TP12, TP13, TP14, TP15, TResult>
        : FastFuncFactory
    {
        public Func<TFunc, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TP10, TP11, TP12, TP13, TP14, TP15, TResult> Create(MethodInfo method)
        {
            return CreateFunc<Func<TFunc, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TP10, TP11, TP12, TP13, TP14, TP15, TResult>>(
                method, T<TFunc>(), T<TP1>(), T<TP2>(), T<TP3>(), T<TP4>(), T<TP5>(), T<TP6>(), T<TP7>(), T<TP8>(), T<TP9>(), T<TP10>(), T<TP11>(),
                T<TP12>(), T<TP13>(), T<TP14>(), T<TP15>());
        }


        public override IFastInvoker CreateInvoker(MemberInfo method, Type invokerType, params Type[] funParamTypes)
        {
            return new FastFunc<TFunc, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TP10, TP11, TP12, TP13, TP14, TP15, TResult>(
                CreateFunc<Func<TFunc, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TP10, TP11, TP12, TP13, TP14, TP15, TResult>>(
                    method, invokerType, funParamTypes));
        }
    }
}
