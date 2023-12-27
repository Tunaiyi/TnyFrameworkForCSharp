// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;

namespace TnyFramework.Common.Event.Buses
{

    internal class EventBus<THandler> : IEventBus<THandler>
        where THandler : Delegate
    {
        private static readonly IList<Type> HANDLER_METHOD_PARAMETER_TYPES;
        private static readonly PropertyInfo HANDLER_PROPERTY;
        private static readonly PropertyInfo PARENT_NOTIFY_PROPERTY;
        private static readonly MethodInfo HANDLER_INVOKER_METHOD;

        private static readonly Func<EventBus<THandler>, THandler> NOTIFIER_FACTORY;

        private THandler? eventHandler;

        private readonly EventBus<THandler>? parent;

        public THandler Notify { get; }

        public THandler? Handler => eventHandler;

        public IEventBox<THandler>? Parent => parent;

        public THandler? ParentNotify => parent?.Notify;

        static EventBus()
        {

            var handlerType = typeof(THandler);
            var handlerMethod = handlerType.GetMethod("Invoke");
            HANDLER_INVOKER_METHOD = handlerMethod!;

            var types = handlerMethod?.GetParameters().Select(t => t.ParameterType).ToList() ?? new List<Type>();
            HANDLER_METHOD_PARAMETER_TYPES = types.ToImmutableList();

            var busType = typeof(EventBus<THandler>);
            HANDLER_PROPERTY = busType.GetRuntimeProperty(nameof(Handler))!;
            PARENT_NOTIFY_PROPERTY = busType.GetRuntimeProperty(nameof(ParentNotify))!;
            NOTIFIER_FACTORY = CreateNotifierFactory();
        }

        private static Func<EventBus<THandler>, THandler> CreateNotifierFactory()
        {
            var busParameterExpr = Expression.Parameter(typeof(EventBus<THandler>), "bus");
            var handlerParameterExpressions = new List<ParameterExpression>();
            var index = 0;
            foreach (var parameterType in HANDLER_METHOD_PARAMETER_TYPES)
            {
                handlerParameterExpressions.Add(Expression.Parameter(parameterType, $"p{index}"));
                index++;
            }

            var handlerType = typeof(THandler);
            var handlerExpr = Expression.Variable(handlerType, "handler");
            var parentExpr = Expression.Variable(handlerType, "parent");

            // 调用  EventHandler
            var getHandlerExpr = Expression.Property(busParameterExpr, HANDLER_PROPERTY);
            Expression assignHandlerExpr = Expression.Assign(handlerExpr, getHandlerExpr);
            Expression invokeHandlerExpr = Expression.Call(handlerExpr, HANDLER_INVOKER_METHOD, handlerParameterExpressions);
            Expression constNullHandlerExpr = Expression.Constant(null, handlerType);
            Expression checkHandlerNullExpr = Expression.NotEqual(handlerExpr, constNullHandlerExpr);
            Expression checkAndInvokeHandlerExpr = Expression.IfThen(checkHandlerNullExpr, invokeHandlerExpr);

            // 调用  ParentNotify
            var getParentExpr = Expression.Property(busParameterExpr, PARENT_NOTIFY_PROPERTY);
            Expression assignParentExpr = Expression.Assign(parentExpr, getParentExpr);
            Expression invokeParentExpr = Expression.Call(parentExpr, HANDLER_INVOKER_METHOD, handlerParameterExpressions);
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

        public EventBus()
        {
            Notify = NOTIFIER_FACTORY.Invoke(this);
        }

        public EventBus(EventBus<THandler> parent)
        {
            this.parent = parent;
            Notify = NOTIFIER_FACTORY.Invoke(this);
        }

        public IEventBus<THandler> ForkChild()
        {
            return new EventBus<THandler>(this);
        }

        public void Add(THandler handler)
        {
            var current = eventHandler;
            while (true)
            {
                var check = current;
                var combined = (THandler) Delegate.Combine(current, handler);
                current = Interlocked.CompareExchange(ref eventHandler, combined, check);
                if (current == check)
                {
                    break;
                }
            }
        }

        public void Add(IEnumerable<THandler> handler)
        {
            foreach (var tHandler in handler)
            {
                Add(tHandler);
            }
        }

        public void Add(object onNodeCreate, params THandler[] handler)
        {
            foreach (var tHandler in handler)
            {
                Add(tHandler);
            }
        }

        public void Remove(THandler handler)
        {
            var current = eventHandler;
            while (true)
            {
                var check = current;
                var removed = (THandler) Delegate.Remove(check, handler)!;
                current = Interlocked.CompareExchange(ref eventHandler, removed, check);
                if (current == check)
                {
                    break;
                }
            }
        }

        public void Remove(IEnumerable<THandler> handler)
        {
            foreach (var tHandler in handler)
            {
                Remove(tHandler);
            }
        }

        public void Remove(params THandler[] handler)
        {
            foreach (var tHandler in handler)
            {
                Remove(tHandler);
            }
        }

        public void Clear()
        {
            var current = eventHandler;
            while (true)
            {
                var check = current;
                current = Interlocked.CompareExchange(ref eventHandler, null, check);
                if (current == check)
                {
                    break;
                }
            }
        }
    }

}
