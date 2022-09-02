using System;
using System.Threading.Tasks;

namespace TnyFramework.Coroutines.Async
{

    public static class AsyncExecutorExtensions
    {
        /// <summary>
        /// 在协程上运行一个action
        /// </summary>
        /// <param name="executor">执行器</param>
        /// <param name="action">action</param>
        public static void ExecAction(this IAsyncExecutor executor, Action action)
        {
            executor.AsyncExec(ActionAsyncHandle(action));
        }

        /// <summary>
        /// 在协程上运行一个function
        /// </summary>
        /// <param name="executor">执行器</param>
        /// <param name="function">任务</param>
        /// <typeparam name="T">类型</typeparam>
        public static void ExecFunc<T>(this IAsyncExecutor executor, Func<T> function)
        {
            executor.AsyncExec(FuncAsyncHandle(function));
        }

        /// <summary>
        /// 在协程上运行一个action
        /// </summary>
        /// <param name="executor">执行器</param>
        /// <param name="handle">action</param>
        public static void Exec(this IAsyncExecutor executor, AsyncHandle handle)
        {
            executor.AsyncExec(handle);
        }

        /// <summary>
        /// 在协程上运行一个function
        /// </summary>
        /// <param name="executor">执行器</param>
        /// <param name="function">任务</param>
        /// <typeparam name="T">类型</typeparam>
        public static void Exec<T>(this IAsyncExecutor executor, AsyncHandle<T> function)
        {
            executor.AsyncExec(function);
        }

        /// <summary>
        /// 在协程上运行一个action
        /// </summary>
        /// <param name="executor">执行器</param> 
        /// <param name="action">action</param>
        /// <returns>等待的任务</returns>
        public static Task AsyncAction(this IAsyncExecutor executor, Action action)
        {
            return executor.AsyncExec(ActionAsyncHandle(action));
        }

        /// <summary>
        /// <![CDATA[在协程上运行一个返回 task<T> 的任务]]>
        /// </summary>
        /// <param name="executor">执行器</param>
        /// <param name="function">任务</param>
        /// <typeparam name="T">类型</typeparam>
        /// <returns>等待的任务</returns>
        public static Task<T> AsyncFunc<T>(this IAsyncExecutor executor, Func<T> function)
        {
            return executor.AsyncExec(FuncAsyncHandle(function));
        }

        private static AsyncHandle ActionAsyncHandle(Action action)
        {
            return () => ActionTask(action);
        }

        private static Task ActionTask(Action action)
        {
            action();
            return Task.CompletedTask;
        }

        private static AsyncHandle<T> FuncAsyncHandle<T>(Func<T> func)
        {
            return () => Task.FromResult(func());
        }
    }

}
