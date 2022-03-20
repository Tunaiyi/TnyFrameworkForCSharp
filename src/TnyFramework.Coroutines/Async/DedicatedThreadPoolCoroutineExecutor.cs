using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TnyFramework.Common.Logger;
using TnyFramework.Coroutines.Concurrency;

namespace TnyFramework.Coroutines.Async
{
    /// <summary>
    /// 线程池协程运行器
    /// </summary>
    public class DedicatedThreadPoolCoroutineExecutor : ICoroutineExecutor
    {
        private static readonly ILogger LOGGER = LogFactory.Logger<DedicatedThreadPoolCoroutineExecutor>();

        private static readonly Action<System.Exception> DEFAULT_EXCEPTION_HANDLER = exception => {
            LOGGER.LogWarning(exception, "CoroutineExecutor[{}] execute exception", Thread.CurrentThread.Name);
        };

        private readonly DedicatedThreadPool threadPool;


        protected DedicatedThreadPoolCoroutineExecutor(string name)
            : this(Environment.ProcessorCount, name, null, ApartmentState.Unknown, DEFAULT_EXCEPTION_HANDLER)
        {

        }


        public DedicatedThreadPoolCoroutineExecutor(int threads, string name, Action<System.Exception> exceptionHandler = null,
            int threadMaxStackSize = 0)
            : this(threads, name, null, ApartmentState.Unknown, exceptionHandler, threadMaxStackSize)
        {

        }


        public DedicatedThreadPoolCoroutineExecutor(int threads, string name, TimeSpan? deadlockTimeout = null,
            Action<System.Exception> exceptionHandler = null, int threadMaxStackSize = 0)
            : this(threads, name, deadlockTimeout, ApartmentState.Unknown, exceptionHandler, threadMaxStackSize)
        {

        }


        public DedicatedThreadPoolCoroutineExecutor(int threads, string name, TimeSpan? deadlockTimeout = null,
            ApartmentState state = ApartmentState.Unknown, Action<System.Exception> exceptionHandler = null, int threadMaxStackSize = 0)
        {
            var setting = new DedicatedThreadPoolSettings(threads, name, deadlockTimeout, state, exceptionHandler, threadMaxStackSize);
            threadPool = new DedicatedThreadPool(setting);
        }


        public void Summit(Action action)
        {
            threadPool.QueueUserWorkItem(action);
        }


        public void Shutdown()
        {
            threadPool.Dispose();
            threadPool.WaitForThreadsExit();
        }
    }
}
