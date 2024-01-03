// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

namespace TnyFramework.Common.Event
{

    /// <summary>
    /// 无参数
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    public class HandleEvent<TSource>
        : HandleBaseEvent<EventHandle<TSource>, IHandleEvent<TSource>>,
            IHandleEvent<TSource>
    {
        public HandleEvent()
        {
        }

        public HandleEvent(HandleEvent<TSource> parent) : base(parent)
        {
        }

        public void Notify(TSource source)
        {
            parent?.Notify(source);
            DoNotify(Invoke, source);
        }

        private void Invoke(
            EventHandle<TSource> handler,
            TSource tuple)
        {
            handler(tuple);
        }

        public override IHandleEvent<TSource> ForkChild()
        {
            return new HandleEvent<TSource>(this);
        }
    }

    /// <summary>
    /// 1个参数
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TArg1"></typeparam>
    public class HandleEvent<TSource, TArg1>
        : HandleBaseEvent<EventHandle<TSource, TArg1>, IHandleEvent<TSource, TArg1>>,
            IHandleEvent<TSource, TArg1>
    {
        public HandleEvent()
        {
        }

        private HandleEvent(HandleEvent<TSource, TArg1> parent) : base(parent)
        {
        }

        /// <summary>
        /// trigger
        /// </summary>
        public void Notify(TSource source, TArg1 arg1)
        {
            parent?.Notify(source, arg1);
            DoNotify(Invoke, (source, arg1));
        }

        private void Invoke(
            EventHandle<TSource, TArg1> handle,
            (TSource source, TArg1 arg1) tuple)
        {
            handle(
                tuple.Item1,
                tuple.Item2
            );
        }

        public override IHandleEvent<TSource, TArg1> ForkChild()
        {
            return new HandleEvent<TSource, TArg1>(this);
        }
    }

    /// <summary>
    /// 2个参数
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TArg1"></typeparam>
    /// <typeparam name="TArg2"></typeparam>
    public class HandleEvent<TSource, TArg1, TArg2>
        : HandleBaseEvent<EventHandle<TSource, TArg1, TArg2>, IHandleEvent<TSource, TArg1, TArg2>>,
            IHandleEvent<TSource, TArg1, TArg2>
    {
        public HandleEvent()
        {
        }

        private HandleEvent(HandleEvent<TSource, TArg1, TArg2> parent) : base(parent)
        {
        }

        /// <summary>
        /// trigger
        /// </summary>
        public void Notify(TSource source, TArg1 arg1, TArg2 arg2)
        {
            parent?.Notify(source, arg1, arg2);
            DoNotify(Invoke, (source, arg1, arg2));
        }

        private void Invoke(
            EventHandle<TSource, TArg1, TArg2> handle,
            (TSource, TArg1, TArg2) tuple)
        {
            handle(
                tuple.Item1,
                tuple.Item2,
                tuple.Item3
            );
        }

        public override IHandleEvent<TSource, TArg1, TArg2> ForkChild()
        {
            return new HandleEvent<TSource, TArg1, TArg2>(this);
        }
    }

    public class HandleEvent<TSource, TArg1, TArg2, TArg3>
        : HandleBaseEvent<EventHandle<TSource, TArg1, TArg2, TArg3>, IHandleEvent<TSource, TArg1, TArg2, TArg3>>,
            IHandleEvent<TSource, TArg1, TArg2, TArg3>
    {
        public HandleEvent()
        {
        }

        private HandleEvent(HandleEvent<TSource, TArg1, TArg2, TArg3> parent) : base(parent)
        {
        }

        /// <summary>
        /// trigger
        /// </summary>
        public void Notify(TSource source, TArg1 arg1, TArg2 arg2, TArg3 arg3)
        {
            parent?.Notify(source, arg1, arg2, arg3);
            DoNotify(Invoke, (source, arg1, arg2, arg3));
        }

        private void Invoke(
            EventHandle<TSource, TArg1, TArg2, TArg3> handle,
            (TSource, TArg1, TArg2, TArg3) tuple)
        {
            handle(
                tuple.Item1,
                tuple.Item2,
                tuple.Item3,
                tuple.Item4
            );
        }

        public override IHandleEvent<TSource, TArg1, TArg2, TArg3> ForkChild()
        {
            return new HandleEvent<TSource, TArg1, TArg2, TArg3>(this);
        }
    }

