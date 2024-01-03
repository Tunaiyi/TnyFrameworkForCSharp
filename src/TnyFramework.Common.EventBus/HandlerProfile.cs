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
using System.Linq.Expressions;
using System.Reflection;

namespace TnyFramework.Common.EventBus;

internal class HandlerProfile<THandler> where THandler : Delegate
{
    private readonly Func<EventBus<THandler>, THandler> notifierFactory;

    public HandlerProfile()
    {
        var handlerType = typeof(THandler);
        var handlerMethod = handlerType.GetMethod("Invoke");
        var handlerInvokerMethod = handlerMethod!;

        var types = handlerMethod?.GetParameters().Select(t => t.ParameterType).ToList() ?? new List<Type>();
        IList<Type> handlerMethodParameterTypes = types;

        var busType = typeof(EventBus<THandler>);
        var handlerProperty = busType.GetRuntimeProperty(nameof(EventBus<THandler>.Handler))!;
        var parentProperty = busType.GetRuntimeProperty(nameof(EventBus<THandler>.ParentNotify))!;
        notifierFactory = CreateNotifierFactory(handlerMethodParameterTypes, handlerProperty, parentProperty, handlerInvokerMethod);
    }

    private Func<EventBus<THandler>, THandler> CreateNotifierFactory(
        IEnumerable<Type> handlerMethodParameterTypes,
        PropertyInfo handlerProperty,
        PropertyInfo parentProperty,
        MethodInfo handlerInvokerMethod
    )
    {
        var busParameterExpr = Expression.Parameter(typeof(EventBus<THandler>), "bus");
        var handlerParameterExpressions = new List<ParameterExpression>();
        var index = 0;
        foreach (var parameterType in handlerMethodParameterTypes)
        {
            handlerParameterExpressions.Add(Expression.Parameter(parameterType, $"p{index}"));
            index++;
        }

        var handlerType = typeof(THandler);
        var handlerExpr = Expression.Variable(handlerType, "handler");
        var parentExpr = Expression.Variable(handlerType, "parent");

        // 调用  EventHandler
        var getHandlerExpr = Expression.Property(busParameterExpr, handlerProperty);
        Expression assignHandlerExpr = Expression.Assign(handlerExpr, getHandlerExpr);
        Expression invokeHandlerExpr = Expression.Call(handlerExpr, handlerInvokerMethod, handlerParameterExpressions);
        Expression constNullHandlerExpr = Expression.Constant(null, handlerType);
        Expression checkHandlerNullExpr = Expression.NotEqual(handlerExpr, constNullHandlerExpr);
        Expression checkAndInvokeHandlerExpr = Expression.IfThen(checkHandlerNullExpr, invokeHandlerExpr);

        // 调用  ParentNotify
        var getParentExpr = Expression.Property(busParameterExpr, parentProperty);
        Expression assignParentExpr = Expression.Assign(parentExpr, getParentExpr);
        Expression invokeParentExpr = Expression.Call(parentExpr, handlerInvokerMethod, handlerParameterExpressions);
        Expression checkParentNullExpr = Expression.NotEqual(parentExpr, constNullHandlerExpr);
        Expression checkAndInvokeParentExpr = Expression.IfThen(checkParentNullExpr, invokeParentExpr);

        var notifierLambdaExpr = Expression.Lambda<THandler>(
            Expression.Block(new[] {handlerExpr, parentExpr},
                assignHandlerExpr,
                checkAndInvokeHandlerExpr,
                assignParentExpr,
                checkAndInvokeParentExpr
            ),
            handlerParameterExpressions);
        return Expression.Lambda<Func<EventBus<THandler>, THandler>>(notifierLambdaExpr, busParameterExpr).Compile();
    }

    public THandler CreateHandler(EventBus<THandler> bus)
    {
        return notifierFactory.Invoke(bus);
    }
}
