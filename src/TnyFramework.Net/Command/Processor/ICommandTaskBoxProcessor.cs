using System.Threading.Tasks;
using TnyFramework.Coroutines.Async;
using TnyFramework.Net.Command.Tasks;

namespace TnyFramework.Net.Command.Processor
{

    public interface ICommandTaskBoxProcessor 
    {
        /// <summary>
        /// 提交
        /// </summary>
        /// <param name="box"></param>
        void Submit(CommandTaskBox box);
        
        /// <summary>
        /// 在协程上运行一个返回 task 的任务
        /// </summary>
        /// <param name="handle">运行任务</param>
        /// <returns>task</returns>
        Task AsyncExec(CommandTaskBox box, AsyncHandle handle);

        /// <summary>
        /// 在协程上运行一个返回 task<T> 的任务
        /// </summary>
        /// <param name="function">任务</param>
        /// <typeparam name="T">类型</typeparam>
        /// <returns>等待的任务</returns>
        Task<T> AsyncExec<T>(CommandTaskBox box, AsyncHandle<T> function);
    }

}
