using System;
using System.Threading.Tasks;

namespace TnyFramework.Coroutines.Async
{

    public interface IAsyncExecutor
    {
        /// <summary>
        /// 在协程上运行一个action
        /// </summary>
        /// <param name="action">action</param>
        void ExecAction(Action action);

        /// <summary>
        /// 在协程上运行一个function
        /// </summary>
        /// <param name="function">任务</param>
        /// <typeparam name="T">类型</typeparam>
        void ExecFunc<T>(Func<T> function);

        /// <summary>
        /// 在协程上运行一个action
        /// </summary>
        /// <param name="handle">action</param>
        void Exec(AsyncHandle handle);

        /// <summary>
        /// 在协程上运行一个function
        /// </summary>
        /// <param name="function">任务</param>
        /// <typeparam name="T">类型</typeparam>
        void Exec<T>(AsyncHandle<T> function);

        /// <summary>
        /// 在协程上运行一个action
        /// </summary>
        /// <param name="action">action</param>
        /// <returns>等待的任务</returns>
        Task AsyncAction(Action action);

        /// <summary>
        /// 在协程上运行一个返回 task<T> 的任务
        /// </summary>
        /// <param name="function">任务</param>
        /// <typeparam name="T">类型</typeparam>
        /// <returns>等待的任务</returns>
        Task<T> AsyncFunc<T>(Func<T> function);

        /// <summary>
        /// 在协程上运行一个返回 task 的任务
        /// </summary>
        /// <param name="handle">运行任务</param>
        /// <returns>task</returns>
        Task AsyncExec(AsyncHandle handle);

        /// <summary>
        /// 在协程上运行一个返回 task<T> 的任务
        /// </summary>
        /// <param name="function">任务</param>
        /// <typeparam name="T">类型</typeparam>
        /// <returns>等待的任务</returns>
        Task<T> AsyncExec<T>(AsyncHandle<T> function);
    }

}
