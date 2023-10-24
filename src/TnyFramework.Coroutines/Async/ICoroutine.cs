// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.Threading;
using System.Threading.Tasks;

namespace TnyFramework.Coroutines.Async
{

    /// <summary>
    /// 协程接口
    /// </summary>
    public interface ICoroutine : IAsyncExecutor
    {
        /// <summary>
        /// 协程状态
        /// </summary>
        CoroutineStatus Status { get; }

        TaskScheduler AsTaskScheduler();

        SynchronizationContext AsSynchronizationContext();

        /// <summary>
        /// 协程 id 自动分配
        /// </summary>
        int Id { get; }

        /// <summary>
        /// 协程名称
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 是否启动
        /// </summary>
        /// <returns>启动返回 true</returns>
        bool IsStart();

        /// <summary>
        /// 是否停止(非 start 状态)
        /// </summary>
        /// <returns>停止返回 true</returns>
        bool IsStop();

        /// <summary>
        /// 是否已关闭
        /// </summary>
        /// <returns>关闭返回 true</returns>
        bool IsShutdown();

        /// <summary>
        /// 延迟
        /// </summary>
        /// <param name="millisecondsDelay">延迟时间</param>
        /// <returns>task</returns>
        Task Delay(int millisecondsDelay);

        /// <summary>
        /// 延迟
        /// </summary>
        /// <param name="delay">延迟时间</param>
        /// <returns>task</returns>
        Task Delay(TimeSpan delay);

        /// <summary>
        /// 重复运行
        /// </summary>
        /// <param name="times">次数</param>
        /// <param name="action">行为</param>
        /// <returns>task</returns>
        Task Repeat(int times, Action action);

        /// <summary>
        /// 重复运行
        /// </summary>
        /// <param name="times">次数</param>
        /// <param name="handle">行为</param>
        /// <returns>task</returns>
        Task Repeat(int times, AsyncHandle handle);

        /// <summary>
        /// 运行直到任务返回 true 则停止, 每次运行会进行 await func
        /// </summary>
        /// <param name="func">行为</param>
        /// <returns>task</returns>
        Task ExecUntil(AsyncHandle<bool> func);

        /// <summary>
        /// 运行直到任务返回 true 则停止, 每次运行会进行 await func
        /// </summary>
        /// <param name="func">行为</param>
        /// <returns>task</returns>
        Task<T> ExecUntil<T>(AsyncHandle<CoroutineState<T>> func);

        /// <summary>
        /// 运行直到任务返回 true 则停止, 每次运行会进行 yield
        /// </summary>
        /// <param name="func">行为</param>
        /// <returns>task</returns>
        Task RunUntil(Func<bool> func);

        /// <summary>
        /// 运行直到任务返回 true 则停止, 每次运行会进行 yield
        /// </summary>
        /// <param name="func">行为</param>
        /// <returns>task</returns>
        Task<T> RunUntil<T>(Func<CoroutineState<T>> func);

        /// <summary>
        /// 关闭协程
        /// </summary>
        /// <param name="millisecondsTimeout">等待超时</param>
        /// <returns>无超时返回 false, 超时返回 true</returns>
        Task<bool> Shutdown(long millisecondsTimeout);
    }

}
