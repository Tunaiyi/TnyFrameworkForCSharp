// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

namespace TnyFramework.Common.Event
{

    public static partial class Events
    {
        public static IHandleEvent<TSource> Create<TSource>()
        {
            return new HandleEvent<TSource>();
        }

        public static IHandleEvent<TSource, TArg1> Create<TSource, TArg1>()
        {
            return new HandleEvent<TSource, TArg1>();
        }

        public static IHandleEvent<TSource, TArg1, TArg2> Create<TSource, TArg1, TArg2>()
        {
            return new HandleEvent<TSource, TArg1, TArg2>();
        }

        public static IHandleEvent<TSource, TArg1, TArg2, TArg3> Create<TSource, TArg1, TArg2, TArg3>()
        {
            return new HandleEvent<TSource, TArg1, TArg2, TArg3>();
        }

        public static IHandleEvent<TSource, TArg1, TArg2, TArg3, TArg4> Create<TSource, TArg1, TArg2, TArg3, TArg4>()
        {
            return new HandleEvent<TSource, TArg1, TArg2, TArg3, TArg4>();
        }

        public static IHandleEvent<TSource, TArg1, TArg2, TArg3, TArg4, TArg5> Create<TSource, TArg1, TArg2, TArg3, TArg4, TArg5>()
        {
            return new HandleEvent<TSource, TArg1, TArg2, TArg3, TArg4, TArg5>();
        }

        public static IHandleEvent<TSource, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6> Create<TSource, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6>()
        {
            return new HandleEvent<TSource, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6>();
        }
    }

}
