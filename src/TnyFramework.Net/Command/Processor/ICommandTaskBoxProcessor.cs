// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System.Threading.Tasks;
using TnyFramework.Coroutines.Async;
using TnyFramework.Net.Command.Tasks;

namespace TnyFramework.Net.Command.Processor
{

    public interface ICommandTaskBoxProcessor
    {
        /// <summary>
        /// 提交
        /// </summary>
        /// <param name="box">消息盒</param>
        void Submit(CommandTaskBox box);

        /// <summary>
        /// 在协程上运行一个返回 task 的任务
        /// </summary>
        /// <param name="box">消息盒</param>
        /// <param name="handle">运行任务</param>
        /// <returns>task</returns>
        Task AsyncExec(CommandTaskBox box, AsyncHandle handle);

        /// <summary>
        /// 在协程上运行一个返回 task<T> 的任务
        /// </summary>
        /// <param name="box">消息盒</param>
        /// <param name="function">任务</param>
        /// <typeparam name="T">类型</typeparam>
        /// <returns>等待的任务</returns>
        Task<T> AsyncExec<T>(CommandTaskBox box, AsyncHandle<T> function);
    }

}
