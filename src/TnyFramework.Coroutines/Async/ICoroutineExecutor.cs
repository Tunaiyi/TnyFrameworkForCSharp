using System;
namespace TnyFramework.Coroutines.Async
{
    /// <summary>
    /// 协程执行器
    /// </summary>
    public interface ICoroutineExecutor
    {
        /// <summary>
        /// 提交
        /// </summary>
        /// <param name="action">任务</param>
        void Summit(Action action);
    }
}
