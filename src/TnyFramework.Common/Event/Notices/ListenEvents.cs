// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

namespace TnyFramework.Common.Event.Notices
{

    public static partial class Events
    {
        public static IListenEvent<TListener, TSource> Create<TListener, TSource>(EventListen<TListener, TSource> listen)
        {
            return new ListenEvent<TListener, TSource>(listen);
        }

        public static IListenEvent<TListener, TSource, TArg1> Create<TListener, TSource, TArg1>(EventListen<TListener, TSource, TArg1> listen)
        {
            return new ListenEvent<TListener, TSource, TArg1>(listen);
        }

        public static IListenEvent<TListener, TSource, TArg1, TArg2> Create<TListener, TSource, TArg1, TArg2>(
            EventListen<TListener, TSource, TArg1, TArg2> listen)
        {
            return new ListenEvent<TListener, TSource, TArg1, TArg2>(listen);
        }

        public static IListenEvent<TListener, TSource, TArg1, TArg2, TArg3> Create<TListener, TSource, TArg1, TArg2, TArg3>(
            EventListen<TListener, TSource, TArg1, TArg2, TArg3> handler)
        {
            return new ListenEvent<TListener, TSource, TArg1, TArg2, TArg3>(handler);
        }

        public static IListenEvent<TListener, TSource, TArg1, TArg2, TArg3, TArg4> Create<TListener, TSource, TArg1, TArg2, TArg3, TArg4>(
            EventListen<TListener, TSource, TArg1, TArg2, TArg3, TArg4> handler)
        {
            return new ListenEvent<TListener, TSource, TArg1, TArg2, TArg3, TArg4>(handler);
        }

        public static IListenEvent<TListener, TSource, TArg1, TArg2, TArg3, TArg4, TArg5> Create<TListener, TSource, TArg1, TArg2, TArg3, TArg4,
            TArg5>(EventListen<TListener, TSource, TArg1, TArg2, TArg3, TArg4, TArg5> handler)
        {
            return new ListenEvent<TListener, TSource, TArg1, TArg2, TArg3, TArg4, TArg5>(handler);
        }

        public static IListenEvent<TListener, TSource, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6> Create<TListener, TSource, TArg1, TArg2, TArg3,
            TArg4, TArg5, TArg6>(EventListen<TListener, TSource, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6> handler)
        {
            return new ListenEvent<TListener, TSource, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6>(handler);
        }
    }

}
