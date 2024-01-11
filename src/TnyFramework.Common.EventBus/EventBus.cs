// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.Collections.Generic;
using System.Threading;
using TnyFramework.Common.Event;

namespace TnyFramework.Common.EventBus;

internal class EventBus<THandler> : IEventBus<THandler>
    where THandler : Delegate
{
    private static HandlerProfile<THandler> HandlerProfile { get; } = new();

    private THandler? eventHandler;

    private readonly EventBus<THandler>? parent;

    public THandler Notify { get; }

    public THandler? Handler => eventHandler;

    public THandler? ParentNotify => parent?.Notify;

    public EventBus()
    {
        Notify = HandlerProfile.CreateHandler(this);
    }

    private EventBus(EventBus<THandler> parent)
    {
        this.parent = parent;
        Notify = HandlerProfile.CreateHandler(this);
    }

    IEvent IEvent.ForkChild()
    {
        return ForkChild();
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

    public void Add(params THandler[] handler)
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

    public void Dispose()
    {
        Clear();
    }
}
