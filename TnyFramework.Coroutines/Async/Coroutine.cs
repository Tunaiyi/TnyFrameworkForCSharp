using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TnyFramework.Common.Logger;
using TnyFramework.Coroutines.Exception;
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
        private readonly ICoroutineExecutor executor;

        // 提交状态
        private volatile int submit = SUBMIT_STATUS_IDLE;

        // 协程状态
        private volatile int status = (int)CoroutineStatus.Start;

        // 跟踪计数
        private int trackedCount;

        // 当前运行线程(运行任务时非空)空闲为 null
        private volatile Thread execThread;

        // 关闭任务
        private volatile Task<bool> shuttingTask = null;


        public static Coroutine Current => CURRENT_COROUTINE.Value;


        internal static void InitializeSynchronizationContext()
        {
            var current = SynchronizationContext.Current;
            if (current is CoroutineSynchronizationContext)
                return;
            var coroutine = new Coroutine();
            SynchronizationContext.SetSynchronizationContext(coroutine.Context);
            CURRENT_COROUTINE.Value = coroutine;
        }


        public Coroutine() : this(ThreadPoolCoroutineExecutor.Default)
        {
        }


        /// <summary>
        /// 协程构建器
        /// </summary>
        /// <param name="executor">协程执行器</param>
        /// <param name="name">协程名字</param>
        public Coroutine(ICoroutineExecutor executor, string name = null)
        {
            Id = Interlocked.Increment(ref _ID_COUNTER);
            Name = name ?? $"Coroutine-{Id}";
            Queue = new CoroutineWorkQueue();
            Context = new CoroutineSynchronizationContext(this);
            this.executor = executor;
        }


        public int Id { get; }

        public string Name { get; }

        private CoroutineWorkQueue Queue { get; }

        internal CoroutineSynchronizationContext Context { get; }

        private bool PendingTasks => Queue.WorkCount != 0 || trackedCount != 0;


        internal void Track()
        {
            Interlocked.Increment(ref trackedCount);
        }


        internal void Tracked()
        {
            Interlocked.Decrement(ref trackedCount);
        }


        public void Post(CoroutineSynchronizationContext context, SendOrPostCallback callback, object state)
        {
            Enqueue(new CoroutineWork(callback, context, state));
        }


        public void Send(CoroutineSynchronizationContext context, SendOrPostCallback callback, object state)
        {
            if (execThread == Thread.CurrentThread)
            {
                callback(state);
            } else
            {
                using var handle = new ManualResetEvent(false);
                Enqueue(new CoroutineWork(callback, context, state, handle));
                handle.WaitOne();
            }
        }


        private void Enqueue(CoroutineWork request)
        {
            Queue.Enqueue(request);
            TrySummit();
        }


        private void TrySummit()
        {
            var current = submit;
            if (current != SUBMIT_STATUS_IDLE)
                return;
            if (Interlocked.CompareExchange(ref submit, SUBMIT_STATUS_SUBMIT, SUBMIT_STATUS_IDLE) == current)
            {
                executor.Summit(ExecuteTasks);
            }
        }


        public Task<bool> Shutdown(long millisecondsTimeout)
        {
            while (true)
            {
                var current = status;
                if (current != (int)CoroutineStatus.Start)
                {
                    var shutting = shuttingTask;
                    return shutting ?? Task.FromResult(true);
                }
                if (Interlocked.CompareExchange(ref status, (int)CoroutineStatus.Shutting, current) != current)
                    continue;
                var task = new CoroutineFuncTask<bool>(() => DoShutdown(millisecondsTimeout));
                Context.Post((state) => ((ICoroutineTask)state).Invoke(), task);
                return task.SourceTask;
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
            return Exec(async () => {
                for (var index = 0; index < times; index++)
                {
                    action.Invoke();
                    await Task.Yield();
                }
            });
        }


        public Task Repeat(int times, CoroutineAction action)
        {
            return Exec(async () => {
                for (var index = 0; index < times; index++)
                {
                    await action.Invoke();
                }
            });
        }


        public Task ExecUntil(CoroutineFunc<bool> func)
        {
            return Exec(async () => {
                while (true)
                {
                    if (await func.Invoke())
                        break;
                }
            });
        }


        public Task RunUntil(Func<bool> func)
        {
            return Exec(async () => {
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


        public Task<T> ExecUntil<T>(CoroutineFunc<CoroutineState<T>> func)
        {
            return Exec(async () => {
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
            return Exec(async () => {
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
                    LOGGER.LogInformation("{} shutdown timeout !!! Pending {}", this, trackedCount);
                    Interlocked.Exchange(ref status, (int)CoroutineStatus.Shutdown);
                    return false;
                }
                await Task.Delay((int)Math.Min(remain, 20L));
            }
            LOGGER.LogInformation("{} shutdown success.  Pending {}", this, trackedCount);
            Interlocked.Exchange(ref status, (int)CoroutineStatus.Shutdown);
            return true;
        }


        // Exec will execute tasks off the task list
        private void ExecuteTasks()
        {
            var currentContext = SynchronizationContext.Current;
            var currentCoroutine = CURRENT_COROUTINE.Value;
            try
            {
                var queue = Queue.CurrentFrameQueue;
                if (queue.IsEmpty)
                {
                    return;
                }
                execThread = Thread.CurrentThread;
                CURRENT_COROUTINE.Value = this;
                while (queue.TryDequeue(out var request))
                {
                    var context = request.Context;
                    SynchronizationContext.SetSynchronizationContext(context);
                    request.Invoke();
                }
            } finally
            {
                execThread = null;
                CURRENT_COROUTINE.Value = currentCoroutine;
                SynchronizationContext.SetSynchronizationContext(currentContext);
                Interlocked.Exchange(ref submit, SUBMIT_STATUS_IDLE);
                if (!Queue.IsWorkEmpty)
                {
                    TrySummit();
                }
            }
        }


        public CoroutineStatus Status => (CoroutineStatus)status;


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


        public Task Run(Action action)
        {
            CheckStartStatus();
            var task = new ActionTask(action);
            Context.Post(Invoke, task);
            return task.SourceTask;
        }


        public async Task Exec(CoroutineAction action)
        {
            CheckStartStatus();
            await DoRun(action);
        }


        public Task<T> Run<T>(Func<T> function)
        {
            CheckStartStatus();
            var task = new FuncTask<T>(function);
            Context.Post(Invoke, task);
            return task.SourceTask;
        }


        public async Task<T> Exec<T>(CoroutineFunc<T> function)
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



        private Task DoRun(CoroutineAction action)
        {
            var task = new CoroutineActionTask(action);
            Context.Post(Invoke, task);
            return task.SourceTask;
        }


        private Task<T> DoExec<T>(CoroutineFunc<T> function)
        {
            var task = new CoroutineFuncTask<T>(function);
            Context.Post(Invoke, task);
            return task.SourceTask;
        }


        private static async void Invoke(object value)
        {
            if (value is ICoroutineTask task)
            {
                await task.Invoke();
            }
        }
    }
}
