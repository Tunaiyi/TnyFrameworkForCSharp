using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using TnyFramework.Common.Exception;
namespace TnyFramework.Common.Invoke
{
    public class FastActionExpression
    {
        public ParameterExpression[] ParameterExpressions { get; }

        public Expression BodyExpression { get; }


        public FastActionExpression(ParameterExpression invoker, IReadOnlyList<ParameterExpression> paramExpressions, Expression bodyExpression)
        {
            ParameterExpressions = new ParameterExpression[paramExpressions.Count + 1];
            ParameterExpressions[0] = invoker;
            for (var index = 0; index < paramExpressions.Count; index++)
            {
                ParameterExpressions[index + 1] = paramExpressions[index];
            }
            BodyExpression = bodyExpression;
        }
    }

    public abstract class FastActionFactory
    {
        protected Type T<T>()
        {
            return typeof(T);
        }


        private static FastActionFactory CreateActionFactory(Type invoker)
        {
            return (FastActionFactory)Activator.CreateInstance(typeof(FastActionFactory<>).MakeGenericType(invoker));
        }


        private static FastActionFactory CreateActionFactory(MethodInfo method, List<Type> genericTypes)
        {
            var types = genericTypes.ToArray();
            switch (method.GetParameters().Length)
            {
                case 0:
                    return (FastActionFactory)Activator.CreateInstance(typeof(FastActionFactory<>).MakeGenericType(types));
                case 1:
                    return (FastActionFactory)Activator.CreateInstance(typeof(FastActionFactory<,>).MakeGenericType(types));
                case 2:
                    return (FastActionFactory)Activator.CreateInstance(typeof(FastActionFactory<,,>).MakeGenericType(types));
                case 3:
                    return (FastActionFactory)Activator.CreateInstance(typeof(FastActionFactory<,,,>).MakeGenericType(types));
                case 4:
                    return (FastActionFactory)Activator.CreateInstance(typeof(FastActionFactory<,,,,>).MakeGenericType(types));
                case 5:
                    return (FastActionFactory)Activator.CreateInstance(typeof(FastActionFactory<,,,,,>).MakeGenericType(types));
                case 6:
                    return (FastActionFactory)Activator.CreateInstance(typeof(FastActionFactory<,,,,,,>).MakeGenericType(types));
                case 7:
                    return (FastActionFactory)Activator.CreateInstance(typeof(FastActionFactory<,,,,,,,>).MakeGenericType(types));
                case 8:
                    return (FastActionFactory)Activator.CreateInstance(typeof(FastActionFactory<,,,,,,,,>).MakeGenericType(types));
                case 9:
                    return (FastActionFactory)Activator.CreateInstance(typeof(FastActionFactory<,,,,,,,,,>).MakeGenericType(types));
                case 10:
                    return (FastActionFactory)Activator.CreateInstance(typeof(FastActionFactory<,,,,,,,,,,>).MakeGenericType(types));
                case 11:
                    return (FastActionFactory)Activator.CreateInstance(typeof(FastActionFactory<,,,,,,,,,,,>).MakeGenericType(types));
                case 12:
                    return (FastActionFactory)Activator.CreateInstance(typeof(FastActionFactory<,,,,,,,,,,,,>).MakeGenericType(types));
                case 13:
                    return (FastActionFactory)Activator.CreateInstance(typeof(FastActionFactory<,,,,,,,,,,,,,>).MakeGenericType(types));
                case 14:
                    return (FastActionFactory)Activator.CreateInstance(typeof(FastActionFactory<,,,,,,,,,,,,,,>).MakeGenericType(types));
                case 15:
                    return (FastActionFactory)Activator.CreateInstance(typeof(FastActionFactory<,,,,,,,,,,,,,,,>).MakeGenericType(types));
                default:
                    throw new ArgumentException($"{method} 参数数量 > 15");
            }
        }


        public static FastActionFactory CreateFactory(PropertyInfo property)
        {
            return CreateActionFactory(property.PropertyType);
        }


