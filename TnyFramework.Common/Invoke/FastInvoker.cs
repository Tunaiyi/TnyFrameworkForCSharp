using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using TnyFramework.Common.Exception;
namespace TnyFramework.Common.Invoke
{
    public class InvokerExpressions
    {
        public ParameterExpression[] ParameterExpressions { get; }

        public Expression BodyExpression { get; }


        public InvokerExpressions(ParameterExpression invoker, IReadOnlyList<ParameterExpression> paramExpressions, Expression bodyExpression)
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

    public class BaseFastInvokerFactory<TFunc>
    {
        protected Type T<T>()
        {
            return typeof(T);
        }


        protected InvokerExpressions CreateInvokeExpression(MethodInfo method, Type funInvokeType, params Type[] funParamTypes)
        {
            var paramTypes = method.GetParameters();
            var invokerExpr = Expression.Parameter(funInvokeType, "invoker");
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
            return new InvokerExpressions(invokerExpr, paramExprList, bodyExpr);
        }


        protected TFunc Create(MethodInfo method, Type funInvokeType, params Type[] funParamTypes)
        {
            var expressions = CreateInvokeExpression(method, funInvokeType, funParamTypes);
            return Expression.Lambda<TFunc>(expressions.BodyExpression, expressions.ParameterExpressions).Compile();
        }
    }


    public class FastInvokerFactory<TInvoker, TResult>
        : BaseFastInvokerFactory<Func<TInvoker, TResult>>
    {
        public Func<TInvoker, TResult> Create(MethodInfo method)
        {
            return Create(method, T<TInvoker>());
        }
    }

    public class FastInvokerFactory<TInvoker, TP1, TResult>
        : BaseFastInvokerFactory<Func<TInvoker, TP1, TResult>>
    {
        public Func<TInvoker, TP1, TResult> Create(MethodInfo method)
        {
            return Create(method, T<TInvoker>(), T<TP1>());
        }
    }

    public class FastInvokerFactory<TInvoker, TP1, TP2, TResult>
        : BaseFastInvokerFactory<Func<TInvoker, TP1, TP2, TResult>>
    {
        public Func<TInvoker, TP1, TP2, TResult> Create(MethodInfo method)
        {
            return Create(method, T<TInvoker>(), T<TP1>(), T<TP2>());
        }
    }


    public class FastInvokerFactory<TInvoker, TP1, TP2, TP3, TResult> :
        BaseFastInvokerFactory<Func<TInvoker, TP1, TP2, TP3, TResult>>
    {
        public Func<TInvoker, TP1, TP2, TP3, TResult> Create(MethodInfo method)
        {
            return Create(method, T<TInvoker>(), T<TP1>(), T<TP2>(), T<TP3>());
        }
    }

    public class FastInvokerFactory<TInvoker, TP1, TP2, TP3, TP4, TResult> :
        BaseFastInvokerFactory<Func<TInvoker, TP1, TP2, TP3, TP4, TResult>>
    {
        public Func<TInvoker, TP1, TP2, TP3, TP4, TResult> Create(MethodInfo method)
        {
            return Create(method, T<TInvoker>(), T<TP1>(), T<TP2>(), T<TP3>(), T<TP4>());
        }
    }

    public class FastInvokerFactory<TInvoker, TP1, TP2, TP3, TP4, TP5, TResult> :
        BaseFastInvokerFactory<Func<TInvoker, TP1, TP2, TP3, TP4, TP5, TResult>>
    {
        public Func<TInvoker, TP1, TP2, TP3, TP4, TP5, TResult> Create(MethodInfo method)
        {
            return Create(method, T<TInvoker>(), T<TP1>(), T<TP2>(), T<TP3>(), T<TP4>(), T<TP5>());
        }
    }

    public class FastInvokerFactory<TInvoker, TP1, TP2, TP3, TP4, TP5, TP6, TResult>
        : BaseFastInvokerFactory<Func<TInvoker, TP1, TP2, TP3, TP4, TP5, TP6, TResult>>
    {
        public Func<TInvoker, TP1, TP2, TP3, TP4, TP5, TP6, TResult> Create(MethodInfo method)
        {
            return Create(method, T<TInvoker>(), T<TP1>(), T<TP2>(), T<TP3>(), T<TP4>(), T<TP5>(), T<TP6>());
        }
    }

    public class FastInvokerFactory<TInvoker, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TResult>
        : BaseFastInvokerFactory<Func<TInvoker, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TResult>>
    {
        public Func<TInvoker, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TResult> Create(MethodInfo method)
        {
            return Create(method, T<TInvoker>(), T<TP1>(), T<TP2>(), T<TP3>(), T<TP4>(), T<TP5>(), T<TP6>(), T<TP7>());
        }
    }

