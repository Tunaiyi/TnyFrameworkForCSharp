// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

namespace TnyFramework.Common.Event.Notices;

/// <summary>
/// 无参数
/// </summary>
/// <typeparam name="TListener"></typeparam>
/// <typeparam name="TSource"></typeparam>
public class ListenEvent<TListener, TSource>
    : ListenBaseEvent<TListener, EventListen<TListener, TSource>, ListenEvent<TListener, TSource>>,
        IListenEvent<TListener, TSource>
{
    internal ListenEvent(EventListen<TListener, TSource> caller) : base(caller)
    {
    }

    private ListenEvent(ListenEvent<TListener, TSource> parent) :
        base(parent)
    {
    }

    public void Notify(TSource source)
    {
        parent?.Notify(source);
        DoTrigger(Invoke, source);
    }

    private void Invoke(
        EventListen<TListener, TSource> listen,
        TListener listener,
        TSource tuple)
    {
        listen.Invoke(
            listener,
            tuple
        );
    }

    public IListenEvent<TListener, TSource> ForkChild()
    {
        return new ListenEvent<TListener, TSource>(this);
    }
}

/// <summary>
/// 1个参数
/// </summary>
/// <typeparam name="TListener"></typeparam>
/// <typeparam name="TSource"></typeparam>
/// <typeparam name="TArg1"></typeparam>
public class ListenEvent<TListener, TSource, TArg1>
    : ListenBaseEvent<TListener, EventListen<TListener, TSource, TArg1>, ListenEvent<TListener, TSource, TArg1>>,
        IListenEvent<TListener, TSource, TArg1>
{
    internal ListenEvent(EventListen<TListener, TSource, TArg1> caller) : base(caller)
    {
    }

    private ListenEvent(ListenEvent<TListener, TSource, TArg1> parent) :
        base(parent)
    {
    }

    /// <summary>
    /// trigger
    /// </summary>
    public void Notify(TSource source, TArg1 arg1)
    {
        parent?.Notify(source, arg1);
        DoTrigger(Invoke, (source, arg1));
    }

    private void Invoke(
        EventListen<TListener, TSource, TArg1> listen,
        TListener listener,
        (TSource, TArg1) tuple)
    {
        listen.Invoke(listener, tuple);
    }

    public IListenEvent<TListener, TSource, TArg1> ForkChild()
    {
        return new ListenEvent<TListener, TSource, TArg1>(this);
    }
}

/// <summary>
/// 2个参数
/// </summary>
/// <typeparam name="TListener"></typeparam>
/// <typeparam name="TSource"></typeparam>
/// <typeparam name="TArg1"></typeparam>
/// <typeparam name="TArg2"></typeparam>
public class ListenEvent<TListener, TSource, TArg1, TArg2>
    : ListenBaseEvent<TListener, EventListen<TListener, TSource, TArg1, TArg2>, ListenEvent<TListener, TSource, TArg1, TArg2>>,
        IListenEvent<TListener, TSource, TArg1, TArg2>
{
    internal ListenEvent(EventListen<TListener, TSource, TArg1, TArg2> caller) : base(caller)
    {
    }

    private ListenEvent(ListenEvent<TListener, TSource, TArg1, TArg2> parent) :
        base(parent)
    {
    }

    /// <summary>
    /// trigger
    /// </summary>
    public void Notify(TSource source, TArg1 arg1, TArg2 arg2)
    {
        parent?.Notify(source, arg1, arg2);
        DoTrigger(Invoke, (source, arg1, arg2));
    }

    private void Invoke(
        EventListen<TListener, TSource, TArg1, TArg2> listen,
        TListener listener,
        (TSource, TArg1, TArg2) tuple)
    {
        listen.Invoke(listener, tuple);
    }

    public IListenEvent<TListener, TSource, TArg1, TArg2> ForkChild()
    {
        return new ListenEvent<TListener, TSource, TArg1, TArg2>(this);
    }
}

/// <summary>
/// 3个参数
/// </summary>
/// <typeparam name="TListener"></typeparam>
/// <typeparam name="TSource"></typeparam>
/// <typeparam name="TArg1"></typeparam>
/// <typeparam name="TArg2"></typeparam>
/// <typeparam name="TArg3"></typeparam>
public class ListenEvent<TListener, TSource, TArg1, TArg2, TArg3>
    : ListenBaseEvent<TListener, EventListen<TListener, TSource, TArg1, TArg2, TArg3>,
            ListenEvent<TListener, TSource, TArg1, TArg2, TArg3>>,
        IListenEvent<TListener, TSource, TArg1, TArg2, TArg3>
{
    internal ListenEvent(EventListen<TListener, TSource, TArg1, TArg2, TArg3> caller) : base(caller)
    {
    }

    private ListenEvent(ListenEvent<TListener, TSource, TArg1, TArg2, TArg3> parent) :
        base(parent)
    {
    }

    /// <summary>
    /// trigger
    /// </summary>
    public void Notify(TSource source, TArg1 arg1, TArg2 arg2, TArg3 arg3)
    {
        parent?.Notify(source, arg1, arg2, arg3);
        DoTrigger(Invoke, (source, arg1, arg2, arg3));
    }

    private void Invoke(
        EventListen<TListener, TSource, TArg1, TArg2, TArg3> handler,
        TListener listener,
        (TSource, TArg1, TArg2, TArg3) tuple)
    {
        handler.Invoke(listener, tuple);
    }

    public IListenEvent<TListener, TSource, TArg1, TArg2, TArg3> ForkChild()
    {
        return new ListenEvent<TListener, TSource, TArg1, TArg2, TArg3>(this);
    }
}

