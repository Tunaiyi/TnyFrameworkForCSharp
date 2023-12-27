// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.Collections.Generic;

namespace TnyFramework.Common.Event.Buses
{

    public interface IEventBox<in THandler> : IEventBus
        where THandler : Delegate
    {
        /// <summary>
        /// 添加监听
        /// </summary>
        /// <param name="handler"></param>
        void Add(THandler handler);

        /// <summary>
        /// 添加监听
        /// </summary>
        /// <param name="handler"></param>
        void Add(IEnumerable<THandler> handler);

        /// <summary>
        /// 添加监听
        /// </summary>
        /// <param name="onNodeCreate"></param>
        /// <param name="handler"></param>
        void Add(object onNodeCreate, params THandler[] handler);

        /// <summary>
        /// 移除监听
        /// </summary>
        /// <param name="handler"></param>
        void Remove(THandler handler);

        /// <summary>
        /// 移除监听
        /// </summary>
        /// <param name="handler"></param>
        void Remove(IEnumerable<THandler> handler);

        /// <summary>
        /// 移除监听
        /// </summary>
        /// <param name="handler"></param>
        void Remove(params THandler[] handler);

        /// <summary>
        /// 清理所有
        /// </summary>
        void Clear();

        /// <summary>
        /// 获取父事件盒
        /// </summary>
        IEventBox<THandler>? Parent { get; }
    }

}
