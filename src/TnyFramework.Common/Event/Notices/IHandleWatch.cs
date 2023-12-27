// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

namespace TnyFramework.Common.Event.Notices;

public interface IHandleWatch<out TSource>
    : IEventWatch<EventHandle<TSource>>;

public interface IHandleWatch<out TSource, out TArg1>
    : IEventWatch<EventHandle<TSource, TArg1>>;

public interface IHandleWatch<out TSource, out TArg1, out TArg2>
    : IEventWatch<EventHandle<TSource, TArg1, TArg2>>;

public interface IHandleWatch<out TSource, out TArg1, out TArg2, out TArg3>
    : IEventWatch<EventHandle<TSource, TArg1, TArg2, TArg3>>;

public interface IHandleWatch<out TSource, out TArg1, out TArg2, out TArg3, out TArg4>
    : IEventWatch<EventHandle<TSource, TArg1, TArg2, TArg3, TArg4>>;

public interface IHandleWatch<out TSource, out TArg1, out TArg2, out TArg3, out TArg4, out TArg5>
    : IEventWatch<EventHandle<TSource, TArg1, TArg2, TArg3, TArg4, TArg5>>;

public interface IHandleWatch<out TSource, out TArg1, out TArg2, out TArg3, out TArg4, out TArg5, out TArg6>
    : IEventWatch<EventHandle<TSource, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6>>;
