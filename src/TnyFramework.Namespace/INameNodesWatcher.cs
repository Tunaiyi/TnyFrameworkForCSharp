// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System.Threading.Tasks;
using TnyFramework.Common.Event;
using TnyFramework.Namespace.Listener;

namespace TnyFramework.Namespace
{

    public interface INameNodesWatcher
    {
        /// <summary>
        /// 监控路径
        /// </summary>
        string WatchPath { get; }

        /// <summary>
        /// 是否是模糊匹配
        /// </summary>
        /// <returns>是否是模糊匹配</returns>
        bool IsMatch();

        /// <summary>
        /// 停止监控
        /// </summary>
        Task Unwatch();

        /// <summary>
        /// 是否是停止监控
        /// </summary>
        /// <returns></returns>
        bool IsUnwatch();

        /// <summary>
        /// 是否是监控
        /// </summary>
        /// <returns></returns>
        bool IsWatch();

        IEventBox<OnWatch> WatchEvent { get; }

        IEventBox<OnComplete> CompleteEvent { get; }

        IEventBox<OnError> ErrorEvent { get; }
    }

    public interface INameNodesWatcher<TValue> : INameNodesWatcher
    {
        /// <summary>
        /// 监控
        /// </summary>
        /// <returns>返回 Task</returns>
        Task<INameNodesWatcher<TValue>> Watch();

        IEventBox<OnNodeLoad<TValue>> LoadEvent { get; }

        IEventBox<OnNodeCreate<TValue>> CreateEvent { get; }

        IEventBox<OnNodeUpdate<TValue>> UpdateEvent { get; }

        IEventBox<OnNodeDelete<TValue>> DeleteEvent { get; }
    }

}
