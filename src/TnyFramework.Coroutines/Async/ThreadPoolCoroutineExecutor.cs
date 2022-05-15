using System;
using System.Threading.Tasks;

namespace TnyFramework.Coroutines.Async
{

    /// <summary>
    /// 线程池协程运行器
    /// </summary>
    public class ThreadPoolCoroutineExecutor : ICoroutineExecutor
    {
        private ThreadPoolCoroutineExecutor()
        {
        }

        public static ICoroutineExecutor Default { get; } = new ThreadPoolCoroutineExecutor();

        public void Summit(Action action)
        {
            Task.Run(action);
        }
    }

}
