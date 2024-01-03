// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TnyFramework.Coroutines.ThreadPools;

namespace TnyFramework.Coroutines.TaskSchedulers
{

    public class WorkStealingTaskScheduler : TaskScheduler, IDisposable
    {
        private readonly string name;
        private readonly int mConcurrencyLevel;
        private readonly Queue<Task> mQueue = new();
        private WorkStealingQueue<Task>?[] mWsQueues = new WorkStealingQueue<Task>[Environment.ProcessorCount];
        private readonly Lazy<Thread[]> mThreads;
        private int mThreadsWaiting;
        private bool mShutdown;
        private int tempIndex = 0;

        private readonly IThreadFactory? threadFactory;

        [ThreadStatic]
        private static WorkStealingQueue<Task>? _M_WSQ;

        /// <summary>Initializes a new instance of the WorkStealingTaskScheduler class.</summary>
        /// <remarks>This constructors defaults to using twice as many threads as there are processors.</remarks>
        public WorkStealingTaskScheduler() : this(Environment.ProcessorCount * 2)
        {
        }

        /// <summary>Initializes a new instance of the WorkStealingTaskScheduler class.</summary>
        /// <param name="concurrencyLevel">The number of threads to use in the scheduler.</param>
        /// <param name="name">线程名</param>
        /// <param name="threadFactory"></param>
        public WorkStealingTaskScheduler(int concurrencyLevel, string name = "", IThreadFactory? threadFactory = null)
        {
            this.name = string.IsNullOrEmpty(name) ? "WorkStealingTaskScheduler" : name;
            // Store the concurrency level
            if (concurrencyLevel <= 0) throw new ArgumentOutOfRangeException("concurrencyLevel");
            mConcurrencyLevel = concurrencyLevel;
            this.threadFactory = threadFactory;
            // Set up threads
            mThreads = new Lazy<Thread[]>(() => {
                var threads = new Thread[mConcurrencyLevel];
                for (var i = 0; i < threads.Length; i++)
                {
                    var thread = CreateCoreThread(i, DispatchLoop);
                    threads[i] = thread;
                    threads[i].Start();
                }
                return threads;
            });
        }

        private Thread CreateCoreThread(int index, ThreadStart start)
        {
            Thread thread;
            if (threadFactory != null)
            {
                thread = threadFactory.Create(index, start);
                if (string.IsNullOrEmpty(thread.Name))
                {
                    thread.Name = $"{name}-[Core]-{index}";
                }
            } else
            {
                thread = new Thread(start) {
                    IsBackground = true,
                    Name = $"{name}-[Core]-{index}"
                };
            }
            return thread;
        }

        private Thread CreateLongRunning(int index, ParameterizedThreadStart start)
        {
            Thread thread;
            if (threadFactory != null)
            {
                thread = threadFactory.Create(index, start);
            } else
            {
                thread = new Thread(start) {
                    IsBackground = true,
                    Name = $"{name}-[LongThread]-{index}"
                };
            }
            return thread;
        }

        /// <summary>Queues a task to the scheduler.</summary>
        /// <param name="task">The task to be scheduled.</param>
        protected override void QueueTask(Task task)
        {
            // Make sure the pool is started, e.g. that all threads have been created.
            _ = mThreads.Value;

            // If the task is marked as long-running, give it its own dedicated thread
            // rather than queueing it.
            if ((task.CreationOptions & TaskCreationOptions.LongRunning) != 0)
            {
                var thread = CreateLongRunning(Interlocked.Increment(ref tempIndex),
                    state => TryExecuteTask(state as Task ?? throw new InvalidOperationException()));
                thread.Start(task);
            } else
            {
                // Otherwise, insert the work item into a queue, possibly waking a thread.
                // If there's a local queue and the task does not prefer to be in the global queue,
                // add it to the local queue.
                var wsq = _M_WSQ;
                if (wsq != null && ((task.CreationOptions & TaskCreationOptions.PreferFairness) == 0))
                {
                    // Add to the local queue and notify any waiting threads that work is available.
                    // Races may occur which result in missed event notifications, but they're benign in that
                    // this thread will eventually pick up the work item anyway, as will other threads when another
                    // work item notification is received.
                    wsq.LocalPush(task);
                    if (mThreadsWaiting > 0) // OK to read lock-free.
                    {
                        lock (mQueue)
                        {
                            Monitor.Pulse(mQueue);
                        }
                    }
                }
                // Otherwise, add the work item to the global queue
                else
                {
                    lock (mQueue)
                    {
                        mQueue.Enqueue(task);
                        if (mThreadsWaiting > 0) Monitor.Pulse(mQueue);
                    }
                }
            }
        }