    public class HandleEvent<TSource, TArg1, TArg2, TArg3, TArg4>
        : HandleBaseEvent<EventHandle<TSource, TArg1, TArg2, TArg3, TArg4>, IHandleEvent<TSource, TArg1, TArg2, TArg3, TArg4>>,
            IHandleEvent<TSource, TArg1, TArg2, TArg3, TArg4>
    {
        public HandleEvent()
        {
        }

        private HandleEvent(HandleEvent<TSource, TArg1, TArg2, TArg3, TArg4> parent) : base(parent)
        {
        }

        /// <summary>
        /// trigger
        /// </summary>
        public void Notify(TSource source, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4)
        {
            parent?.Notify(source, arg1, arg2, arg3, arg4);
            DoNotify(Invoke, (source, arg1, arg2, arg3, arg4));
        }

        private void Invoke(
            EventHandle<TSource, TArg1, TArg2, TArg3, TArg4> handle,
            (TSource, TArg1, TArg2, TArg3, TArg4) tuple)
        {
            handle(
                tuple.Item1,
                tuple.Item2,
                tuple.Item3,
                tuple.Item4,
                tuple.Item5
            );
        }

        public override IHandleEvent<TSource, TArg1, TArg2, TArg3, TArg4> ForkChild()
        {
            return new HandleEvent<TSource, TArg1, TArg2, TArg3, TArg4>(this);
        }
    }

    public class HandleEvent<TSource, TArg1, TArg2, TArg3, TArg4, TArg5>
        : HandleBaseEvent<EventHandle<TSource, TArg1, TArg2, TArg3, TArg4, TArg5>, IHandleEvent<TSource, TArg1, TArg2, TArg3, TArg4, TArg5>>,
            IHandleEvent<TSource, TArg1, TArg2, TArg3, TArg4, TArg5>
    {
        public HandleEvent()
        {
        }

        private HandleEvent(HandleEvent<TSource, TArg1, TArg2, TArg3, TArg4, TArg5> parent) : base(parent)
        {
        }

        /// <summary>
        /// trigger
        /// </summary>
        public void Notify(TSource source, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5)
        {
            parent?.Notify(source, arg1, arg2, arg3, arg4, arg5);
            DoNotify(Invoke, (source, arg1, arg2, arg3, arg4, arg5));
        }

        private void Invoke(
            EventHandle<TSource, TArg1, TArg2, TArg3, TArg4, TArg5> handle,
            (TSource, TArg1, TArg2, TArg3, TArg4, TArg5) tuple)
        {
            handle(
                tuple.Item1,
                tuple.Item2,
                tuple.Item3,
                tuple.Item4,
                tuple.Item5,
                tuple.Item6
            );
        }

        public override IHandleEvent<TSource, TArg1, TArg2, TArg3, TArg4, TArg5> ForkChild()
        {
            return new HandleEvent<TSource, TArg1, TArg2, TArg3, TArg4, TArg5>(this);
        }
    }

    public class HandleEvent<TSource, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6>
        : HandleBaseEvent<EventHandle<TSource, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6>,
                IHandleEvent<TSource, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6>>,
            IHandleEvent<TSource, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6>
    {
        public HandleEvent()
        {
        }

        private HandleEvent(HandleEvent<TSource, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6> parent) : base(parent)
        {
        }

        /// <summary>
        /// trigger
        /// </summary>
        public void Notify(TSource source, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6)
        {
            parent?.Notify(source, arg1, arg2, arg3, arg4, arg5, arg6);
            DoNotify(Invoke, (source, arg1, arg2, arg3, arg4, arg5, arg6));
        }

        private void Invoke(
            EventHandle<TSource, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6> handle,
            (TSource, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6) tuple)
        {
            handle(
                tuple.Item1,
                tuple.Item2,
                tuple.Item3,
                tuple.Item4,
                tuple.Item5,
                tuple.Item6,
                tuple.Item7
            );
        }

        public override IHandleEvent<TSource, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6> ForkChild()
        {
            return new HandleEvent<TSource, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6>(this);
        }
    }

}
