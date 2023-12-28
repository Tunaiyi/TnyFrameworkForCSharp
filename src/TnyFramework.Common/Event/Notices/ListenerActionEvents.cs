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

    public static partial class Events
    {
        public static IListenEvent<TListener, TSource> Create<TListener, TSource>(
            Func<TListener, Action<TSource>> factory)
        {
            return new ListenerActionEvent<TListener, TSource>(factory);
        }

        public static IListenEvent<TListener, TSource, TArg1> Create<TListener, TSource, TArg1>(
            Func<TListener, Action<TSource, TArg1>> factory)
        {
            return new ListenerActionEvent<TListener, TSource, TArg1>(factory);
        }

        public static IListenEvent<TListener, TSource, TArg1, TArg2> Create<TListener, TSource, TArg1, TArg2>(
            Func<TListener, Action<TSource, TArg1, TArg2>> factory)
        {
            return new ListenerActionEvent<TListener, TSource, TArg1, TArg2>(factory);
        }

        public static IListenEvent<TListener, TSource, TArg1, TArg2, TArg3> Create<TListener, TSource, TArg1, TArg2, TArg3>(
            Func<TListener, Action<TSource, TArg1, TArg2, TArg3>> factory)
        {
            return new ListenerActionEvent<TListener, TSource, TArg1, TArg2, TArg3>(factory);
        }

        public static IListenEvent<TListener, TSource, TArg1, TArg2, TArg3, TArg4> Create<TListener, TSource, TArg1, TArg2, TArg3, TArg4>(
            Func<TListener, Action<TSource, TArg1, TArg2, TArg3, TArg4>> factory)
        {
            return new ListenerActionEvent<TListener, TSource, TArg1, TArg2, TArg3, TArg4>(factory);
        }

        public static IListenEvent<TListener, TSource, TArg1, TArg2, TArg3, TArg4, TArg5> Create<TListener, TSource, TArg1, TArg2, TArg3, TArg4,
            TArg5>(Func<TListener, Action<TSource, TArg1, TArg2, TArg3, TArg4, TArg5>> factory)
        {
            return new ListenerActionEvent<TListener, TSource, TArg1, TArg2, TArg3, TArg4, TArg5>(factory);
        }

        public static IListenEvent<TListener, TSource, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6> Create<TListener, TSource, TArg1, TArg2, TArg3,
            TArg4, TArg5, TArg6>(Func<TListener, Action<TSource, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6>> factory)
        {
            return new ListenerActionEvent<TListener, TSource, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6>(factory);
        }
    }

}
