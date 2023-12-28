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

    public interface IEventWatch<in TListener>
    {
        /// <summary>
        /// 添加监听
        /// </summary>
        /// <param name="listenerRef"></param>
        void Add(TListener listenerRef);

        /// <summary>
        /// 移除监听
        /// </summary>
        /// <param name="listener"></param>
        void Remove(TListener listener);

        /// <summary>
        /// 移除所有监听
        /// </summary>
        void RemoveAll();
    }

    public interface IEvent
    {
    }

    public interface IEvent<in TListener, out TEvent> : IEvent, IEventWatch<TListener>, IDisposable
        where TEvent : IEvent<TListener, TEvent>
    {
    }

}
