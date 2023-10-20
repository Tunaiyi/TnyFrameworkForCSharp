using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using TnyFramework.Common.Extensions;

namespace TnyFramework.Coroutines.ThreadPools
{

    /// <summary>
    /// The type of threads to use - either foreground or background threads.
    /// </summary>
    public enum ThreadType
    {
        Foreground,
        Background
    }

    /// <summary>
    /// Provides settings for a dedicated thread pool
    /// </summary>
    public class DedicatedThreadPoolSettings
    {
        /// <summary>
        /// Background threads are the default thread type
        /// </summary>
        private const ThreadType DEFAULT_THREAD_TYPE = ThreadType.Background;

        public DedicatedThreadPoolSettings(int numThreads,
            string? name = null!,
            TimeSpan? deadlockTimeout = null,
            ApartmentState apartmentState = ApartmentState.Unknown,
            Action<Exception>? exceptionHandler = null!,
            int threadMaxStackSize = 0)
            : this(numThreads, DEFAULT_THREAD_TYPE, name, deadlockTimeout, apartmentState, exceptionHandler, threadMaxStackSize)
        {
        }

        public DedicatedThreadPoolSettings(int numThreads,
            ThreadType threadType,
            string? name = null!,
            TimeSpan? deadlockTimeout = null,
            ApartmentState apartmentState = ApartmentState.Unknown,
            Action<Exception>? exceptionHandler = null!,
            int threadMaxStackSize = 0)
        {
            Name = name ?? ("DedicatedThreadPool-" + Guid.NewGuid());
            ThreadType = threadType;
            NumThreads = numThreads;
            DeadlockTimeout = deadlockTimeout;
            ApartmentState = apartmentState;
            ExceptionHandler = exceptionHandler ?? (_ => { });
            ThreadMaxStackSize = threadMaxStackSize;

            if (deadlockTimeout.HasValue && deadlockTimeout.Value.TotalMilliseconds <= 0)
                throw new ArgumentOutOfRangeException(nameof(deadlockTimeout),
                    $"deadlockTimeout must be null or at least 1ms. Was {deadlockTimeout}.");
            if (numThreads <= 0)
                throw new ArgumentOutOfRangeException(nameof(numThreads), $"numThreads must be at least 1. Was {numThreads}");
        }

        /// <summary>
        /// The total number of threads to run in this thread pool.
        /// </summary>
        public int NumThreads { get; private set; }

        /// <summary>
        /// The type of threads to run in this thread pool.
        /// </summary>
        public ThreadType ThreadType { get; private set; }

        /// <summary>
        /// Apartment state for threads to run in this thread pool
        /// </summary>
        public ApartmentState ApartmentState { get; private set; }

        /// <summary>
        /// Interval to check for thread deadlocks.
        /// 
        /// If a thread takes longer than <see cref="DeadlockTimeout"/> it will be aborted
        /// and replaced.
        /// </summary>
        public TimeSpan? DeadlockTimeout { get; private set; }

        public string Name { get; private set; }

        public Action<Exception> ExceptionHandler { get; private set; }

        /// <summary>
        /// Gets the thread stack size, 0 represents the default stack size.
        /// </summary>
        public int ThreadMaxStackSize { get; private set; }
    }

    /// <summary>
    /// TaskScheduler for working with a <see cref="DedicatedThreadPool"/> instance
    /// </summary>
    internal class DedicatedThreadPoolTaskScheduler : TaskScheduler
    {
        // Indicates whether the current thread is processing work items.
        [ThreadStatic]
        private static bool _CURRENT_THREAD_IS_RUNNING_TASKS;

        /// <summary>
        /// Number of tasks currently running
        /// </summary>
        private volatile int parallelWorkers = 0;

        private readonly LinkedList<Task> tasks = new LinkedList<Task>();

        private readonly DedicatedThreadPool pool;

        public DedicatedThreadPoolTaskScheduler(DedicatedThreadPool pool)
        {
            this.pool = pool;
        }

        protected override void QueueTask(Task task)
        {
            lock (tasks)
            {
                tasks.AddLast(task);
            }

            EnsureWorkerRequested();
        }

        protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
        {
            //current thread isn't running any tasks, can't execute inline
            if (!_CURRENT_THREAD_IS_RUNNING_TASKS) return false;

            //remove the task from the queue if it was previously added
            if (taskWasPreviouslyQueued)
                if (TryDequeue(task))
                    return TryExecuteTask(task);
                else
                    return false;
            return TryExecuteTask(task);
        }

        protected override bool TryDequeue(Task task)
        {
            lock (tasks) return tasks.Remove(task);
        }

        /// <summary>
        /// Level of concurrency is directly equal to the number of threads
        /// in the <see cref="DedicatedThreadPool"/>.
        /// </summary>
        public override int MaximumConcurrencyLevel {
            get { return pool.Settings.NumThreads; }
        }

        protected override IEnumerable<Task> GetScheduledTasks()
        {
            var lockTaken = false;
            try
            {
                Monitor.TryEnter(tasks, ref lockTaken);

                //should this be immutable?
                if (lockTaken) return tasks;
                else throw new NotSupportedException();
            } finally
            {
                if (lockTaken) Monitor.Exit(tasks);
            }
        }

        private void EnsureWorkerRequested()
        {
            var count = parallelWorkers;
            while (count < pool.Settings.NumThreads)
            {
                var prev = Interlocked.CompareExchange(ref parallelWorkers, count + 1, count);
                if (prev == count)
                {
                    RequestWorker();
                    break;
                }
                count = prev;
            }
        }

        private void ReleaseWorker()
        {
            var count = parallelWorkers;
            while (count > 0)
            {
                var prev = Interlocked.CompareExchange(ref parallelWorkers, count - 1, count);
                if (prev == count)
                {
                    break;
                }
                count = prev;
            }
        }

        private void RequestWorker()
        {
            pool.QueueUserWorkItem(() => {
                // this thread is now available for inlining
                _CURRENT_THREAD_IS_RUNNING_TASKS = true;
                try
                {
                    // Process all available items in the queue.
                    while (true)
                    {
                        Task item;
                        lock (tasks)
                        {
                            // done processing
                            if (tasks.Count == 0)
                            {
                                ReleaseWorker();
                                break;
                            }

                            // Get the next item from the queue
                            item = tasks.First?.Value!;
                            tasks.RemoveFirst();
                        }

                        // Execute the task we pulled out of the queue
                        TryExecuteTask(item);
                    }
                }
                // We're done processing items on the current thread
                finally
                {
                    _CURRENT_THREAD_IS_RUNNING_TASKS = false;
                }
            });
        }
    }

    /// <summary>
    /// An instanced, dedicated thread pool.
    /// </summary>
    public sealed class DedicatedThreadPool : IDisposable
    {
        public DedicatedThreadPool(DedicatedThreadPoolSettings settings)
        {
            workQueue = new ThreadPoolWorkQueue();
            Settings = settings;
            workers = Enumerable.Range(1, settings.NumThreads).Select(workerId => new PoolWorker(this, workerId)).ToArray();

            // Note:
            // The DedicatedThreadPoolSupervisor was removed because aborting thread could lead to unexpected behavior
            // If a new implementation is done, it should spawn a new thread when a worker is not making progress and
            // try to keep {settings.NumThreads} active threads.
        }

        public DedicatedThreadPoolSettings Settings { get; private set; }

        private readonly ThreadPoolWorkQueue workQueue;
        private readonly PoolWorker[] workers;

        public bool QueueUserWorkItem(Action work)
        {
            if (work == null)
                throw new ArgumentNullException("work");

            return workQueue.TryAdd(work);
        }

        public void Dispose()
        {
            workQueue.CompleteAdding();
        }

        public void WaitForThreadsExit()
        {
            WaitForThreadsExit(Timeout.InfiniteTimeSpan);
        }

        public void WaitForThreadsExit(TimeSpan timeout)
        {
            Task.WaitAll(workers.Select(worker => worker.ThreadExit).ToArray(), timeout);
        }

        private class PoolWorker
        {
            private readonly DedicatedThreadPool pool;

