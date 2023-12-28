// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;

namespace TnyFramework.Common.Event.Notices
{

    /// <summary>
    /// 无参数
    /// </summary>
    /// <typeparam name="TListener"></typeparam>
    /// <typeparam name="TSource"></typeparam>
    public class ListenerInvokeEvent<TListener, TSource>
        : ListenInvokerEvent<TListener, TSource, ListenerInvokeEvent<TListener, TSource>>,
            IListenEvent<TListener, TSource>
    {
        internal ListenerInvokeEvent(Action<TListener, TSource> invoker) : base(invoker)
    {
    }

        private ListenerInvokeEvent(ListenerInvokeEvent<TListener, TSource> parent) :
            base(parent)
    {
    }

        public void Notify(TSource source)
    {
        parent?.Notify(source);
        DoNotify(source);
    }

        public IListenEvent<TListener, TSource> ForkChild()
    {
        return new ListenerInvokeEvent<TListener, TSource>(this);
    }
    }

    /// <summary>
    /// 1个参数
    /// </summary>
    /// <typeparam name="TListener"></typeparam>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TArg1"></typeparam>
    public class ListenerInvokeEvent<TListener, TSource, TArg1>
        : ListenInvokerEvent<TListener, (TSource, TArg1), ListenerInvokeEvent<TListener, TSource, TArg1>>,
            IListenEvent<TListener, TSource, TArg1>
    {
        public ListenerInvokeEvent(Action<TListener, (TSource, TArg1)> invoker) : base(invoker)
    {
    }

        public ListenerInvokeEvent(ListenerInvokeEvent<TListener, TSource, TArg1> parent) : base(parent)
    {
    }

        /// <summary>
        /// trigger
        /// </summary>
        public void Notify(TSource source, TArg1 arg1)
    {
        parent?.Notify(source, arg1);
        DoNotify((source, arg1));
    }

        public IListenEvent<TListener, TSource, TArg1> ForkChild()
    {
        return new ListenerInvokeEvent<TListener, TSource, TArg1>(this);
    }
    }