/// <summary>
/// 4个参数
/// </summary>
/// <typeparam name="TListener"></typeparam>
/// <typeparam name="TSource"></typeparam>
/// <typeparam name="TArg1"></typeparam>
/// <typeparam name="TArg2"></typeparam>
/// <typeparam name="TArg3"></typeparam>
/// <typeparam name="TArg4"></typeparam>
public class ListenEvent<TListener, TSource, TArg1, TArg2, TArg3, TArg4>
    : ListenBaseEvent<TListener, EventListen<TListener, TSource, TArg1, TArg2, TArg3, TArg4>,
            ListenEvent<TListener, TSource, TArg1, TArg2, TArg3, TArg4>>,
        IListenEvent<TListener, TSource, TArg1, TArg2, TArg3, TArg4>
{
    internal ListenEvent(EventListen<TListener, TSource, TArg1, TArg2, TArg3, TArg4> caller) : base(caller)
    {
    }

    private ListenEvent(ListenEvent<TListener, TSource, TArg1, TArg2, TArg3, TArg4> parent)
        : base(parent)
    {
    }

    /// <summary>
    /// trigger
    /// </summary>
    public void Notify(TSource source, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4)
    {
        parent?.Notify(source, arg1, arg2, arg3, arg4);
        DoTrigger(Invoke, (source, arg1, arg2, arg3, arg4));
    }

    private void Invoke(
        EventListen<TListener, TSource, TArg1, TArg2, TArg3, TArg4> handler,
        TListener listener,
        (TSource, TArg1, TArg2, TArg3, TArg4) tuple)
    {
        handler.Invoke(listener, tuple);
    }

    public IListenEvent<TListener, TSource, TArg1, TArg2, TArg3, TArg4> ForkChild()
    {
        return new ListenEvent<TListener, TSource, TArg1, TArg2, TArg3, TArg4>(this);
    }
}

/// <summary>
/// 5个参数
/// </summary>
/// <typeparam name="TListener"></typeparam>
/// <typeparam name="TSource"></typeparam>
/// <typeparam name="TArg1"></typeparam>
/// <typeparam name="TArg2"></typeparam>
/// <typeparam name="TArg3"></typeparam>
/// <typeparam name="TArg4"></typeparam>
/// <typeparam name="TArg5"></typeparam>
public class ListenEvent<TListener, TSource, TArg1, TArg2, TArg3, TArg4, TArg5>
    : ListenBaseEvent<TListener, EventListen<TListener, TSource, TArg1, TArg2, TArg3, TArg4, TArg5>,
            ListenEvent<TListener, TSource, TArg1, TArg2, TArg3, TArg4, TArg5>>,
        IListenEvent<TListener, TSource, TArg1, TArg2, TArg3, TArg4, TArg5>
{
    internal ListenEvent(EventListen<TListener, TSource, TArg1, TArg2, TArg3, TArg4, TArg5> caller) : base(caller)
    {
    }

    private ListenEvent(ListenEvent<TListener, TSource, TArg1, TArg2, TArg3, TArg4, TArg5> parent) :
        base(parent)
    {
    }

    /// <summary>
    /// trigger
    /// </summary>
    public void Notify(TSource source, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5)
    {
        parent?.Notify(source, arg1, arg2, arg3, arg4, arg5);
        DoTrigger(Invoke, (source, arg1, arg2, arg3, arg4, arg5));
    }

    private void Invoke(
        EventListen<TListener, TSource, TArg1, TArg2, TArg3, TArg4, TArg5> handler,
        TListener listener,
        (TSource, TArg1, TArg2, TArg3, TArg4, TArg5) tuple)
    {
        handler.Invoke(listener, tuple);
    }

    public IListenEvent<TListener, TSource, TArg1, TArg2, TArg3, TArg4, TArg5> ForkChild()
    {
        return new ListenEvent<TListener, TSource, TArg1, TArg2, TArg3, TArg4, TArg5>(this);
    }
}

/// <summary>
/// 6个参数
/// </summary>
/// <typeparam name="TListener"></typeparam>
/// <typeparam name="TSource"></typeparam>
/// <typeparam name="TArg1"></typeparam>
/// <typeparam name="TArg2"></typeparam>
/// <typeparam name="TArg3"></typeparam>
/// <typeparam name="TArg4"></typeparam>
/// <typeparam name="TArg5"></typeparam>
/// <typeparam name="TArg6"></typeparam>
public class ListenEvent<TListener, TSource, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6>
    : ListenBaseEvent<TListener, EventListen<TListener, TSource, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6>,
            ListenEvent<TListener, TSource, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6>>,
        IListenEvent<TListener, TSource, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6>
{
    internal ListenEvent(EventListen<TListener, TSource, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6> caller) : base(caller)
    {
    }

    private ListenEvent(ListenEvent<TListener, TSource, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6> parent) :
        base(parent)
    {
    }

    /// <summary>
    /// trigger
    /// </summary>
    public void Notify(TSource source, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6)
    {
        parent?.Notify(source, arg1, arg2, arg3, arg4, arg5, arg6);
        DoTrigger(Invoke, (source, arg1, arg2, arg3, arg4, arg5, arg6));
    }

    private void Invoke(
        EventListen<TListener, TSource, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6> handler,
        TListener listener,
        (TSource, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6) tuple)
    {
        handler.Invoke(listener, tuple);
    }

    public IListenEvent<TListener, TSource, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6> ForkChild()
    {
        return new ListenEvent<TListener, TSource, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6>(this);
    }
}
