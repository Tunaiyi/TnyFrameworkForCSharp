// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.Collections.Generic;
using TnyFramework.Common.Exceptions;
using TnyFramework.Common.Extensions;

namespace TnyFramework.Common.Lifecycle;

public abstract class Lifecycle : IComparable<Lifecycle>
{
    private static readonly Dictionary<LifecycleStage, Dictionary<Type, Lifecycle>> LIFECYCLES_MAP;

    public abstract Type HandlerType { get; }

    public abstract int Order { get; }

    public abstract Lifecycle GetHead<T>() where T : Lifecycle;

    public abstract Lifecycle GetTail<T>() where T : Lifecycle;

    public abstract Lifecycle? GetPrev<T>() where T : Lifecycle;

    public abstract Lifecycle? GetNext<T>() where T : Lifecycle;

    static Lifecycle()
    {
        var map = new Dictionary<LifecycleStage, Dictionary<Type, Lifecycle>>();
        foreach (var value in LifecycleStage.GetValues())
        {
            map[value] = new Dictionary<Type, Lifecycle>();
        }
        LIFECYCLES_MAP = map;
    }

    internal static TLifecycle PutLifecycle<TLifecycle>(LifecycleStage<TLifecycle> stage, TLifecycle lifecycle)
        where TLifecycle : Lifecycle
    {
        var dictionary = LIFECYCLES_MAP[stage];
        var old = dictionary.PutIfAbsent(lifecycle.HandlerType, lifecycle);
        if (old != null)
        {
            throw new IllegalArgumentException($"{lifecycle.HandlerType} 已经存在 {old}, 无法添加 {lifecycle}");
        }
        return lifecycle;
    }

    internal static TLifecycle? GetLifecycle<TLifecycle>(LifecycleStage<TLifecycle> stage, Type type)
        where TLifecycle : Lifecycle
    {
        var dictionary = LIFECYCLES_MAP[stage];
        return (TLifecycle?) dictionary.Get(type);
    }

    internal static TLifecycle? GetLifecycle<TLifecycle, THandler>(LifecycleStage<TLifecycle> stage)
        where TLifecycle : Lifecycle
        where THandler : ILifecycleHandler
    {
        var dictionary = LIFECYCLES_MAP[stage];
        return (TLifecycle?) dictionary.Get(typeof(THandler));
    }

    public int CompareTo(Lifecycle? other)
    {
        if (other == null)
        {
            return 1;
        }
        var value = other.Order - Order;
        return value == 0 ? string.Compare(HandlerType.Name, other.HandlerType.Name, StringComparison.Ordinal) : value;
    }
}

public abstract class Lifecycle<TLife, THandler> : Lifecycle
    where TLife : Lifecycle<TLife, THandler>, new()
    where THandler : ILifecycleHandler
{
    private Type handlerType = null!;

    public Type lifeType = null!;

    private ILifecyclePriority Priority { get; set; } = null!;

    public TLife? Next { get; private set; }

    public TLife? Prev { get; private set; }

    public override Type HandlerType => handlerType;

    public override int Order => Priority.Order;

    public TLife HeadLifecycle {
        get {
            if (Prev == null)
            {
                if (this is TLife life)
                {
                    return life;
                }
                return null!;
            }
            return Prev.HeadLifecycle;
        }
    }

    public TLife TailLifecycle {
        get {
            if (Next == null)
            {
                if (this is TLife life)
                {
                    return life;
                }
                return null!;
            }
            return Next.TailLifecycle;
        }
    }

    public override Lifecycle GetHead<T>() => HeadLifecycle;

    public override Lifecycle GetTail<T>() => TailLifecycle;

    public override Lifecycle? GetPrev<T>() => Prev;

    public override Lifecycle? GetNext<T>() => Next;

    public TLife Append(TLife life)
    {
        if (Next != null)
        {
            throw new IllegalArgumentException($"{this} next is exist {Next}");
        }
        if (life.Order > Order)
        {
            throw new IllegalArgumentException($"{life} [{life.Order}] prior to {this} [{Order}]");
        }
        if (!(this is TLife item))
        {
            throw new IllegalArgumentException($"this {this} is not type of {nameof(TLife)}");
        }
        life.SetPrev(item);
        return item.Next = life;
    }

    public TLife Append<TOtherHandler>() where TOtherHandler : THandler
    {
        return Append(Value<TOtherHandler>());
    }

    private void SetPrev(TLife life)
    {
        if (Prev != null)
        {
            throw new IllegalArgumentException($"{this} prev is exist {Prev}");
        }
        Prev = life;
    }

    protected bool Equals(Lifecycle<TLife, THandler> other)
    {
        return handlerType == other.handlerType && lifeType == other.lifeType;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((Lifecycle<TLife, THandler>) obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(handlerType, lifeType);
    }

    public static TLife Value<TSubHandler>()
        where TSubHandler : THandler
    {
        return Value<TSubHandler>(LifecycleLevel.CUSTOM_LEVEL_5);
    }

    public static TLife Value(Type handlerType)
    {
        return Value(LifecycleLevel.CUSTOM_LEVEL_5, handlerType);
    }

    public static TLife Value<TSubHandler>(ILifecyclePriority lifeCycleLevel)
        where TSubHandler : THandler
    {
        return Value(lifeCycleLevel, typeof(TSubHandler));
    }

    public static TLife Value(ILifecyclePriority lifeCycleLevel, Type handleType)
    {
        var stage = Stage();
        var lifecycle = GetLifecycle(Stage(), handleType);
        if (lifecycle != null)
            return lifecycle;
        lifecycle = new TLife {
            Priority = lifeCycleLevel,
            handlerType = handleType,
            lifeType = typeof(TLife)
        };
        return PutLifecycle(stage, lifecycle);
    }

    private static LifecycleStage<TLife> Stage()
    {
        return LifecycleStage.ForLifecycleType<TLife>();
    }
}