    /// <summary>
    /// 2个参数
    /// </summary>
    /// <typeparam name="TListener"></typeparam>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TArg1"></typeparam>
    /// <typeparam name="TArg2"></typeparam>
    public class ListenerInvokeEvent<TListener, TSource, TArg1, TArg2>
        : ListenInvokerEvent<TListener, (TSource, TArg1, TArg2), ListenerInvokeEvent<TListener, TSource, TArg1, TArg2>>,
            IListenEvent<TListener, TSource, TArg1, TArg2>
    {
        public ListenerInvokeEvent(Action<TListener, (TSource, TArg1, TArg2)> invoker) : base(invoker)
    {
    }

        public ListenerInvokeEvent(ListenerInvokeEvent<TListener, TSource, TArg1, TArg2> parent) : base(parent)
    {
    }

        /// <summary>
        /// trigger
        /// </summary>
        public void Notify(TSource source, TArg1 arg1, TArg2 arg2)
    {
        parent?.Notify(source, arg1, arg2);
        DoNotify((source, arg1, arg2));
    }

        public IListenEvent<TListener, TSource, TArg1, TArg2> ForkChild()
    {
        return new ListenerInvokeEvent<TListener, TSource, TArg1, TArg2>(this);
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
    public class ListenerInvokeEvent<TListener, TSource, TArg1, TArg2, TArg3>
        : ListenInvokerEvent<TListener, (TSource, TArg1, TArg2, TArg3),
                ListenerInvokeEvent<TListener, TSource, TArg1, TArg2, TArg3>>,
            IListenEvent<TListener, TSource, TArg1, TArg2, TArg3>
    {
        public ListenerInvokeEvent(Action<TListener, (TSource, TArg1, TArg2, TArg3)> invoker) : base(invoker)
    {
    }

        public ListenerInvokeEvent(ListenerInvokeEvent<TListener, TSource, TArg1, TArg2, TArg3> parent) : base(parent)
    {
    }

        /// <summary>
        /// trigger
        /// </summary>
        public void Notify(TSource source, TArg1 arg1, TArg2 arg2, TArg3 arg3)
    {
        parent?.Notify(source, arg1, arg2, arg3);
        DoNotify((source, arg1, arg2, arg3));
    }

        public IListenEvent<TListener, TSource, TArg1, TArg2, TArg3> ForkChild()
    {
        return new ListenerInvokeEvent<TListener, TSource, TArg1, TArg2, TArg3>(this);
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
    public class ListenerInvokeEvent<TListener, TSource, TArg1, TArg2, TArg3, TArg4>
        : ListenInvokerEvent<TListener, (TSource, TArg1, TArg2, TArg3, TArg4),
                ListenerInvokeEvent<TListener, TSource, TArg1, TArg2, TArg3, TArg4>>,
            IListenEvent<TListener, TSource, TArg1, TArg2, TArg3, TArg4>
    {
        public ListenerInvokeEvent(Action<TListener, (TSource, TArg1, TArg2, TArg3, TArg4)> invoker) : base(invoker)
    {
    }

        public ListenerInvokeEvent(ListenerInvokeEvent<TListener, TSource, TArg1, TArg2, TArg3, TArg4> parent) : base(parent)
    {
    }

        /// <summary>
        /// trigger
        /// </summary>
        public void Notify(TSource source, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4)
    {
        parent?.Notify(source, arg1, arg2, arg3, arg4);
        DoNotify((source, arg1, arg2, arg3, arg4));
    }

        public IListenEvent<TListener, TSource, TArg1, TArg2, TArg3, TArg4> ForkChild()
    {
        return new ListenerInvokeEvent<TListener, TSource, TArg1, TArg2, TArg3, TArg4>(this);
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
    public class ListenerInvokeEvent<TListener, TSource, TArg1, TArg2, TArg3, TArg4, TArg5>
        : ListenInvokerEvent<TListener, (TSource, TArg1, TArg2, TArg3, TArg4, TArg5),
                ListenerInvokeEvent<TListener, TSource, TArg1, TArg2, TArg3, TArg4, TArg5>>,
            IListenEvent<TListener, TSource, TArg1, TArg2, TArg3, TArg4, TArg5>
    {
        public ListenerInvokeEvent(Action<TListener, (TSource, TArg1, TArg2, TArg3, TArg4, TArg5)> invoker) : base(invoker)
    {
    }

        public ListenerInvokeEvent(ListenerInvokeEvent<TListener, TSource, TArg1, TArg2, TArg3, TArg4, TArg5> parent) : base(parent)
    {
    }

        /// <summary>
        /// trigger
        /// </summary>
        public void Notify(TSource source, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5)
    {
        parent?.Notify(source, arg1, arg2, arg3, arg4, arg5);
        DoNotify((source, arg1, arg2, arg3, arg4, arg5));
    }

        public IListenEvent<TListener, TSource, TArg1, TArg2, TArg3, TArg4, TArg5> ForkChild()
    {
        return new ListenerInvokeEvent<TListener, TSource, TArg1, TArg2, TArg3, TArg4, TArg5>(this);
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
    public class ListenerInvokeEvent<TListener, TSource, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6>
        : ListenInvokerEvent<TListener, (TSource, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6),
                ListenerInvokeEvent<TListener, TSource, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6>>,
            IListenEvent<TListener, TSource, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6>
    {
        public ListenerInvokeEvent(Action<TListener, (TSource, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6)> invoker) : base(invoker)
    {
    }

        public ListenerInvokeEvent(ListenerInvokeEvent<TListener, TSource, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6> parent) : base(parent)
    {
    }

        /// <summary>
        /// trigger
        /// </summary>
        public void Notify(TSource source, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6)
    {
        parent?.Notify(source, arg1, arg2, arg3, arg4, arg5, arg6);
        DoNotify((source, arg1, arg2, arg3, arg4, arg5, arg6));
    }

        public IListenEvent<TListener, TSource, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6> ForkChild()
    {
        return new ListenerInvokeEvent<TListener, TSource, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6>(this);
    }
    }

}
