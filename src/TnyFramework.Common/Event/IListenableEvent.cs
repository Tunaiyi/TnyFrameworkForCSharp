// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

namespace TnyFramework.Common.Event
{

    public interface IListenableEvent<in TListener, out TEvent> : IEvent<TListener, TEvent>
        where TEvent : IListenableEvent<TListener, TEvent>
    {
    }

    public interface IListenEvent<in TListener, in TSource> :
        IListenableEvent<TListener, IListenEvent<TListener, TSource>>,
        IEventNotice<TSource, IListenEvent<TListener, TSource>>
    {
    }

    public interface IListenEvent<in TListener, in TSource, in TArg1> :
        IListenableEvent<TListener, IListenEvent<TListener, TSource, TArg1>>,
        IEventNotice<TSource, TArg1, IListenEvent<TListener, TSource, TArg1>>
    {
    }

    public interface IListenEvent<in TListener, in TSource, in TArg1, in TArg2> :
        IListenableEvent<TListener, IListenEvent<TListener, TSource, TArg1, TArg2>>,
        IEventNotice<TSource, TArg1, TArg2, IListenEvent<TListener, TSource, TArg1, TArg2>>
    {
    }

    public interface IListenEvent<in TListener, in TSource, in TArg1, in TArg2, in TArg3> :
        IListenableEvent<TListener, IListenEvent<TListener, TSource, TArg1, TArg2, TArg3>>,
        IEventNotice<TSource, TArg1, TArg2, TArg3, IListenEvent<TListener, TSource, TArg1, TArg2, TArg3>>
    {
    }

    public interface IListenEvent<in TListener, in TSource, in TArg1, in TArg2, in TArg3, in TArg4> :
        IListenableEvent<TListener, IListenEvent<TListener, TSource, TArg1, TArg2, TArg3, TArg4>>,
        IEventNotice<TSource, TArg1, TArg2, TArg3, TArg4, IListenEvent<TListener, TSource, TArg1, TArg2, TArg3, TArg4>>
    {
    }

    public interface IListenEvent<in TListener, in TSource, in TArg1, in TArg2, in TArg3, in TArg4, in TArg5> :
        IListenableEvent<TListener, IListenEvent<TListener, TSource, TArg1, TArg2, TArg3, TArg4, TArg5>>,
        IEventNotice<TSource, TArg1, TArg2, TArg3, TArg4, TArg5, IListenEvent<TListener, TSource, TArg1, TArg2, TArg3, TArg4, TArg5>>
    {
    }

    public interface IListenEvent<in TListener, in TSource, in TArg1, in TArg2, in TArg3, in TArg4, in TArg5, in TArg6> :
        IListenableEvent<TListener, IListenEvent<TListener, TSource, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6>>,
        IEventNotice<TSource, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6,
            IListenEvent<TListener, TSource, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6>>
    {
    }

}