        /// <summary>Executes a task on the current thread.</summary>
        /// <param name="task">The task to be executed.</param>
        /// <param name="taskWasPreviouslyQueued">Ignored.</param>
        /// <returns>Whether the task could be executed.</returns>
        protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
        {
            return TryExecuteTask(task);

            // // Optional replacement: Instead of always trying to execute the task (which could
            // // benignly leave a task in the queue that's already been executed), we
            // // can search the current work-stealing queue and remove the task,
            // // executing it inline only if it's found.
            // WorkStealingQueue<Task> wsq = m_wsq;
            // return wsq != null && wsq.TryFindAndPop(task) && TryExecuteTask(task);
        }

        /// <summary>Gets the maximum concurrency level supported by this scheduler.</summary>
        public override int MaximumConcurrencyLevel => mConcurrencyLevel;

        /// <summary>Gets all of the tasks currently scheduled to this scheduler.</summary>
        /// <returns>An enumerable containing all of the scheduled tasks.</returns>
        protected override IEnumerable<Task> GetScheduledTasks()
        {
            // Keep track of all of the tasks we find
            var tasks = new List<Task>();

            // Get all of the global tasks.  We use TryEnter so as not to hang
            // a debugger if the lock is held by a frozen thread.
            var lockTaken = false;
            try
            {
                Monitor.TryEnter(mQueue, ref lockTaken);
                if (lockTaken) tasks.AddRange(mQueue.ToArray());
                else throw new NotSupportedException();
            } finally
            {
                if (lockTaken) Monitor.Exit(mQueue);
            }

            // Now get all of the tasks from the work-stealing queues
            var queues = mWsQueues;
            for (var i = 0; i < queues.Length; i++)
            {
                var wsq = queues[i];
                if (wsq != null) tasks.AddRange(wsq.ToArray());
            }

            // Return to the debugger all of the collected task instances
            return tasks;
        }

        /// <summary>Adds a work-stealing queue to the set of queues.</summary>
        /// <param name="wsq">The queue to be added.</param>
        private void AddWsq(WorkStealingQueue<Task> wsq)
        {
            lock (mWsQueues)
            {
                // Find the next open slot in the array. If we find one,
                // store the queue and we're done.
                int i;
                for (i = 0; i < mWsQueues.Length; i++)
                {
                    if (mWsQueues[i] == null)
                    {
                        mWsQueues[i] = wsq;
                        return;
                    }
                }

                // We couldn't find an open slot, so double the length
                // of the array by creating a new one, copying over,
                // and storing the new one. Here, i == m_wsQueues.Length.
                WorkStealingQueue<Task>?[] queues = new WorkStealingQueue<Task>[i * 2];
                Array.Copy(mWsQueues, queues, i);
                queues[i] = wsq;
                mWsQueues = queues;
            }
        }

        /// <summary>Remove a work-stealing queue from the set of queues.</summary>
        /// <param name="wsq">The work-stealing queue to remove.</param>
        private void RemoveWsq(WorkStealingQueue<Task>? wsq)
        {
            lock (mWsQueues)
            {
                // Find the queue, and if/when we find it, null out its array slot
                for (var i = 0; i < mWsQueues.Length; i++)
                {
                    if (mWsQueues[i] == wsq)
                    {
                        mWsQueues[i] = null;
                    }
                }
            }
        }

