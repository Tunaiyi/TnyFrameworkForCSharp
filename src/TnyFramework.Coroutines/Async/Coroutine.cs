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

    public class Coroutine : ICoroutine
    {
        private static readonly ILogger LOGGER = LogFactory.Logger<Coroutine>();

        private static readonly ThreadLocal<Coroutine> CURRENT_COROUTINE = new();

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

        private SpinLock locker;

        private volatile SynchronizationContext? asSynchronizationContext;

        private volatile TaskScheduler? asTaskScheduler;

        public Coroutine() : this(TaskScheduler.Default)
        {
        }

        /// <summary>
        /// 协程构建器
        /// </summary>
        /// <param name="executeTaskScheduler">协程执行器</param>
        /// <param name="name">协程名字</param>
        public Coroutine(TaskScheduler executeTaskScheduler, string? name = null)
        {
            Id = Interlocked.Increment(ref _ID_COUNTER);
            Name = name ?? $"Coroutine-{Id}";
            Queue = new CoroutineWorkQueue();
            this.executeTaskScheduler = executeTaskScheduler;
        }

        public TaskScheduler AsTaskScheduler()
        {
            if (asTaskScheduler != null)
            {
                return asTaskScheduler;
            }
            var locked = false;
            locker.Enter(ref locked);
            try
            {
                if (asTaskScheduler != null)
                {
                    return asTaskScheduler;
                }
                asTaskScheduler = new CoroutineTaskScheduler(this);
                return asTaskScheduler;
            } finally
            {
                if (locked)
                {
                    locker.Exit(true);
                }
            }
        }

        public SynchronizationContext AsSynchronizationContext()
        {
            if (asSynchronizationContext != null)
            {
                return asSynchronizationContext;
            }
            var locked = false;
            locker.Enter(ref locked);
            try
            {
                if (asSynchronizationContext != null)
                {
                    return asSynchronizationContext;
                }
                asSynchronizationContext = new CoroutineSynchronizationContext(this);
                return asSynchronizationContext;
            } finally
            {
                if (locked)
                {
                    locker.Exit(true);
                }
            }
        }

        public static Coroutine CurrentCoroutine => CURRENT_COROUTINE.Value!;

        internal static void InitializeSynchronizationContext()
        {
            var current = SynchronizationContext.Current;
            if (current is CoroutineSynchronizationContext)
                return;
            var coroutine = new Coroutine();
            SynchronizationContext.SetSynchronizationContext(coroutine.AsSynchronizationContext());
            CURRENT_COROUTINE.Value = coroutine;
        }

        internal static void InitializeSynchronizationContext(ICoroutine coroutine)
        {
            var current = SynchronizationContext.Current;
            if (current is CoroutineSynchronizationContext)
                return;
            if (!(coroutine is Coroutine value))
                return;
            SynchronizationContext.SetSynchronizationContext(value.AsSynchronizationContext());
            CURRENT_COROUTINE.Value = value;
        }

        public int Id { get; }

        public string Name { get; }

        private CoroutineWorkQueue Queue { get; }

        // private CoroutineSynchronizationContext Context { get; }

        private bool PendingTasks => Queue.WorkCount != 0 || trackedCount != 0;

        internal void Track()
        {
            Interlocked.Increment(ref trackedCount);
        }

        internal void Tracked()
        {
            Interlocked.Decrement(ref trackedCount);
        }

        internal bool InCoroutine => this == CURRENT_COROUTINE.Value;

        internal bool InThread => execThread == Thread.CurrentThread;

        public Task<bool> Shutdown(long millisecondsTimeout)
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

        public Task Delay(int millisecondsDelay)
        {
            return Task.Delay(millisecondsDelay);
        }

        public Task Delay(TimeSpan delay)
        {
            return Task.Delay(delay);
        }

        public Task Repeat(int times, Action action)
        {
            return AsyncExec(async () => {
                for (var index = 0; index < times; index++)
                {
                    action.Invoke();
                    await Task.Yield();
                }
            });
        }

        public Task Repeat(int times, AsyncHandle handle)
        {
            return AsyncExec(async () => {
                for (var index = 0; index < times; index++)
                {
                    await handle.Invoke();
                }
            });
        }

        public Task ExecUntil(AsyncHandle<bool> func)
        {
            return AsyncExec(async () => {
                while (true)
                {
                    if (await func.Invoke())
                        break;
                }
            });
        }

        public Task RunUntil(Func<bool> func)
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

        public Task<T> ExecUntil<T>(AsyncHandle<CoroutineState<T>> func)
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

        public Task<T> RunUntil<T>(Func<CoroutineState<T>> func)
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

        // Exec will execute tasks off the task list
        private void ExecuteAllWorks()
        {
            DoTransaction(() =>
                Task.Factory.StartNew(DoExecuteWorks, CancellationToken.None, TaskCreationOptions.None, asTaskScheduler ?? AsTaskScheduler()));
        }

        private void DoTransaction(Action action)
        {
            var currentCoroutine = CURRENT_COROUTINE.Value;
            try
            {
                execThread = Thread.CurrentThread;
                CURRENT_COROUTINE.Value = this;
                action();
            } finally
            {
                execThread = null!;
                CURRENT_COROUTINE.Value = currentCoroutine ?? null!;
                Interlocked.Exchange(ref submit, SUBMIT_STATUS_IDLE);
                if (!Queue.IsWorkEmpty)
                {
                    TrySummit();
                }
            }
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

        public CoroutineStatus Status => (CoroutineStatus) status;

        public bool IsStart()
        {
            return Status == CoroutineStatus.Start;
        }

        public bool IsStop()
        {
            return Status != CoroutineStatus.Start;
        }

        public bool IsShutdown()
        {
            return Status == CoroutineStatus.Shutdown;
        }

        public async Task AsyncExec(AsyncHandle handle)
        {
            CheckStartStatus();
            await DoRun(handle);
        }

        public async Task<T> AsyncExec<T>(AsyncHandle<T> function)
        {
            CheckStartStatus();
            return await DoExec(function);
        }

        public override string ToString()
        {
            return $"Coroutine[{Name}]-cid[{Id}]";
        }

        private void CheckStartStatus()
        {
            if (Status != CoroutineStatus.Start)
            {
                throw new CoroutineStatusException($"{Name}-cid{Id} 已关闭");
            }
        }

        internal Task AsyncExec(ICoroutineWork work)
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
    }

}
