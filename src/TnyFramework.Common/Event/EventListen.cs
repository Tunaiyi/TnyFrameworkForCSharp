// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

namespace TnyFramework.Common.Event
{

    public delegate void EventListen<in TListener, in TSource>(TListener listener,
        TSource source);

    public delegate void EventListen<in TListener, TSource, TArg1>(TListener listener,
        (TSource source, TArg1 arg1) tuple);

    public delegate void EventListen<in TListener, TSource, TArg1, TArg2>(TListener listener,
        (TSource source, TArg1 arg1, TArg2 arg2) tuple);

    public delegate void EventListen<in TListener, TSource, TArg1, TArg2, TArg3>(TListener listener,
        (TSource source, TArg1 arg1, TArg2 arg2, TArg3 arg3) tuple);

    public delegate void EventListen<in TListener, TSource, TArg1, TArg2, TArg3, TArg4>(TListener listener,
        (TSource source, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4) tuple);

    public delegate void EventListen<in TListener, TSource, TArg1, TArg2, TArg3, TArg4, TArg5>(TListener listener,
        (TSource source, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5) tuple);

    public delegate void EventListen<in TListener, TSource, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6>(TListener listener,
        (TSource source, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6) tuple);

}