        /// <summary>
        /// The dispatch loop run by each thread in the scheduler.
        /// </summary>
        private void DispatchLoop()
        {
            // Create a new queue for this thread, store it in TLS for later retrieval,
            // and add it to the set of queues for this scheduler.
            var wsq = new WorkStealingQueue<Task>();
            _M_WSQ = wsq;
            AddWsq(wsq);

            try
            {
                // Until there's no more work to do...
                while (true)
                {
                    Task wi = null!;

                    // Search order: (1) local WSQ, (2) global Q, (3) steals from other queues.
                    if (!wsq.LocalPop(ref wi))
                    {
                        // We weren't able to get a task from the local WSQ
                        var searchedForSteals = false;
                        while (true)
                        {
                            lock (mQueue)
                            {
                                // If shutdown was requested, exit the thread.
                                if (mShutdown)
                                    return;

                                // (2) try the global queue.
                                if (mQueue.Count != 0)
                                {
                                    // We found a work item! Grab it ...
                                    wi = mQueue.Dequeue();
                                    break;
                                } else if (searchedForSteals)
                                {
                                    // Note that we're not waiting for work, and then wait
                                    mThreadsWaiting++;
                                    try
                                    {
                                        Monitor.Wait(mQueue);
                                    } finally
                                    {
                                        mThreadsWaiting--;
                                    }

                                    // If we were signaled due to shutdown, exit the thread.
                                    if (mShutdown)
                                        return;

                                    searchedForSteals = false;
                                    continue;
                                }
                            }

                            // (3) try to steal.
                            var wsQueues = mWsQueues;
                            int i;
                            for (i = 0; i < wsQueues.Length; i++)
                            {
                                WorkStealingQueue<Task>? q = wsQueues[i];
                                if (q != null && q != wsq && q.TrySteal(ref wi)) break;
                            }

                            if (i != wsQueues.Length) break;

                            searchedForSteals = true;
                        }
                    }

                    // ...and Invoke it.
                    TryExecuteTask(wi);
                }
            } finally
            {
                RemoveWsq(wsq);
            }
        }

        /// <summary>Signal the scheduler to shutdown and wait for all threads to finish.</summary>
        public void Dispose()
        {
            mShutdown = true;
            if (!mThreads.IsValueCreated) return;
            var threads = mThreads.Value;
            lock (mQueue) Monitor.PulseAll(mQueue);
            for (var i = 0; i < threads.Length; i++)
            {
                threads[i].Join();
            }
        }
    }

    /// <summary>A work-stealing queue.</summary>
    /// <typeparam name="T">Specifies the type of data stored in the queue.</typeparam>
    internal class WorkStealingQueue<T> where T : class
    {
        private const int INITIAL_SIZE = 32;
        private T[] mArray = new T[INITIAL_SIZE];
        private int mMask = INITIAL_SIZE - 1;
        private volatile int mHeadIndex = 0;
        private volatile int mTailIndex = 0;

        private readonly object mForeignLock = new object();

        internal void LocalPush(T obj)
        {
            var tail = mTailIndex;

            // When there are at least 2 elements' worth of space, we can take the fast path.
            if (tail < mHeadIndex + mMask)
            {
                mArray[tail & mMask] = obj;
                mTailIndex = tail + 1;
            } else
            {
                // We need to contend with foreign pops, so we lock.
                lock (mForeignLock)
                {
                    var head = mHeadIndex;
                    var count = mTailIndex - mHeadIndex;

                    // If there is still space (one left), just add the element.
                    if (count >= mMask)
                    {
                        // We're full; expand the queue by doubling its size.
                        T[] newArray = new T[mArray.Length << 1];
                        for (var i = 0; i < mArray.Length; i++)
                            newArray[i] = mArray[(i + head) & mMask];

                        // Reset the field values, incl. the mask.
                        mArray = newArray;
                        mHeadIndex = 0;
                        mTailIndex = tail = count;
                        mMask = (mMask << 1) | 1;
                    }

                    mArray[tail & mMask] = obj;
                    mTailIndex = tail + 1;
                }
            }
        }

