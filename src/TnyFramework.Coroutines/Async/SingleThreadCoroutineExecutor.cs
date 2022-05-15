using System;
using System.Threading;

namespace TnyFramework.Coroutines.Async
{

    /// <summary>
    /// 线程池协程运行器
    /// </summary>
    public class SingleThreadCoroutineExecutor : DedicatedThreadPoolCoroutineExecutor
    {
        private static volatile ICoroutineExecutor _INSTANCE;

        public static ICoroutineExecutor Default {
            get {
                if (_INSTANCE != null)
                    return _INSTANCE;
                lock (typeof(SingleThreadCoroutineExecutor))
                {
                    if (_INSTANCE != null)
                        return _INSTANCE;
                    _INSTANCE = new SingleThreadCoroutineExecutor("SingleThreadCoroutineExecutor");
                }
                return _INSTANCE;
            }
        }

        protected SingleThreadCoroutineExecutor(string name) : base(name)
        {
        }

        public SingleThreadCoroutineExecutor(string name, Action<Exception> exceptionHandler = null, int threadMaxStackSize = 0)
            : base(1, name, exceptionHandler, threadMaxStackSize)
        {
        }

        public SingleThreadCoroutineExecutor(string name, TimeSpan? deadlockTimeout = null,
            Action<Exception> exceptionHandler = null, int threadMaxStackSize = 0)
            : base(1, name, deadlockTimeout, exceptionHandler, threadMaxStackSize)
        {
        }

        public SingleThreadCoroutineExecutor(string name, TimeSpan? deadlockTimeout = null,
            ApartmentState state = ApartmentState.Unknown, Action<Exception> exceptionHandler = null, int threadMaxStackSize = 0)
            : base(1, name, deadlockTimeout, state, exceptionHandler, threadMaxStackSize)
        {
        }
    }

}
