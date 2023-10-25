// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TnyFramework.Coroutines.TaskSchedulers
{

    /// <summary>
    /// 任务可控制并发度任务调度器，任务并行执行。
    /// </summary>
    public class LimitedConcurrencyLevelTaskScheduler : TaskScheduler
    {
        /// <summary>Whether the current thread is processing work items.</summary>
        [ThreadStatic]
        private static bool _CURRENT_THREAD_IS_PROCESSING_ITEMS;

        /// <summary>The list of tasks to be executed.</summary>
        private readonly ConcurrentQueue<Task> tasks = new(); // protected by lock(_tasks)

        /// <summary>The maximum concurrency level allowed by this scheduler.</summary>
        private readonly int maxDegreeOfParallelism;

        /// <summary>Whether the scheduler is currently processing work items.</summary>
        private volatile int delegatesQueuedOrRunning; // protected by lock(_tasks)

        /// <summary>
        /// Initializes an instance of the LimitedConcurrencyLevelTaskScheduler class with the
        /// specified degree of parallelism.
        /// </summary>
        /// <param name="maxDegreeOfParallelism">The maximum degree of parallelism provided by this scheduler.</param>
        public LimitedConcurrencyLevelTaskScheduler(int maxDegreeOfParallelism)
        {
            if (maxDegreeOfParallelism < 1) throw new ArgumentOutOfRangeException(nameof(maxDegreeOfParallelism));
            this.maxDegreeOfParallelism = maxDegreeOfParallelism;
        }

        /// <summary>Queues a task to the scheduler.</summary>
        /// <param name="task">The task to be queued.</param>
        protected sealed override void QueueTask(Task task)
        {
            // Add the task to the list of tasks to be processed.  If there aren't enough
            // delegates currently queued or running to process tasks, schedule another.
            tasks.Enqueue(task);
            while (true)
            {
                var running = delegatesQueuedOrRunning;
                if (running >= maxDegreeOfParallelism)
                {
                    return;
                }
                if (Interlocked.CompareExchange(ref delegatesQueuedOrRunning, running + 1, running) == running)
                {
                    NotifyThreadPoolOfPendingWork();
                }
            }
        }

        /// <summary>
        /// Informs the ThreadPool that there's work to be executed for this scheduler.
        /// </summary>
        private void NotifyThreadPoolOfPendingWork()
        {
            ThreadPool.UnsafeQueueUserWorkItem(_ => {
                // Note that the current thread is now processing work items.
                // This is necessary to enable inlining of tasks into this thread.
                _CURRENT_THREAD_IS_PROCESSING_ITEMS = true;
                try
                {
                    // Process all available items in the queue.
                    while (true)
                    {
                        if (tasks.TryDequeue(out var item))
                        {
                            if (item.IsCompleted)
                            {
                                continue;
                            }
                            // Execute the task we pulled out of the queue
                            TryExecuteTask(item);
                            continue;
                        }
                        break;
                    }
                }
                // We're done processing items on the current thread
                finally
                {
                    Interlocked.Decrement(ref delegatesQueuedOrRunning);
                    _CURRENT_THREAD_IS_PROCESSING_ITEMS = false;
                }
            }, null);
        }

        /// <summary>Attempts to execute the specified task on the current thread.</summary>
        /// <param name="task">The task to be executed.</param>
        /// <param name="taskWasPreviouslyQueued"></param>
        /// <returns>Whether the task could be executed on the current thread.</returns>
        protected sealed override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
        {
            // If this thread isn't already processing a task, we don't support inlining
            if (!_CURRENT_THREAD_IS_PROCESSING_ITEMS) return false;

            // If the task was previously queued, remove it from the queue
            // if (taskWasPreviouslyQueued)
            // {
            //     TryDequeue(task);
            // }

            // Try to run the task.
            return TryExecuteTask(task);
        }

        /// <summary>Attempts to remove a previously scheduled task from the scheduler.</summary>
        /// <param name="task">The task to be removed.</param>
        /// <returns>Whether the task could be found and removed.</returns>
        // protected sealed override bool TryDequeue(Task task)
        // {
        //     lock (tasks) return tasks.Remove(task);
        // }

        /// <summary>Gets the maximum concurrency level supported by this scheduler.</summary>
        public sealed override int MaximumConcurrencyLevel => maxDegreeOfParallelism;

        /// <summary>Gets an enumerable of the tasks currently scheduled on this scheduler.</summary>
        /// <returns>An enumerable of the tasks currently scheduled.</returns>
        protected sealed override IEnumerable<Task> GetScheduledTasks()
        {
            var lockTaken = false;
            try
            {
                Monitor.TryEnter(tasks, ref lockTaken);
                if (lockTaken) return tasks.ToArray();
                else throw new NotSupportedException();
            } finally
            {
                if (lockTaken) Monitor.Exit(tasks);
            }
        }
    }

}