        internal bool LocalPop(ref T obj)
        {
            while (true)
            {
                // Decrement the tail using a fence to ensure subsequent read doesn't come before.
                var tail = mTailIndex;
                if (mHeadIndex >= tail)
                {
                    obj = null!;
                    return false;
                }

                tail -= 1;
#pragma warning disable 0420
                Interlocked.Exchange(ref mTailIndex, tail);
#pragma warning restore 0420

                // If there is no interaction with a take, we can head down the fast path.
                if (mHeadIndex <= tail)
                {
                    var idx = tail & mMask;
                    obj = mArray[idx];

                    // Check for nulls in the array.
                    if (obj == null)
                    {
                        continue;
                    }

                    mArray[idx] = null!;
                    return true;
                } else
                {
                    // Interaction with takes: 0 or 1 elements left.
                    lock (mForeignLock)
                    {
                        if (mHeadIndex <= tail)
                        {
                            // Element still available. Take it.
                            var idx = tail & mMask;
                            obj = mArray[idx];

                            // Check for nulls in the array.
                            if (obj == null)
                            {
                                continue;
                            }

                            mArray[idx] = null!;
                            return true;
                        } else
                        {
                            // We lost the race, element was stolen, restore the tail.
                            mTailIndex = tail + 1;
                            obj = null!;
                            return false;
                        }
                    }
                }
            }
        }

        internal bool TrySteal(ref T obj)
        {
            obj = null!;

            while (true)
            {
                if (mHeadIndex >= mTailIndex)
                    return false;

                lock (mForeignLock)
                {
                    // Increment head, and ensure read of tail doesn't move before it (fence).
                    var head = mHeadIndex;
#pragma warning disable 0420
                    Interlocked.Exchange(ref mHeadIndex, head + 1);
#pragma warning restore 0420

                    if (head < mTailIndex)
                    {
                        var idx = head & mMask;
                        obj = mArray[idx];

                        // Check for nulls in the array.
                        if (obj == null)
                        {
                            continue;
                        }

                        mArray[idx] = null!;
                        return true;
                    } else
                    {
                        // Failed, restore head.
                        mHeadIndex = head;
                        obj = null!;
                    }
                }

                return false;
            }
        }

        internal bool TryFindAndPop(T obj)
        {
            // We do an O(N) search for the work item. The theory of work stealing and our
            // inlining logic is that most waits will happen on recently queued work.  And
            // since recently queued work will be close to the tail end (which is where we
            // begin our search), we will likely find it quickly.  In the worst case, we
            // will traverse the whole local queue; this is typically not going to be a
            // problem (although degenerate cases are clearly an issue) because local work
            // queues tend to be somewhat shallow in length, and because if we fail to find
            // the work item, we are about to block anyway (which is very expensive).

            for (var i = mTailIndex - 1; i >= mHeadIndex; i--)
            {
                if (mArray[i & mMask] == obj)
                {
                    // If we found the element, block out steals to avoid interference.
                    lock (mForeignLock)
                    {
                        // If we lost the race, bail.
                        if (mArray[i & mMask] == null)
                        {
                            return false;
                        }

                        // Otherwise, null out the element.
                        mArray[i & mMask] = null!;

                        // And then check to see if we can fix up the indexes (if we're at
                        // the edge).  If we can't, we just leave nulls in the array and they'll
                        // get filtered out eventually (but may lead to superflous resizing).
                        if (i == mTailIndex)
                            mTailIndex -= 1;
                        else if (i == mHeadIndex)
                            mHeadIndex += 1;

                        return true;
                    }
                }
            }

            return false;
        }

        internal T[] ToArray()
        {
            var list = new List<T>();
            for (var i = mTailIndex - 1; i >= mHeadIndex; i--)
            {
                var obj = mArray[i & mMask];
                if (obj != null)
                {
                    list.Add(obj);
                }
            }
            return list.ToArray();
        }
    }

}
