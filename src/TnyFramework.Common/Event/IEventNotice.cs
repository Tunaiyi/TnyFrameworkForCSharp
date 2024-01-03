// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

namespace TnyFramework.Common.Event
{

    public interface IEventNotice
    {
        IEvent ForkChild();
    }

    public interface IEventNotice<in TSource, out TEvent>
        where TEvent : IEvent
    {
        /// <summary>
        /// trigger
        /// </summary>
        public void Notify(TSource source);
    }

    public interface IEventNotice<in TSource, in TArg1, out TEvent>
        where TEvent : IEvent
    {
        /// <summary>
        /// trigger
        /// </summary>
        public void Notify(TSource source, TArg1 arg1);
    }

    public interface IEventNotice<in TSource, in TArg1, in TArg2, out TEvent>
        where TEvent : IEvent
    {
        /// <summary>
        /// trigger
        /// </summary>
        public void Notify(TSource source, TArg1 arg1, TArg2 arg2);
    }

    public interface IEventNotice<in TSource, in TArg1, in TArg2, in TArg3, out TEvent>
        where TEvent : IEvent
    {
        /// <summary>
        /// trigger
        /// </summary>
        public void Notify(TSource source, TArg1 arg1, TArg2 arg2, TArg3 arg3);
    }

    public interface IEventNotice<in TSource, in TArg1, in TArg2, in TArg3, in TArg4, out TEvent>
        where TEvent : IEvent
    {
        /// <summary>
        /// trigger
        /// </summary>
        public void Notify(TSource source, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4);
    }

    public interface IEventNotice<in TSource, in TArg1, in TArg2, in TArg3, in TArg4, in TArg5, out TEvent>
        where TEvent : IEvent
    {
        /// <summary>
        /// trigger
        /// </summary>
        public void Notify(TSource source, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5);
    }

    public interface IEventNotice<in TSource, in TArg1, in TArg2, in TArg3, in TArg4, in TArg5, in TArg6, out TEvent>
        where TEvent : IEvent
    {
        /// <summary>
        /// trigger
        /// </summary>
        public void Notify(TSource source, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6);
    }

}
