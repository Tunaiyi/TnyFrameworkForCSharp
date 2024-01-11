// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;

namespace TnyFramework.Common.EventBus;

/// <summary>
/// 事件总线静态类
/// </summary>
public static class EventBuses
{
    public static IEventBus<THandler> Create<THandler>()
        where THandler : Delegate
    {
        return new EventBus<THandler>();
    }

    public static IEventBus<T> Event<T>(this object invoker, ref IEventBus<T>? eventBus) where T : Delegate
    {
        if (eventBus != null)
        {
            return eventBus;
        }
        lock (invoker)
        {
            {
                eventBus = new EventBus<T>();
            }
        }
        return eventBus;
    }

    public static IEventBus<T> ForkEvent<T>(this object invoker, IEventBus<T> parentBus, ref IEventBus<T>? eventBus)
        where T : Delegate
    {
        if (eventBus != null)
        {
            return eventBus;
        }
        lock (invoker)
        {
            {
                eventBus = parentBus.ForkChild();
            }
        }
        return eventBus;
    }

    public static IEventBus<THandler> Create<THandler>(out IEventBus<THandler> eventBus)
        where THandler : Delegate
    {
        var bus = new EventBus<THandler>();
        eventBus = bus;
        return bus;
    }
}
