using TnyFramework.Net.Command.Processor;
using TnyFramework.Net.Dispatcher;
namespace TnyFramework.Net.Endpoint
{
    /// <summary>
    /// 终端上下文
    /// </summary>
    public interface IEndpointContext
    {

        /// <summary>
        /// 消息分发器
        /// </summary>
        IMessageDispatcher MessageDispatcher { get; }

        /// <summary>
        /// 任务执行器
        /// </summary>
        ICommandTaskBoxProcessor CommandTaskProcessor { get; }
    }
}
