// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TnyFramework.Common.Extensions;
using TnyFramework.Common.Logger;
using TnyFramework.Coroutines.Exceptions;

namespace TnyFramework.Coroutines.Async
{

    public class DefaultCoroutine : Coroutine
    {
        private static readonly ILogger LOGGER = LogFactory.Logger<Coroutine>();

        // 提交状态值 空闲
        private const int SUBMIT_STATUS_IDLE = 0;

        // 提交状态值 已提交
        private const int SUBMIT_STATUS_SUBMIT = 1;

        // ID 计数器
        private static volatile int _ID_COUNTER;

        // 协程执行器
        private readonly TaskScheduler executeTaskScheduler;

        // 提交状态
        private volatile int submit = SUBMIT_STATUS_IDLE;

        // 协程状态
        private volatile int status = (int) CoroutineStatus.Start;

        // 跟踪计数
        private int trackedCount;

        // 当前运行线程(运行任务时非空)空闲为 null
        private volatile Thread? execThread;

        // 关闭任务
        private volatile Task<bool>? shuttingTask = null;

        public DefaultCoroutine() : this(Default)
        {
        }

        /// <summary>
        /// 协程构建器
        /// </summary>
        /// <param name="executeTaskScheduler">协程执行器</param>
        /// <param name="name">协程名字</param>
        public DefaultCoroutine(TaskScheduler executeTaskScheduler, string? name = null)
        {
            CoroutineId = Interlocked.Increment(ref _ID_COUNTER);
            Name = name ?? $"Coroutine-{CoroutineId}";
            Queue = new CoroutineWorkQueue();
            this.executeTaskScheduler = executeTaskScheduler;
            Coroutine = this;
        }

        public override int CoroutineId { get; }

        public override string Name { get; }

        private CoroutineWorkQueue Queue { get; }

        // private CoroutineSynchronizationContext Context { get; }

        private bool PendingTasks => Queue.WorkCount != 0 || trackedCount != 0;

        internal override void Track()
        {
            Interlocked.Increment(ref trackedCount);
        }

        internal override void Tracked()
        {
            Interlocked.Decrement(ref trackedCount);
        }

        public override bool InCoroutine => this == CurrentCoroutine;

        internal override bool InThread => execThread == Thread.CurrentThread;

        public override Task<bool> Shutdown(long millisecondsTimeout)
        {
            while (true)
            {
                var current = status;
                if (current != (int) CoroutineStatus.Start)
                {
                    var shutting = shuttingTask;
                    return shutting ?? Task.FromResult(true);
                }
                if (Interlocked.CompareExchange(ref status, (int) CoroutineStatus.Shutting, current) != current)
                    continue;
                var task = new CoroutineFuncWork<bool>(() => DoShutdown(millisecondsTimeout));
                AsyncExec(task);
                return task.AwaitTypeTask;
            }
        }

        public override Task Delay(int millisecondsDelay)
        {
            return Task.Delay(millisecondsDelay);
        }

        public override Task Delay(TimeSpan delay)
        {
            return Task.Delay(delay);
        }

        public override Task Repeat(int times, Action action)
        {
            return AsyncExec(async () => {
                for (var index = 0; index < times; index++)
                {
                    action.Invoke();
                    await Task.Yield();
                }
            });
        }

        public override Task Repeat(int times, AsyncHandle handle)
        {
            return AsyncExec(async () => {
                for (var index = 0; index < times; index++)
                {
                    await handle.Invoke();
                }
            });
        }

        public override Task ExecUntil(AsyncHandle<bool> func)
        {
            return AsyncExec(async () => {
                while (true)
                {
                    if (await func.Invoke())
                        break;
                }
            });
        }

        public override Task RunUntil(Func<bool> func)
        {
            return AsyncExec(async () => {
                while (true)
                {
                    if (!func.Invoke())
                    {
                        await Task.Yield();
                    } else
                    {
                        break;
                    }
                }
            });
        }

        public override Task<T> ExecUntil<T>(AsyncHandle<CoroutineState<T>> func)
        {
            return AsyncExec(async () => {
                while (true)
                {
                    var state = await func.Invoke();
                    if (!state.Continue)
                    {
                        return state.Result;
                    }
                }
            });
        }