        public static FastActionFactory CreateFactory(MethodInfo method)
        {
            var parameters = method.GetParameters();
            var genericTypes = new List<Type> { method.DeclaringType };
            foreach (var param in parameters)
            {
                genericTypes.Add(param.ParameterType);
            }
            return CreateActionFactory(method, genericTypes);
        }


        public static IFastInvoker Invoker(PropertyInfo property)
        {
            return CreateFactory(property).CreateInvoker(property);
        }


        public static IFastInvoker Invoker(MethodInfo method)
        {
            return CreateFactory(method).CreateInvoker(method);
        }


        public static IFastInvoker Invoker(MethodInfo memberInfo, Type invokerType, params Type[] funParamTypes)
        {
            return CreateFactory(memberInfo).CreateInvoker(memberInfo, invokerType, funParamTypes);
        }



        private FastActionExpression CreateActionExpression(MemberInfo memberInfo, Type invokerType, params Type[] funParamTypes)
        {
            switch (memberInfo)
            {
                case PropertyInfo pro:
                    return CreatePropertyActionExpression(pro, invokerType);
                case MethodInfo method:
                    return CreateMethodActionExpression(method, invokerType, funParamTypes);
                default:
                    throw new CommonException($"不支持类型 ${memberInfo.GetType()} 代理");
            }
        }


        private FastActionExpression CreatePropertyActionExpression(PropertyInfo property, Type invokerType)
        {
            var invokerExpr = Expression.Parameter(invokerType, "invoker");
            var propertyType = property.PropertyType;
            var propertyExpr = Expression.Property(invokerExpr, property);
            if (propertyType == invokerType)
            {
                return new FastActionExpression(invokerExpr, new List<ParameterExpression>(), propertyExpr);
            }
            if (!propertyType.IsAssignableFrom(invokerType))
            {
                return new FastActionExpression(invokerExpr, new List<ParameterExpression>(), Expression.Convert(propertyExpr, invokerType));
            }
            throw new CommonException($"{property} 创建异常, 属性类型 ${propertyType} 与 Func 参数类型 ${invokerType} 不同");
        }


