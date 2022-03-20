using System;
using System.Threading.Tasks;
namespace TnyFramework.Coroutines.Async
{
    /// <summary>
    /// 协程接口
    /// </summary>
    public interface ICoroutine
    {
        /// <summary>
        /// 协程状态
        /// </summary>
        CoroutineStatus Status { get; }

        /// <summary>
        /// 协程 id 自动分配
        /// </summary>
        int Id { get; }


        /// <summary>
        /// 协程名称
        /// </summary>
        string Name { get; }


        /// <summary>
        /// 是否启动
        /// </summary>
        /// <returns>启动返回 true</returns>
        bool IsStart();


        /// <summary>
        /// 是否停止(非 start 状态)
        /// </summary>
        /// <returns>停止返回 true</returns>
        bool IsStop();


        /// <summary>
        /// 是否已关闭
        /// </summary>
        /// <returns>关闭返回 true</returns>
        bool IsShutdown();


        /// <summary>
        /// 在协程上运行一个action
        /// </summary>
        /// <param name="action">action</param>
        Task Run(Action action);


        /// <summary>
        /// 在协程上运行一个返回 task<T> 的任务
        /// </summary>
        /// <param name="function">任务</param>
        /// <typeparam name="T">类型</typeparam>
        /// <returns>等待的任务</returns>
        Task<T> Run<T>(Func<T> function);


        /// <summary>
        /// 在协程上运行一个返回 task 的任务
        /// </summary>
        /// <param name="action">运行任务</param>
        /// <returns>task</returns>
        Task Exec(CoroutineAction action);



        /// <summary>
        /// 在协程上运行一个返回 task<T> 的任务
        /// </summary>
        /// <param name="function">任务</param>
        /// <typeparam name="T">类型</typeparam>
        /// <returns>等待的任务</returns>
        Task<T> Exec<T>(CoroutineFunc<T> function);


        /// <summary>
        /// 延迟
        /// </summary>
        /// <param name="millisecondsDelay">延迟时间</param>
        /// <returns>task</returns>
        Task Delay(int millisecondsDelay);


        /// <summary>
        /// 延迟
        /// </summary>
        /// <param name="delay">延迟时间</param>
        /// <returns>task</returns>
        Task Delay(TimeSpan delay);


        /// <summary>
        /// 重复运行
        /// </summary>
        /// <param name="times">次数</param>
        /// <param name="action">行为</param>
        /// <returns>task</returns>
        Task Repeat(int times, Action action);


        /// <summary>
        /// 重复运行
        /// </summary>
        /// <param name="times">次数</param>
        /// <param name="action">行为</param>
        /// <returns>task</returns>
        Task Repeat(int times, CoroutineAction action);


        /// <summary>
        /// 运行直到任务返回 true 则停止, 每次运行会进行 await func
        /// </summary>
        /// <param name="func">行为</param>
        /// <returns>task</returns>
        Task ExecUntil(CoroutineFunc<bool> func);


        /// <summary>
        /// 运行直到任务返回 true 则停止, 每次运行会进行 await func
        /// </summary>
        /// <param name="func">行为</param>
        /// <returns>task</returns>
        Task<T> ExecUntil<T>(CoroutineFunc<CoroutineState<T>> func);


        /// <summary>
        /// 运行直到任务返回 true 则停止, 每次运行会进行 yield
        /// </summary>
        /// <param name="func">行为</param>
        /// <returns>task</returns>
        Task RunUntil(Func<bool> func);


        /// <summary>
        /// 运行直到任务返回 true 则停止, 每次运行会进行 yield
        /// </summary>
        /// <param name="func">行为</param>
        /// <returns>task</returns>
        Task<T> RunUntil<T>(Func<CoroutineState<T>> func);


        /// <summary>
        /// 关闭协程
        /// </summary>
        /// <param name="millisecondsTimeout">等待超时</param>
        /// <returns>无超时返回 false, 超时返回 true</returns>
        Task<bool> Shutdown(long millisecondsTimeout);
    }
}
