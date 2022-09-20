// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using TnyFramework.Common.Exceptions;

namespace TnyFramework.Common.FastInvoke.ActionInvoke
{

    public class BaseFastActionFactory<TInvoker, TFastInvoker>
        : IFastActionFactory
        where TFastInvoker : BaseFastAction<TInvoker>, new()
    {
        private readonly Type invokerType;

        private readonly Type[] paramTypes;

        protected BaseFastActionFactory()
        {
            var currentType = GetType();
            var genericArguments = currentType.GetGenericArguments();
            paramTypes = new Type[genericArguments.Length - 1];
            for (var index = 0; index < genericArguments.Length; index++)
            {
                var type = genericArguments[index];
                if (index == 0)
                {
                    invokerType = type;
                } else
                {
                    paramTypes[index - 1] = type;
                }
            }
        }

        private TInvoker Create(MemberInfo member)
        {
            var expressions = CreateActionExpression(member);
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
                Action = Create(member)
            };
            return invoker;
        }

        private FastActionExpression CreateActionExpression(MemberInfo memberInfo)
        {
            switch (memberInfo)
            {
                case PropertyInfo property:
                    return CreatePropertyOrFieldSetterExpression(property);
                case FieldInfo field:
                    return CreatePropertyOrFieldSetterExpression(field);
                case MethodInfo method:
                    return CreateMethodActionExpression(method);
                default:
                    throw new CommonException($"不支持类型 ${memberInfo.GetType()} 代理");
            }
        }

        private FastActionExpression CreatePropertyOrFieldSetterExpression(MemberInfo member)
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
            var valueExpr = Expression.Parameter(paramTypes[0], "value");
            var paramExprList = new List<ParameterExpression> {valueExpr};
            Expression bodyExpr = null;
            if (memberType == paramTypes[0])
            {
                bodyExpr = Expression.Assign(memberExpr, valueExpr);
            } else if (!memberType.IsAssignableFrom(paramTypes[0]))
            {
                bodyExpr = Expression.Assign(memberExpr, Expression.Convert(valueExpr, memberType));
            }
            if (bodyExpr != null)
            {
                return new FastActionExpression(invokerExpr, paramExprList, bodyExpr);
            }
            throw new CommonException($"{member} 创建异常, 属性类型 ${memberType} 与 Func 参数类型 ${paramTypes[0]} 不同");
        }

        private FastActionExpression CreateMethodActionExpression(MethodInfo method)
        {
            var methodParams = method.GetParameters();
            var invokerExpr = Expression.Parameter(invokerType, "invoker");
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
                    throw new CommonException($"{method} 创建异常, 第${index}个参数 method 参数类型 ${methodParamType} 与 Func 参数类型 ${funParamType} 不同");
                }
                var paramCastExpr = Expression.Convert(paramExpr, methodParamType);
                invokeParamExprList.Add(paramCastExpr);
            }
            var bodyExpr = Expression.Call(invokerExpr, method, invokeParamExprList);
            return new FastActionExpression(invokerExpr, paramExprList, bodyExpr);
        }
    }

}