        public override Task<T> RunUntil<T>(Func<CoroutineState<T>> func)
        {
            return AsyncExec(async () => {
                while (true)
                {
                    var state = func.Invoke();
                    if (!state.Continue)
                    {
                        return state.Result;
                    }
                    await Task.Yield();
                }
            });
        }

        private async Task<bool> DoShutdown(long millisecondsTimeout)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            while (PendingTasks)
            {
                var remain = millisecondsTimeout - stopwatch.ElapsedMilliseconds;
                if (remain <= 0)
                {
                    LOGGER.LogInformation("{Coroutine} shutdown timeout !!! Pending {Count}", this, trackedCount);
                    Interlocked.Exchange(ref status, (int) CoroutineStatus.Shutdown);
                    return false;
                }
                await Task.Delay((int) Math.Min(remain, 20L));
            }
            LOGGER.LogInformation("{Coroutine} shutdown success.  Pending {Count}", this, trackedCount);
            Interlocked.Exchange(ref status, (int) CoroutineStatus.Shutdown);
            return true;
        }

        public override CoroutineStatus Status => (CoroutineStatus) status;

        public override bool IsStart()
        {
            return Status == CoroutineStatus.Start;
        }

        public override bool IsStop()
        {
            return Status != CoroutineStatus.Start;
        }

        public override bool IsShutdown()
        {
            return Status == CoroutineStatus.Shutdown;
        }

        public override async Task AsyncExec(AsyncHandle handle)
        {
            CheckStartStatus();
            await DoRun(handle);
        }

        public override async Task<T> AsyncExec<T>(AsyncHandle<T> function)
        {
            CheckStartStatus();
            return await DoExec(function);
        }

        public override string ToString()
        {
            return $"Coroutine[{Name}]-cid[{CoroutineId}]";
        }

        private void CheckStartStatus()
        {
            if (Status != CoroutineStatus.Start)
            {
                throw new CoroutineStatusException($"{Name}-cid{CoroutineId} 已关闭");
            }
        }

        internal override Task AsyncExec(ICoroutineWork work)
        {
            var task = work.AwaitTask;
            Enqueue(work);
            return task;
        }

        private Task DoRun(AsyncHandle handle)
        {
            var task = new CoroutineActionWork(handle);
            return AsyncExec(task);
        }

        private Task<T> DoExec<T>(AsyncHandle<T> function)
        {
            var task = new CoroutineFuncWork<T>(function);
            AsyncExec(task);
            return task.AwaitTypeTask;
        }

        private void Enqueue(ICoroutineWork request)
        {
            if (InCoroutine || InThread)
            {
                request.Invoke();
            } else
            {
                Queue.Enqueue(request);
                TrySummit();
            }
        }

        private void TrySummit()
        {
            var current = submit;
            if (current == SUBMIT_STATUS_IDLE && Interlocked.CompareExchange(ref submit, SUBMIT_STATUS_SUBMIT, SUBMIT_STATUS_IDLE) == current)
            {
                executeTaskScheduler.StartNew(ExecuteAllWorks);
            }
        }

        // Exec will execute tasks off the task list
        private void ExecuteAllWorks()
        {
            DoTransaction(() => Task.Factory.StartNew(DoExecuteWorks, CancellationToken.None, TaskCreationOptions.None, this));
        }

        private void DoExecuteWorks()
        {
            DoTransaction(() => {
                var queue = Queue.CurrentFrameQueue;
                if (queue.IsEmpty)
                {
                    return;
                }
                while (queue.TryDequeue(out var work))
                {
                    work.Invoke();
                }
            });
        }

        private void DoTransaction(Action action)
        {
            var currentCoroutine = CurrentCoroutine;
            try
            {
                execThread = Thread.CurrentThread;
                SetCurrentCoroutine(this);
                action();
            } finally
            {
                execThread = null!;
                SetCurrentCoroutine(currentCoroutine ?? null!);
                Interlocked.Exchange(ref submit, SUBMIT_STATUS_IDLE);
                if (!Queue.IsWorkEmpty)
                {
                    TrySummit();
                }
            }
        }
    }

}
