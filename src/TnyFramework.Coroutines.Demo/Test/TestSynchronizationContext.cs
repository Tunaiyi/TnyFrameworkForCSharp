using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Microsoft.Extensions.Logging;
using TnyFramework.Common.Logger;
namespace TnyFramework.Coroutines.Demo.Test
{
    public class WorkRequest
    {
        private SendOrPostCallback callback;

        private object state;


        public WorkRequest(SendOrPostCallback callback, object state)
        {
            this.callback = callback;
            this.state = state;
        }


        public void Invoke()
        {
            callback(state);
        }
    }

    public class TestSynchronizationContext : SynchronizationContext
    {
        private static readonly ILogger LOGGER = LogFactory.Logger<TestSynchronizationContext>();


        private readonly List<WorkRequest> m_AsyncWorkQueue;
        private readonly List<WorkRequest> m_CurrentFrameWork = new List<WorkRequest>(20);

        private int m_TrackedCount = 0;


        public TestSynchronizationContext()
        {
            m_AsyncWorkQueue = new List<WorkRequest>(20);
        }


        public TestSynchronizationContext(List<WorkRequest> mAsyncWorkQueue)
        {
            m_AsyncWorkQueue = mAsyncWorkQueue;
        }


        public override SynchronizationContext CreateCopy()
        {
            LOGGER.LogInformation("CreateCopy {TheadId}", Thread.CurrentThread.ManagedThreadId);
            return new TestSynchronizationContext(this.m_AsyncWorkQueue);
        }


        public override void OperationStarted()
        {
            Interlocked.Increment(ref m_TrackedCount);
            LOGGER.LogInformation("OperationStarted {ThreadId}", Thread.CurrentThread.ManagedThreadId);
        }


        public override void OperationCompleted()
        {
            Interlocked.Decrement(ref m_TrackedCount);
            LOGGER.LogInformation("OperationCompleted {ThreadId}", Thread.CurrentThread.ManagedThreadId);
        }


        public override void Post(SendOrPostCallback callback, object state)
        {
            LOGGER.LogInformation("Post >> {State} = {ThreadId}", state, Thread.CurrentThread.ManagedThreadId);
            lock (m_AsyncWorkQueue)
            {
                m_AsyncWorkQueue.Add(new WorkRequest(callback, state));
            }
        }


        public override void Send(SendOrPostCallback d, object state)
        {
            LOGGER.LogInformation("Send {ThreadId}", Thread.CurrentThread.ManagedThreadId);
        }


        public static void Context()
        {
            if (Current == null)
            {
                SetSynchronizationContext(new TestSynchronizationContext());
            }
        }


        private bool HasPendingTasks()
        {
            return m_AsyncWorkQueue.Count != 0 || m_TrackedCount != 0;
        }


        // SynchronizationContext must be set before any user code is executed. This is done on
        // Initial domain load and domain reload at MonoManager ReloadAssembly
        private static void InitializeSynchronizationContext()
        {
            SetSynchronizationContext(new TestSynchronizationContext());
        }


        // Exec will execute tasks off the task list
        private void Exec()
        {
            lock (m_AsyncWorkQueue)
            {
                m_CurrentFrameWork.AddRange(m_AsyncWorkQueue);
                m_AsyncWorkQueue.Clear();
            }

            // When you invoke work, remove it from the list to stop it being triggered again (case 1213602)
            while (m_CurrentFrameWork.Count > 0)
            {
                LOGGER.LogInformation(message: "====== m_AsyncWorkQueue {Count}", m_CurrentFrameWork.Count);
                var work = m_CurrentFrameWork[0];
                m_CurrentFrameWork.RemoveAt(0);
                work.Invoke();
            }
        }


        // All requests must be processed on the main thread where the full Unity API is available
        // See ScriptRunDelayedTasks in PlayerLoopCallbacks.h
        public static void ExecuteTasks()
        {
            if (Current is TestSynchronizationContext context)
                context.Exec();
        }


        private static bool ExecutePendingTasks(long millisecondsTimeout)
        {
            if (!(Current is TestSynchronizationContext context))
            {
                return true;
            }

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            while (context.HasPendingTasks())
            {
                if (stopwatch.ElapsedMilliseconds > millisecondsTimeout)
                {
                    break;
                }

                context.Exec();
                Thread.Sleep(1);
            }

            return !context.HasPendingTasks();
        }
    }
}