            private readonly TaskCompletionSource<object> threadExit;

            public Task ThreadExit => threadExit.Task;

            public PoolWorker(DedicatedThreadPool pool, int workerId)
            {
                this.pool = pool;
                threadExit = new TaskCompletionSource<object>();

                var thread = new Thread(RunThread, pool.Settings.ThreadMaxStackSize);

                thread.IsBackground = pool.Settings.ThreadType == ThreadType.Background;

                if (pool.Settings.Name.IsNotBlank())
                    thread.Name = $"{pool.Settings.Name}_{workerId}";
                if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    if (pool.Settings.ApartmentState != ApartmentState.Unknown)
#pragma warning disable CA1416
                        thread.SetApartmentState(pool.Settings.ApartmentState);
#pragma warning restore CA1416
                }

                thread.Start();
            }

            private void RunThread()
            {
                try
                {
                    foreach (var action in pool.workQueue.GetConsumingEnumerable())
                    {
                        try
                        {
                            action();
                        } catch (Exception ex)
                        {
                            pool.Settings.ExceptionHandler(ex);
                        }
                    }
                } finally
                {
                    threadExit.TrySetResult(null!);
                }
            }
        }

        private class ThreadPoolWorkQueue
        {
            private static readonly int PROCESSOR_COUNT = Environment.ProcessorCount;
            private const int COMPLETED_STATE = 1;

            private readonly ConcurrentQueue<Action> queue = new ConcurrentQueue<Action>();
            private readonly UnfairSemaphore semaphore = new UnfairSemaphore();
            private int outstandingRequests;
            private int isAddingCompleted;

            public bool IsAddingCompleted {
                get { return Volatile.Read(ref isAddingCompleted) == COMPLETED_STATE; }
            }

            public bool TryAdd(Action work)
            {
                // If TryAdd returns true, it's garanteed the work item will be executed.
                // If it returns false, it's also garanteed the work item won't be executed.

                if (IsAddingCompleted)
                    return false;

                queue.Enqueue(work);
                EnsureThreadRequested();

                return true;
            }

            public IEnumerable<Action> GetConsumingEnumerable()
            {
                while (true)
                {
                    if (queue.TryDequeue(out var work))
                    {
                        yield return work;
                    } else if (IsAddingCompleted)
                    {
                        while (queue.TryDequeue(out work))
                            yield return work;

                        break;
                    } else
                    {
                        semaphore.Wait();
                        MarkThreadRequestSatisfied();
                    }
                }
            }

            public void CompleteAdding()
            {
                var previousCompleted = Interlocked.Exchange(ref isAddingCompleted, COMPLETED_STATE);

                if (previousCompleted == COMPLETED_STATE)
                    return;

                // When CompleteAdding() is called, we fill up the _outstandingRequests and the semaphore
                // This will ensure that all threads will unblock and try to execute the remaining item in
                // the queue. When IsAddingCompleted is set, all threads will exit once the queue is empty.

                while (true)
                {
                    var count = Volatile.Read(ref outstandingRequests);
                    var countToRelease = UnfairSemaphore.MAX_WORKER - count;

                    var prev = Interlocked.CompareExchange(ref outstandingRequests, UnfairSemaphore.MAX_WORKER, count);

                    if (prev == count)
                    {
                        semaphore.Release((short) countToRelease);
                        break;
                    }
                }
            }

            private void EnsureThreadRequested()
            {
                // There is a double counter here (_outstandingRequest and _semaphore)
                // Unfair semaphore does not support value bigger than short.MaxValue,
                // tring to Release more than short.MaxValue could fail miserably.

                // The _outstandingRequest counter ensure that we only request a
                // maximum of {ProcessorCount} to the semaphore.

                // It's also more efficient to have two counter, _outstandingRequests is
                // more lightweight than the semaphore.

                // This trick is borrowed from the .Net ThreadPool
                // https://github.com/dotnet/coreclr/blob/bc146608854d1db9cdbcc0b08029a87754e12b49/src/mscorlib/src/System/Threading/ThreadPool.cs#L568

                var count = Volatile.Read(ref outstandingRequests);
                while (count < PROCESSOR_COUNT)
                {
                    var prev = Interlocked.CompareExchange(ref outstandingRequests, count + 1, count);
                    if (prev == count)
                    {
                        semaphore.Release();
                        break;
                    }
                    count = prev;
                }
            }

