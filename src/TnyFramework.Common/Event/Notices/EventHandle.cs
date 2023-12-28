// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

namespace TnyFramework.Common.Event.Notices
{

    public delegate void EventHandle<in TSource>(TSource source);

    public delegate void EventHandle<in TSource, in TArg1>(TSource source, TArg1 arg1);

    public delegate void EventHandle<in TSource, in TArg1, in TArg2>(TSource source, TArg1 arg1, TArg2 arg2);

    public delegate void EventHandle<in TSource, in TArg1, in TArg2, in TArg3>(TSource source, TArg1 arg1, TArg2 arg2, TArg3 arg3);

    public delegate void EventHandle<in TSource, in TArg1, in TArg2, in TArg3, in TArg4>(TSource source, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4);

    public delegate void EventHandle<in TSource, in TArg1, in TArg2, in TArg3, in TArg4, in TArg5>(TSource source, TArg1 arg1, TArg2 arg2, TArg3 arg3,
        TArg4 arg4, TArg5 arg5);

    public delegate void EventHandle<in TSource, in TArg1, in TArg2, in TArg3, in TArg4, in TArg5, in TArg6>(TSource source, TArg1 arg1, TArg2 arg2,
        TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6);

}
