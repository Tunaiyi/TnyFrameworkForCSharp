using System.Threading;
using TnyFramework.Coroutines.Async;
using TnyFramework.Net.Endpoint;
using TnyFramework.Net.Message;
using TnyFramework.Net.Transport;

namespace TnyFramework.Net.Dispatcher
{

    public class RpcHandleContext
    {
        private static readonly AsyncLocal<RpcHandleContext> LOCAL_CONTEXT = new AsyncLocal<RpcHandleContext>();

        private MessageCommand command;

        public static RpcHandleContext Current {
            get {
                var info = LOCAL_CONTEXT.Value;
                if (info != null)
                    return info;
                info = new RpcHandleContext();
                LOCAL_CONTEXT.Value = info;
                return info;
            }
        }

        /// <summary>
        /// 获取消息对象
        /// </summary>
        public IMessage Message => command.Message;

        /// <summary>
        /// 终端对象
        /// </summary>
        public IEndpoint Endpoint => command.Endpoint;

        /// <summary>
        /// 获取通道
        /// </summary>
        public ITunnel Tunnel => command.Tunnel;

        /// <summary>
        /// 当前用户执行器
        /// </summary>
        public IAsyncExecutor Executor => command.Endpoint;

        internal static void SetCurrent(MessageCommand command)
        {
            Current.command = command;
        }

        internal static void Clean()
        {
            var info = LOCAL_CONTEXT.Value;
            if (info != null)
            {
                info.command = null;
            }
        }
    }

}