            private void MarkThreadRequestSatisfied()
            {
                var count = Volatile.Read(ref outstandingRequests);
                while (count > 0)
                {
                    var prev = Interlocked.CompareExchange(ref outstandingRequests, count - 1, count);
                    if (prev == count)
                    {
                        break;
                    }
                    count = prev;
                }
            }
        }

        // This class has been translated from:
        // https://github.com/dotnet/coreclr/blob/97433b9d153843492008652ff6b7c3bf4d9ff31c/src/vm/win32threadpool.h#L124

        // UnfairSemaphore is a more scalable semaphore than Semaphore.  It prefers to release threads that have more recently begun waiting,
        // to preserve locality.  Additionally, very recently-waiting threads can be released without an addition kernel transition to unblock
        // them, which reduces latency.
        //
        // UnfairSemaphore is only appropriate in scenarios where the order of unblocking threads is not important, and where threads frequently
        // need to be woken.

        [StructLayout(LayoutKind.Sequential)]
        private sealed class UnfairSemaphore
        {
            public const int MAX_WORKER = 0x7FFF;

            // We track everything we care about in A 64-bit struct to allow us to
            // do CompareExchanges on this for atomic updates.
            [StructLayout(LayoutKind.Explicit)]
            private struct SemaphoreState
            {
                //how many threads are currently spin-waiting for this semaphore?
                [FieldOffset(0)]
                public short Spinners;

                //how much of the semaphore's count is availble to spinners?
                [FieldOffset(2)]
                public short CountForSpinners;

                //how many threads are blocked in the OS waiting for this semaphore?
                [FieldOffset(4)]
                public short Waiters;

                //how much count is available to waiters?
                [FieldOffset(6)]
                public short CountForWaiters;

                [FieldOffset(0)]
                public long RawData;
            }

            [StructLayout(LayoutKind.Explicit, Size = 64)]
            private struct CacheLinePadding
            {
            }

            private readonly Semaphore m_semaphore;

            // padding to ensure we get our own cache line
#pragma warning disable 169
            private readonly CacheLinePadding m_padding1;
            private SemaphoreState m_state;
            private readonly CacheLinePadding m_padding2;
#pragma warning restore 169

            public UnfairSemaphore()
            {
                m_semaphore = new Semaphore(0, short.MaxValue);
            }

            public bool Wait()
            {
                return Wait(Timeout.InfiniteTimeSpan);
            }

            public bool Wait(TimeSpan timeout)
            {
                while (true)
                {
                    var currentCounts = GetCurrentState();
                    var newCounts = currentCounts;

                    // First, just try to grab some count.
                    if (currentCounts.CountForSpinners > 0)
                    {
                        --newCounts.CountForSpinners;
                        if (TryUpdateState(newCounts, currentCounts))
                            return true;
                    } else
                    {
                        // No count available, become a spinner
                        ++newCounts.Spinners;
                        if (TryUpdateState(newCounts, currentCounts))
                            break;
                    }
                }

                //
                // Now we're a spinner.
                //
                var numSpins = 0;
                const int spinLimitPerProcessor = 50;
                while (true)
                {
                    var currentCounts = GetCurrentState();
                    var newCounts = currentCounts;

                    if (currentCounts.CountForSpinners > 0)
                    {
                        --newCounts.CountForSpinners;
                        --newCounts.Spinners;
                        if (TryUpdateState(newCounts, currentCounts))
                            return true;
                    } else
                    {
                        var spinnersPerProcessor = (double) currentCounts.Spinners / Environment.ProcessorCount;
                        var spinLimit = (int) ((spinLimitPerProcessor / spinnersPerProcessor) + 0.5);
                        if (numSpins >= spinLimit)
                        {
                            --newCounts.Spinners;
                            ++newCounts.Waiters;
                            if (TryUpdateState(newCounts, currentCounts))
                                break;
                        } else
                        {
                            //
                            // We yield to other threads using Thread.Sleep(0) rather than the more traditional Thread.Yield().
                            // This is because Thread.Yield() does not yield to threads currently scheduled to run on other
                            // processors.  On a 4-core machine, for example, this means that Thread.Yield() is only ~25% likely
                            // to yield to the correct thread in some scenarios.
                            // Thread.Sleep(0) has the disadvantage of not yielding to lower-priority threads.  However, this is ok because
                            // once we've called this a few times we'll become a "waiter" and wait on the Semaphore, and that will
                            // yield to anything that is runnable.
                            //
                            Thread.Sleep(0);
                            numSpins++;
                        }
                    }
                }

                //
                // Now we're a waiter
                //
                var waitSucceeded = m_semaphore.WaitOne(timeout);

                while (true)
                {
                    var currentCounts = GetCurrentState();
                    var newCounts = currentCounts;

                    --newCounts.Waiters;

                    if (waitSucceeded)
                        --newCounts.CountForWaiters;

                    if (TryUpdateState(newCounts, currentCounts))
                        return waitSucceeded;
                }
            }