        private FastActionExpression CreateMethodActionExpression(MethodInfo method, Type invokerType, params Type[] funParamTypes)
        {
            var paramTypes = method.GetParameters();
            var invokerExpr = Expression.Parameter(invokerType, "invoker");
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
                    throw new CommonException($"{method} 创建异常, 第${index}个参数 method 参数类型 ${methodParamType} 与 Func 参数类型 ${funParamType} 不同");
                }
                var paramCastExpr = Expression.Convert(paramExpr, methodParamType);
                invokeParamExprList.Add(paramCastExpr);
            }
            var bodyExpr = Expression.Call(invokerExpr, method, invokeParamExprList);
            return new FastActionExpression(invokerExpr, paramExprList, bodyExpr);
        }


        protected TAction CreateAction<TAction>(MemberInfo method, Type invokerType, params Type[] funParamTypes)
        {
            var expressions = CreateActionExpression(method, invokerType, funParamTypes);
            return Expression.Lambda<TAction>(expressions.BodyExpression, expressions.ParameterExpressions).Compile();
        }



        public abstract IFastInvoker CreateInvoker(MemberInfo method, Type invokerType, params Type[] funParamTypes);


        public IFastInvoker CreateInvoker(MemberInfo memberInfo)
        {
            List<ParameterInfo> paramInfos;
            switch (memberInfo)
            {
                case PropertyInfo property:
                    paramInfos = new List<ParameterInfo>();
                    break;
                case MethodInfo method:
                    paramInfos = method.GetParameters().ToList();
                    break;
                default:
                    throw new CommonException($"不支持类型 ${memberInfo.GetType()} 代理");
            }
            return CreateInvoker(memberInfo, memberInfo.DeclaringType, paramInfos.Select(parameterInfo => parameterInfo.ParameterType).ToArray());
        }
    }


    public class FastActionFactory<TAction>
        : FastActionFactory
    {
        public Action<TAction> Create(MethodInfo method)
        {
            return CreateAction<Action<TAction>>(
                method, T<TAction>());
        }


        public override IFastInvoker CreateInvoker(MemberInfo method, Type invokerType, params Type[] funParamTypes)
        {
            return new FastAction<TAction>(CreateAction<Action<TAction>>(method, invokerType, funParamTypes));
        }
    }

    public class FastActionFactory<TAction, TP1>
        : FastActionFactory
    {
        public Action<TAction, TP1> Create(MethodInfo method)
        {
            return CreateAction<Action<TAction, TP1>>(
                method, T<TAction>(), T<TP1>());
        }


        public override IFastInvoker CreateInvoker(MemberInfo method, Type invokerType, params Type[] funParamTypes)
        {
            return new FastAction<TAction, TP1>(
                CreateAction<Action<TAction, TP1>>(
                    method, invokerType, funParamTypes));
        }
    }

    public class FastActionFactory<TAction, TP1, TP2>
        : FastActionFactory
    {
        public Action<TAction, TP1, TP2> Create(MethodInfo method)
        {
            return CreateAction<Action<TAction, TP1, TP2>>(
                method, T<TAction>(), T<TP1>(), T<TP2>());
        }


        public override IFastInvoker CreateInvoker(MemberInfo method, Type invokerType, params Type[] funParamTypes)
        {
            return new FastAction<TAction, TP1, TP2>(
                CreateAction<Action<TAction, TP1, TP2>>(
                    method, invokerType, funParamTypes));
        }
    }


    public class FastActionFactory<TAction, TP1, TP2, TP3> :
        FastActionFactory
    {
        public Action<TAction, TP1, TP2, TP3> Create(MethodInfo method)
        {
            return CreateAction<Action<TAction, TP1, TP2, TP3>>(
                method, T<TAction>(), T<TP1>(), T<TP2>(), T<TP3>());
        }


        public override IFastInvoker CreateInvoker(MemberInfo method, Type invokerType, params Type[] funParamTypes)
        {
            return new FastAction<TAction, TP1, TP2, TP3>(
                CreateAction<Action<TAction, TP1, TP2, TP3>>(
                    method, invokerType, funParamTypes));
        }
    }

    public class FastActionFactory<TAction, TP1, TP2, TP3, TP4> :
        FastActionFactory
    {
        public Action<TAction, TP1, TP2, TP3, TP4> Create(MethodInfo method)
        {
            return CreateAction<Action<TAction, TP1, TP2, TP3, TP4>>(
                method, T<TAction>(), T<TP1>(), T<TP2>(), T<TP3>(), T<TP4>());
        }


        public override IFastInvoker CreateInvoker(MemberInfo method, Type invokerType, params Type[] funParamTypes)
        {
            return new FastAction<TAction, TP1, TP2, TP3, TP4>(
                CreateAction<Action<TAction, TP1, TP2, TP3, TP4>>(
                    method, invokerType, funParamTypes));
        }
    }

    public class FastActionFactory<TAction, TP1, TP2, TP3, TP4, TP5> :
        FastActionFactory
    {
        public Action<TAction, TP1, TP2, TP3, TP4, TP5> Create(MethodInfo method)
        {
            return CreateAction<Action<TAction, TP1, TP2, TP3, TP4, TP5>>(
                method, T<TAction>(), T<TP1>(), T<TP2>(), T<TP3>(), T<TP4>(), T<TP5>());
        }


        public override IFastInvoker CreateInvoker(MemberInfo method, Type invokerType, params Type[] funParamTypes)
        {
            return new FastAction<TAction, TP1, TP2, TP3, TP4, TP5>(
                CreateAction<Action<TAction, TP1, TP2, TP3, TP4, TP5>>(
                    method, invokerType, funParamTypes));
        }
    }

    public class FastActionFactory<TAction, TP1, TP2, TP3, TP4, TP5, TP6>
        : FastActionFactory
    {
        public Action<TAction, TP1, TP2, TP3, TP4, TP5, TP6> Create(MethodInfo method)
        {
            return CreateAction<Action<TAction, TP1, TP2, TP3, TP4, TP5, TP6>>(
                method, T<TAction>(), T<TP1>(), T<TP2>(), T<TP3>(), T<TP4>(), T<TP5>(), T<TP6>());
        }


        public override IFastInvoker CreateInvoker(MemberInfo method, Type invokerType, params Type[] funParamTypes)
        {
            return new FastAction<TAction, TP1, TP2, TP3, TP4, TP5, TP6>(
                CreateAction<Action<TAction, TP1, TP2, TP3, TP4, TP5, TP6>>(
                    method, invokerType, funParamTypes));
        }
    }

    public class FastActionFactory<TAction, TP1, TP2, TP3, TP4, TP5, TP6, TP7>
        : FastActionFactory
    {
        public Action<TAction, TP1, TP2, TP3, TP4, TP5, TP6, TP7> Create(MethodInfo method)
        {
            return CreateAction<Action<TAction, TP1, TP2, TP3, TP4, TP5, TP6, TP7>>(
                method, T<TAction>(), T<TP1>(), T<TP2>(), T<TP3>(), T<TP4>(), T<TP5>(), T<TP6>(), T<TP7>());
        }


        public override IFastInvoker CreateInvoker(MemberInfo method, Type invokerType, params Type[] funParamTypes)
        {
            return new FastAction<TAction, TP1, TP2, TP3, TP4, TP5, TP6, TP7>(
                CreateAction<Action<TAction, TP1, TP2, TP3, TP4, TP5, TP6, TP7>>(
                    method, invokerType, funParamTypes));
        }
    }

    public class FastActionFactory<TAction, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8>
        : FastActionFactory
    {
        public Action<TAction, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8> Create(MethodInfo method)
        {
            return CreateAction<Action<TAction, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8>>(
                method, T<TAction>(), T<TP1>(), T<TP2>(), T<TP3>(), T<TP4>(), T<TP5>(), T<TP6>(), T<TP7>(), T<TP8>());
        }


        public override IFastInvoker CreateInvoker(MemberInfo method, Type invokerType, params Type[] funParamTypes)
        {
            return new FastAction<TAction, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8>(
                CreateAction<Action<TAction, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8>>(
                    method, invokerType, funParamTypes));
        }
    }

    public class FastActionFactory<TAction, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9>
        : FastActionFactory
    {
        public Action<TAction, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9> Create(MethodInfo method)
        {
            return CreateAction<Action<TAction, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9>>(
                method, T<TAction>(), T<TP1>(), T<TP2>(), T<TP3>(), T<TP4>(), T<TP5>(), T<TP6>(), T<TP7>(), T<TP8>(), T<TP9>());
        }


        public override IFastInvoker CreateInvoker(MemberInfo method, Type invokerType, params Type[] funParamTypes)
        {
            return new FastAction<TAction, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9>(
                CreateAction<Action<TAction, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9>>(
                    method, invokerType, funParamTypes));
        }
    }

    public class FastActionFactory<TAction, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TP10>
        : FastActionFactory
    {
        public Action<TAction, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TP10> Create(MethodInfo method)
        {
            return CreateAction<Action<TAction, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TP10>>(
                method, T<TAction>(), T<TP1>(), T<TP2>(), T<TP3>(), T<TP4>(), T<TP5>(), T<TP6>(), T<TP7>(), T<TP8>(), T<TP9>(), T<TP10>());
        }


        public override IFastInvoker CreateInvoker(MemberInfo method, Type invokerType, params Type[] funParamTypes)
        {
            return new FastAction<TAction, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TP10>(
                CreateAction<Action<TAction, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TP10>>(
                    method, invokerType, funParamTypes));
        }
    }

    public class FastActionFactory<TAction, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TP10, TP11>
        : FastActionFactory
    {
        public Action<TAction, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TP10, TP11> Create(MethodInfo method)
        {
            return CreateAction<Action<TAction, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TP10, TP11>>(
                method, T<TAction>(), T<TP1>(), T<TP2>(), T<TP3>(), T<TP4>(), T<TP5>(), T<TP6>(), T<TP7>(), T<TP8>(), T<TP9>(), T<TP10>(), T<TP11>());
        }


        public override IFastInvoker CreateInvoker(MemberInfo method, Type invokerType, params Type[] funParamTypes)
        {
            return new FastAction<TAction, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TP10, TP11>(
                CreateAction<Action<TAction, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TP10, TP11>>(
                    method, invokerType, funParamTypes));
        }
    }

    public class FastActionFactory<TAction, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TP10, TP11, TP12>
        : FastActionFactory
    {
        public Action<TAction, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TP10, TP11, TP12> Create(MethodInfo method)
        {
            return CreateAction<Action<TAction, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TP10, TP11, TP12>>(
                method, T<TAction>(), T<TP1>(), T<TP2>(), T<TP3>(), T<TP4>(), T<TP5>(), T<TP6>(), T<TP7>(), T<TP8>(), T<TP9>(), T<TP10>(), T<TP11>(),
                T<TP12>());
        }


        public override IFastInvoker CreateInvoker(MemberInfo method, Type invokerType, params Type[] funParamTypes)
        {
            return new FastAction<TAction, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TP10, TP11, TP12>(
                CreateAction<Action<TAction, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TP10, TP11, TP12>>(
                    method, invokerType, funParamTypes));
        }
    }

    public class FastActionFactory<TAction, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TP10, TP11, TP12, TP13>
        : FastActionFactory
    {
        public Action<TAction, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TP10, TP11, TP12, TP13> Create(MethodInfo method)
        {
            return CreateAction<Action<TAction, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TP10, TP11, TP12, TP13>>(
                method, T<TAction>(), T<TP1>(), T<TP2>(), T<TP3>(), T<TP4>(), T<TP5>(), T<TP6>(), T<TP7>(), T<TP8>(), T<TP9>(), T<TP10>(), T<TP11>(),
                T<TP12>(), T<TP13>());
        }


        public override IFastInvoker CreateInvoker(MemberInfo method, Type invokerType, params Type[] funParamTypes)
        {
            return new FastAction<TAction, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TP10, TP11, TP12, TP13>(
                CreateAction<Action<TAction, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TP10, TP11, TP12, TP13>>(
                    method, invokerType, funParamTypes));
        }
    }

    public class FastActionFactory<TAction, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TP10, TP11, TP12, TP13, TP14>
        : FastActionFactory
    {
        public Action<TAction, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TP10, TP11, TP12, TP13, TP14> Create(MethodInfo method)
        {
            return CreateAction<Action<TAction, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TP10, TP11, TP12, TP13, TP14>>(
                method, T<TAction>(), T<TP1>(), T<TP2>(), T<TP3>(), T<TP4>(), T<TP5>(), T<TP6>(), T<TP7>(), T<TP8>(), T<TP9>(), T<TP10>(), T<TP11>(),
                T<TP12>(), T<TP13>(), T<TP14>());
        }


        public override IFastInvoker CreateInvoker(MemberInfo method, Type invokerType, params Type[] funParamTypes)
        {
            return new FastAction<TAction, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TP10, TP11, TP12, TP13, TP14>(
                CreateAction<Action<TAction, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TP10, TP11, TP12, TP13, TP14>>(
                    method, invokerType, funParamTypes));
        }
    }

    public class FastActionFactory<TAction, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TP10, TP11, TP12, TP13, TP14, TP15>
        : FastActionFactory
    {
        public Action<TAction, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TP10, TP11, TP12, TP13, TP14, TP15> Create(MethodInfo method)
        {
            return CreateAction<Action<TAction, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TP10, TP11, TP12, TP13, TP14, TP15>>(
                method, T<TAction>(), T<TP1>(), T<TP2>(), T<TP3>(), T<TP4>(), T<TP5>(), T<TP6>(), T<TP7>(), T<TP8>(), T<TP9>(), T<TP10>(), T<TP11>(),
                T<TP12>(), T<TP13>(), T<TP14>(), T<TP15>());
        }


        public override IFastInvoker CreateInvoker(MemberInfo method, Type invokerType, params Type[] funParamTypes)
        {
            return new FastAction<TAction, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TP10, TP11, TP12, TP13, TP14, TP15>(
                CreateAction<Action<TAction, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TP10, TP11, TP12, TP13, TP14, TP15>>(
                    method, invokerType, funParamTypes));
        }
    }
}
