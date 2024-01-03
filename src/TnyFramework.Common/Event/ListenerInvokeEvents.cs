// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;

namespace TnyFramework.Common.Event
{

    public static partial class Events
    {
        public static IListenEvent<TListener, TSource> Create<TListener, TSource>(
            Action<TListener, TSource> handler)
        {
            return new ListenerInvokeEvent<TListener, TSource>(handler);
        }

        public static IListenEvent<TListener, TSource, TArg1> Create<TListener, TSource, TArg1>(
            Action<TListener, (TSource source, TArg1 arg1)> handler)
        {
            return new ListenerInvokeEvent<TListener, TSource, TArg1>(handler);
        }

        public static IListenEvent<TListener, TSource, TArg1, TArg2> Create<TListener, TSource, TArg1, TArg2>(
            Action<TListener, (TSource source, TArg1 arg1, TArg2 arg2)> handler)
        {
            return new ListenerInvokeEvent<TListener, TSource, TArg1, TArg2>(handler);
        }

        public static IListenEvent<TListener, TSource, TArg1, TArg2, TArg3> Create<TListener, TSource, TArg1, TArg2, TArg3>(
            Action<TListener, (TSource source, TArg1 arg1, TArg2 arg2, TArg3 arg3)> handler)
        {
            return new ListenerInvokeEvent<TListener, TSource, TArg1, TArg2, TArg3>(handler);
        }

        public static IListenEvent<TListener, TSource, TArg1, TArg2, TArg3, TArg4> Create<TListener, TSource, TArg1, TArg2, TArg3, TArg4>(
            Action<TListener, (TSource source, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4)> handler)
        {
            return new ListenerInvokeEvent<TListener, TSource, TArg1, TArg2, TArg3, TArg4>(handler);
        }

        public static IListenEvent<TListener, TSource, TArg1, TArg2, TArg3, TArg4, TArg5> Create<TListener, TSource, TArg1, TArg2, TArg3, TArg4,
            TArg5>(Action<TListener, (TSource source, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5)> handler)
        {
            return new ListenerInvokeEvent<TListener, TSource, TArg1, TArg2, TArg3, TArg4, TArg5>(handler);
        }

        public static IListenEvent<TListener, TSource, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6> Create<TListener, TSource, TArg1, TArg2, TArg3,
            TArg4, TArg5, TArg6>(Action<TListener, (TSource source, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6)> handler)
        {
            return new ListenerInvokeEvent<TListener, TSource, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6>(handler);
        }


    }

}