            public void Release()
            {
                Release(1);
            }

            public void Release(short count)
            {
                while (true)
                {
                    var currentState = GetCurrentState();
                    var newState = currentState;

                    var remainingCount = count;

                    // First, prefer to release existing spinners,
                    // because a) they're hot, and b) we don't need a kernel
                    // transition to release them.
                    var spinnersToRelease = Math.Max((short) 0,
                        Math.Min(remainingCount, (short) (currentState.Spinners - currentState.CountForSpinners)));
                    newState.CountForSpinners += spinnersToRelease;
                    remainingCount -= spinnersToRelease;

                    // Next, prefer to release existing waiters
                    var waitersToRelease = Math.Max((short) 0,
                        Math.Min(remainingCount, (short) (currentState.Waiters - currentState.CountForWaiters)));
                    newState.CountForWaiters += waitersToRelease;
                    remainingCount -= waitersToRelease;

                    // Finally, release any future spinners that might come our way
                    newState.CountForSpinners += remainingCount;

                    // Try to commit the transaction
                    if (TryUpdateState(newState, currentState))
                    {
                        // Now we need to release the waiters we promised to release
                        if (waitersToRelease > 0)
                            m_semaphore.Release(waitersToRelease);

                        break;
                    }
                }
            }

            private bool TryUpdateState(SemaphoreState newState, SemaphoreState currentState)
            {
                if (Interlocked.CompareExchange(ref m_state.RawData, newState.RawData, currentState.RawData) == currentState.RawData)
                {
                    Debug.Assert(newState.CountForSpinners <= MAX_WORKER, "CountForSpinners is greater than MaxWorker");
                    Debug.Assert(newState.CountForSpinners >= 0, "CountForSpinners is lower than zero");
                    Debug.Assert(newState.Spinners <= MAX_WORKER, "Spinners is greater than MaxWorker");
                    Debug.Assert(newState.Spinners >= 0, "Spinners is lower than zero");
                    Debug.Assert(newState.CountForWaiters <= MAX_WORKER, "CountForWaiters is greater than MaxWorker");
                    Debug.Assert(newState.CountForWaiters >= 0, "CountForWaiters is lower than zero");
                    Debug.Assert(newState.Waiters <= MAX_WORKER, "Waiters is greater than MaxWorker");
                    Debug.Assert(newState.Waiters >= 0, "Waiters is lower than zero");
                    Debug.Assert(newState.CountForSpinners + newState.CountForWaiters <= MAX_WORKER,
                        "CountForSpinners + CountForWaiters is greater than MaxWorker");

                    return true;
                }

                return false;
            }

            private SemaphoreState GetCurrentState()
            {
                // Volatile.Read of a long can get a partial read in x86 but the invalid
                // state will be detected in TryUpdateState with the CompareExchange.

                var state = new SemaphoreState();
                state.RawData = Volatile.Read(ref m_state.RawData);
                return state;
            }
        }
    }

}