    public class FastInvokerFactory<TInvoker, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TResult>
        : BaseFastInvokerFactory<Func<TInvoker, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TResult>>
    {
        public Func<TInvoker, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TResult> Create(MethodInfo method)
        {
            return Create(method, T<TInvoker>(), T<TP1>(), T<TP2>(), T<TP3>(), T<TP4>(), T<TP5>(), T<TP6>(), T<TP7>(), T<TP8>());
        }
    }

    public class FastInvokerFactory<TInvoker, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TResult>
        : BaseFastInvokerFactory<Func<TInvoker, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TResult>>
    {
        public Func<TInvoker, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TResult> Create(MethodInfo method)
        {
            return Create(method, T<TInvoker>(), T<TP1>(), T<TP2>(), T<TP3>(), T<TP4>(), T<TP5>(), T<TP6>(), T<TP7>(), T<TP8>(), T<TP9>());
        }
    }

    public class FastInvokerFactory<TInvoker, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TP10, TResult>
        : BaseFastInvokerFactory<Func<TInvoker, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TP10, TResult>>
    {
        public Func<TInvoker, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TP10, TResult> Create(MethodInfo method)
        {
            return Create(method, T<TInvoker>(), T<TP1>(), T<TP2>(), T<TP3>(), T<TP4>(), T<TP5>(), T<TP6>(), T<TP7>(), T<TP8>(), T<TP9>(), T<TP10>());
        }
    }

    public class FastInvokerFactory<TInvoker, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TP10, TP11, TResult>
        : BaseFastInvokerFactory<Func<TInvoker, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TP10, TP11, TResult>>
    {
        public Func<TInvoker, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TP10, TP11, TResult> Create(MethodInfo method)
        {
            return Create(method, T<TInvoker>(), T<TP1>(), T<TP2>(), T<TP3>(), T<TP4>(), T<TP5>(), T<TP6>(), T<TP7>(), T<TP8>(), T<TP9>(), T<TP10>(),
                T<TP11>());
        }
    }

    public class FastInvokerFactory<TInvoker, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TP10, TP11, TP12, TResult>
        : BaseFastInvokerFactory<Func<TInvoker, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TP10, TP11, TP12, TResult>>
    {
        public Func<TInvoker, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TP10, TP11, TP12, TResult> Create(MethodInfo method)
        {
            return Create(method, T<TInvoker>(), T<TP1>(), T<TP2>(), T<TP3>(), T<TP4>(), T<TP5>(), T<TP6>(), T<TP7>(), T<TP8>(), T<TP9>(), T<TP10>(),
                T<TP11>(), T<TP12>());
        }
    }

    public class FastInvokerFactory<TInvoker, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TP10, TP11, TP12, TP13, TResult>
        : BaseFastInvokerFactory<Func<TInvoker, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TP10, TP11, TP12, TP13, TResult>>
    {
        public Func<TInvoker, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TP10, TP11, TP12, TP13, TResult> Create(MethodInfo method)
        {
            return Create(method, T<TInvoker>(), T<TP1>(), T<TP2>(), T<TP3>(), T<TP4>(), T<TP5>(), T<TP6>(), T<TP7>(), T<TP8>(), T<TP9>(), T<TP10>(),
                T<TP11>(), T<TP12>(), T<TP13>());
        }
    }

    public class FastInvokerFactory<TInvoker, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TP10, TP11, TP12, TP13, TP14, TResult>
        : BaseFastInvokerFactory<Func<TInvoker, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TP10, TP11, TP12, TP13, TP14, TResult>>
    {
        public Func<TInvoker, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TP10, TP11, TP12, TP13, TP14, TResult> Create(MethodInfo method)
        {
            return Create(method, T<TInvoker>(), T<TP1>(), T<TP2>(), T<TP3>(), T<TP4>(), T<TP5>(), T<TP6>(), T<TP7>(), T<TP8>(), T<TP9>(), T<TP10>(),
                T<TP11>(), T<TP12>(), T<TP13>(), T<TP14>());
        }
    }

    public class FastInvokerFactory<TInvoker, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TP10, TP11, TP12, TP13, TP14, TP15, TResult>
        : BaseFastInvokerFactory<Func<TInvoker, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TP10, TP11, TP12, TP13, TP14, TP15, TResult>>
    {
        public Func<TInvoker, TP1, TP2, TP3, TP4, TP5, TP6, TP7, TP8, TP9, TP10, TP11, TP12, TP13, TP14, TP15, TResult> Create(MethodInfo method)
        {
            return Create(method, T<TInvoker>(), T<TP1>(), T<TP2>(), T<TP3>(), T<TP4>(), T<TP5>(), T<TP6>(), T<TP7>(), T<TP8>(), T<TP9>(), T<TP10>(),
                T<TP11>(), T<TP12>(), T<TP13>(), T<TP14>(), T<TP15>());
        }
    }
}
