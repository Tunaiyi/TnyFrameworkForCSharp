using System;
using System.Threading;
using Microsoft.Extensions.Logging;
using TnyFramework.Common.Logger;

namespace TnyFramework.Coroutines.Async
{

    /// <summary>
    /// 协程SynchronizationContext
    /// </summary>
    public class CoroutineSynchronizationContext : SynchronizationContext
    {
        /// <summary>
        /// 协程对象
        /// </summary>
        private readonly Coroutine coroutine;

        /// <summary>
        /// 初始化 Context
        /// </summary>
        public static void InitializeSynchronizationContext()
        {
            Coroutine.InitializeSynchronizationContext();
        }

        /// <summary>
        /// 初始化 Context
        /// </summary>
        public static void InitializeSynchronizationContext(ICoroutine coroutine)
        {
            Coroutine.InitializeSynchronizationContext(coroutine);
        }

        internal CoroutineSynchronizationContext(Coroutine coroutine)
        {
            this.coroutine = coroutine;
        }

        private int Id => coroutine.Id;

        private string Name => coroutine.Name;

        public override SynchronizationContext CreateCopy()
        {
            return new CoroutineSynchronizationContext(coroutine);
        }

        public override void OperationStarted()
        {
            coroutine.Track();
        }

        public override void OperationCompleted()
        {
            coroutine.Tracked();
        }

        public override void Post(SendOrPostCallback callback, object state)
        {
            coroutine.Post(this, callback, state);
        }

        public override void Send(SendOrPostCallback callback, object state)
        {
            coroutine.Send(this, callback, state);
        }

        public override string ToString()
        {
            return $"{coroutine}";
        }
    }

    public class CoroutineWork
    {
        private static readonly ILogger LOGGER = LogFactory.Logger<CoroutineWork>();

        private readonly SendOrPostCallback callback;

        private readonly object state;

        private readonly ManualResetEvent awaitHandler;

        private readonly Exception exception;

        public CoroutineWork(SendOrPostCallback callback, SynchronizationContext context, object state, ManualResetEvent awaitHandler)
            : this(callback, context, state, null, awaitHandler)
        {
        }

        public CoroutineWork(SendOrPostCallback callback, SynchronizationContext context, object state,
            Exception exception = null, ManualResetEvent awaitHandler = null)
        {
            Context = context;
            this.callback = callback;
            this.state = state;
            this.awaitHandler = awaitHandler;
            this.exception = exception;
        }

        internal SynchronizationContext Context { get; }

        public void Invoke()
        {
            try
            {
                callback(state);
            } catch (Exception e)
            {
                var thread = Thread.CurrentThread;
                LOGGER.LogError(e, "ThreadName {TName} - {TId} Run exception", thread.Name, thread.ManagedThreadId);
            } finally
            {
                awaitHandler?.Set();
            }
        }
    }

}
