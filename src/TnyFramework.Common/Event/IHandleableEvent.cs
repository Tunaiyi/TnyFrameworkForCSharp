// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

namespace TnyFramework.Common.Event
{

    public interface IHandleableEvent<in TListener, out TEvent> : IEvent<TListener, TEvent>
        where TEvent : IHandleableEvent<TListener, TEvent>
    {
    }

    public interface IHandleEvent<TSource> :
        IHandleableEvent<EventHandle<TSource>, IHandleEvent<TSource>>,
        IEventNotice<TSource, IHandleEvent<TSource>>,
        IHandleWatch<TSource>
    {
    }

    public interface IHandleEvent<TSource, TArg1> :
        IHandleableEvent<EventHandle<TSource, TArg1>, IHandleEvent<TSource, TArg1>>,
        IEventNotice<TSource, TArg1, IHandleEvent<TSource, TArg1>>,
        IHandleWatch<TSource, TArg1>
    {
    }

    public interface IHandleEvent<TSource, TArg1, TArg2> :
        IHandleableEvent<EventHandle<TSource, TArg1, TArg2>, IHandleEvent<TSource, TArg1, TArg2>>,
        IEventNotice<TSource, TArg1, TArg2, IHandleEvent<TSource, TArg1, TArg2>>,
        IHandleWatch<TSource, TArg1, TArg2>
    {
    }

    public interface IHandleEvent<TSource, TArg1, TArg2, TArg3> :
        IHandleableEvent<EventHandle<TSource, TArg1, TArg2, TArg3>, IHandleEvent<TSource, TArg1, TArg2, TArg3>>,
        IEventNotice<TSource, TArg1, TArg2, TArg3, IHandleEvent<TSource, TArg1, TArg2, TArg3>>,
        IHandleWatch<TSource, TArg1, TArg2, TArg3>
    {
    }

    public interface IHandleEvent<TSource, TArg1, TArg2, TArg3, TArg4> :
        IHandleableEvent<EventHandle<TSource, TArg1, TArg2, TArg3, TArg4>, IHandleEvent<TSource, TArg1, TArg2, TArg3, TArg4>>,
        IEventNotice<TSource, TArg1, TArg2, TArg3, TArg4, IHandleEvent<TSource, TArg1, TArg2, TArg3, TArg4>>,
        IHandleWatch<TSource, TArg1, TArg2, TArg3, TArg4>
    {
    }

    public interface IHandleEvent<TSource, TArg1, TArg2, TArg3, TArg4, TArg5> :
        IHandleableEvent<EventHandle<TSource, TArg1, TArg2, TArg3, TArg4, TArg5>, IHandleEvent<TSource, TArg1, TArg2, TArg3, TArg4, TArg5>>,
        IEventNotice<TSource, TArg1, TArg2, TArg3, TArg4, TArg5, IHandleEvent<TSource, TArg1, TArg2, TArg3, TArg4, TArg5>>,
        IHandleWatch<TSource, TArg1, TArg2, TArg3, TArg4, TArg5>
    {
    }

    public interface IHandleEvent<TSource, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6> :
        IHandleableEvent<EventHandle<TSource, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6>, IHandleEvent<TSource, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6>>,
        IEventNotice<TSource, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, IHandleEvent<TSource, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6>>,
        IHandleWatch<TSource, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6>
    {
    }

}
