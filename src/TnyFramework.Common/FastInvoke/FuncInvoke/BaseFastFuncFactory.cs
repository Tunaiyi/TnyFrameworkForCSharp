using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using TnyFramework.Common.Exceptions;

namespace TnyFramework.Common.FastInvoke.FuncInvoke
{

    public abstract class BaseFastFuncFactory<TInvoker, TFastInvoker>
        : IFastFuncFactory
        where TFastInvoker : BaseFastFunc<TInvoker>, new()
    {
        private readonly Type invokerType;

        private readonly Type[] paramTypes;

        private readonly Type returnType;

        protected BaseFastFuncFactory()
        {
            var currentType = GetType();
            var genericArguments = currentType.GetGenericArguments();
            paramTypes = new Type[genericArguments.Length - 2];
            for (var index = 0; index < genericArguments.Length; index++)
            {
                var type = genericArguments[index];
                if (index == 0)
                {
                    invokerType = type;
                } else if (index < genericArguments.Length - 1)
                {
                    paramTypes[index - 1] = type;
                } else
                {
                    returnType = type;
                }
            }
        }

        private TInvoker Create(MemberInfo member)
        {
            var expressions = CreateFuncExpression(member);
            return Expression.Lambda<TInvoker>(expressions.BodyExpression, expressions.ParameterExpressions).Compile();
        }

        public IFastInvoker CreateInvoker(FieldInfo field)
        {
            return NewInvoker(field);
        }

        public IFastInvoker CreateInvoker(PropertyInfo property)
        {
            return NewInvoker(property);
        }

        public IFastInvoker CreateInvoker(MethodInfo method)
        {
            return NewInvoker(method);
        }

        public IFastInvoker CreateInvoker(ConstructorInfo constructor)
        {
            return NewInvoker(constructor);
        }

        private IFastInvoker NewInvoker(MemberInfo member)
        {
            var invoker = new TFastInvoker {
                Func = Create(member)
            };
            return invoker;
        }

        private FastFuncExpression CreateFuncExpression(MemberInfo memberInfo)
        {
            switch (memberInfo)
            {
                case ConstructorInfo constructor:
                    return CreateConstructorFuncExpression(constructor);
                case FieldInfo field:
                    return CreatePropertyOrFieldGetterExpression(field);
                case PropertyInfo property:
                    return CreatePropertyOrFieldGetterExpression(property);
                case MethodInfo method:
                    return CreateMethodFuncExpression(method);
                default:
                    throw new CommonException($"不支持类型 ${memberInfo.GetType()} 代理");
            }
        }

        private FastFuncExpression CreatePropertyOrFieldGetterExpression(MemberInfo member)
        {
            var invokerExpr = Expression.Parameter(invokerType, "invoker");
            Type memberType;
            MemberExpression memberExpr;
            switch (member)
            {
                case PropertyInfo property:
                    memberType = property.PropertyType;
                    memberExpr = Expression.Property(property.GetMethod.IsStatic ? null : invokerExpr, property);
                    break;
                case FieldInfo field:
                    memberType = field.FieldType;
                    memberExpr = Expression.Field(field.IsStatic ? null : invokerExpr, field);
                    break;
                default:
                    throw new CommonException($"CreatePropertyOrFieldSetExpression 无法处理 {member} 成员");
            }
            if (memberType == returnType)
            {
                return new FastFuncExpression(invokerExpr, new List<ParameterExpression>(), memberExpr);
            }
            if (!memberType.IsAssignableFrom(returnType))
            {
                return new FastFuncExpression(invokerExpr, new List<ParameterExpression>(), Expression.Convert(memberExpr, returnType));
            }
            throw new CommonException($"{member} 创建异常, 属性类型 ${memberType} 与 Func 参数类型 ${returnType} 不同");
        }

        private FastFuncExpression CreateConstructorFuncExpression(ConstructorInfo constructor)
        {
            var constParams = constructor.GetParameters();
            var paramExprList = new List<ParameterExpression>(); // 申明参数
            var invokeParamExprList = new List<Expression>(); // 调用传参

            var nullInvokerExpr = Expression.Parameter(typeof(object), $"nullInvoker");
            paramExprList.Add(nullInvokerExpr);

            for (var index = 0; index < constParams.Length; index++)
            {
                var param = constParams[index];
                var funParamType = paramTypes[index];
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

        private FastFuncExpression CreateMethodFuncExpression(MethodInfo method)
        {
            var methodParams = method.GetParameters();

            var hasInvoker = !method.IsStatic;
            var invokerExpr = Expression.Parameter(hasInvoker ? invokerType : typeof(object), "invoker");
            var paramExprList = new List<ParameterExpression>(); // 申明参数
            var invokeParamExprList = new List<Expression>(); // 调用传参
            for (var index = 0; index < methodParams.Length; index++)
            {
                var param = methodParams[index];
                var funParamType = paramTypes[index];
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
            var bodyExpr = hasInvoker
                ? Expression.Call(invokerExpr, method, invokeParamExprList)
                : Expression.Call(method, invokeParamExprList);
            return new FastFuncExpression(invokerExpr, paramExprList, bodyExpr);
        }
    }

}
