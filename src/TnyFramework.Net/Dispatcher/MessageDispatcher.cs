using System.Collections.Concurrent;
using System.Collections.Generic;
using TnyFramework.Common.Exception;
using TnyFramework.Net.Message;
using TnyFramework.Net.Transport;
using Microsoft.Extensions.Logging;
using TnyFramework.Common.Event;
using TnyFramework.Common.Logger;
using TnyFramework.Net.Common;
using TnyFramework.Net.Endpoint;
namespace TnyFramework.Net.Dispatcher
{
    public class MessageDispatcher : IMessageDispatcher
    {
        private static readonly ILogger LOGGER = LogFactory.Logger<MessageDispatcher>();

        /// <summary>
        /// controller map
        /// </summary>
        private volatile IDictionary<int, IDictionary<MessageMode, MethodControllerHolder>> methodHolder =
            new ConcurrentDictionary<int, IDictionary<MessageMode, MethodControllerHolder>>();

        /// <summary>
        /// 分发器上下文
        /// </summary>
        private readonly MessageDispatcherContext context;

        /// <summary>
        /// 终端管理器
        /// </summary>
        private readonly IEndpointKeeperManager endpointKeeperManager;


        public IEventBox<CommandExecute> CommandExecuteEvent => context.CommandExecuteEvent;

        public IEventBox<CommandDone> CommandDoneEvent => context.CommandDoneEvent;


        public MessageDispatcher(MessageDispatcherContext context, IEndpointKeeperManager endpointKeeperManager)
        {
            this.context = context;
            this.endpointKeeperManager = endpointKeeperManager;
        }


        public ICommand Dispatch(INetTunnel tunnel, IMessage message)
        {
            // 获取方法持有器
            var controller = SelectController(message.ProtocolId, message.Mode);
            if (controller != null)
            {
                return new ControllerMessageCommand(controller, tunnel, message, this.context, this.endpointKeeperManager);
            }
            if (message.Mode != MessageMode.Request)
                return null;
            LOGGER.LogWarning("{Mode} controller [{Name}] not exist", message.Mode, message.ProtocolId);
            return RunnableCommand.Action(() => tunnel.Send(MessageContexts.Respond(NetResultCode.SERVER_NO_SUCH_PROTOCOL, message)));
        }


        /// <summary>
        /// 是否可以分发
        /// </summary>
        /// <param name="head">检测的消息头</param>
        /// <returns></returns>
        public bool IsCanDispatch(IMessageHead head)
        {
            return SelectController(head.ProtocolId, head.Mode) != null;
        }


        private MethodControllerHolder SelectController(int protocol, MessageMode mode)
        {
            // 获取方法持有器
            if (!methodHolder.TryGetValue(protocol, out var controllerMap))
                return null;
            return controllerMap.TryGetValue(mode, out var controller) ? controller : null;
        }


        /// <summary>
        /// 添加控制器对象列表 
        /// </summary>
        /// <param name="controllerExecutors">控制器对象列表</param>
        internal void AddControllers(IEnumerable<object> controllerExecutors)
        {
            foreach (var executor in controllerExecutors)
            {
                AddController(executor);
            }
        }



        /// <summary>
        /// 添加控制器对象
        /// </summary>
        /// <param name="controllerExecutor">控制器对象</param>
        public void AddController(object controllerExecutor)
        {
            lock (this)
            {
                var typeHolder = new TypeControllerHolder(controllerExecutor, context);
                foreach (var controller in typeHolder.MethodControllers)
                {
                    if (!methodHolder.TryGetValue(controller.Protocol, out var holderMap))
                    {
                        holderMap = new Dictionary<MessageMode, MethodControllerHolder>();
                        methodHolder.Add(controller.Protocol, holderMap);
                    }
                    foreach (var mode in controller.MessageModes)
                    {

                        if (holderMap.TryGetValue(mode, out var old))
                        {
                            throw new IllegalArgumentException($"{old} 与 {controller} 对MessageMode {mode} 处理发生冲突");
                        }
                        holderMap.Add(mode, controller);
                    }
                }
            }
        }
    }
}
