// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System.Threading.Tasks;

namespace TnyFramework.Coroutines.Async
{

    public interface IAsyncExecutor
    {
        /// <summary>
        /// 在协程上运行一个返回 task 的任务
        /// </summary>
        /// <param name="handle">运行任务</param>
        /// <returns>task</returns>
        Task AsyncExec(AsyncHandle handle);

        /// <summary>
        /// <![CDATA[在协程上运行一个返回 task<T> 的任务]]>
        /// </summary>
        /// <param name="function">任务</param>
        /// <typeparam name="T">类型</typeparam>
        /// <returns>等待的任务</returns>
        Task<T> AsyncExec<T>(AsyncHandle<T> function);
    }

}
