using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using TnyFramework.Common.Event;
using TnyFramework.Common.Logger;
using TnyFramework.Net.Base;
using TnyFramework.Net.Plugin;
using TnyFramework.Net.Rpc;

namespace TnyFramework.Net.Dispatcher
{

    public class MessageDispatcherContext
    {
        private static readonly IEventBus<CommandExecute> COMMAND_EXECUTE_EVENT_BUS = EventBuses.Create<CommandExecute>();
        private static readonly IEventBus<CommandDone> COMMAND_DONE_EVENT_BUS = EventBuses.Create<CommandDone>();

        /// <summary>
        /// 激活事件总线, 可监听到所有 Command 的事件
        /// </summary>
        public static IEventBox<CommandExecute> CommandExecuteEventBox => COMMAND_EXECUTE_EVENT_BUS;

        /// <summary>
        /// 断线事件总线, 可监听到所有 Command 的事件
        /// </summary>
        public static IEventBox<CommandDone> CommandDoneEventBox => COMMAND_DONE_EVENT_BUS;

        private static readonly ILogger LOGGER = LogFactory.Logger<MessageDispatcherContext>();

        private readonly INetAppContext appContext;

        private readonly IDictionary<object, IAuthenticateValidator> authenticateValidators = new Dictionary<object, IAuthenticateValidator>();

        private readonly IEventBus<CommandExecute> commandExecuteEvent;

        private readonly IEventBus<CommandDone> commandDoneEvent;

        public IEventBox<CommandExecute> CommandExecuteEvent => commandExecuteEvent;

        public IEventBox<CommandDone> CommandDoneEvent => commandDoneEvent;

        public MessageDispatcherContext(
            INetAppContext appContext,
            IList<ICommandPlugin> commandPlugins,
            IList<IAuthenticateValidator> authenticateValidators)
        {
            CommandPlugins = commandPlugins;
            this.appContext = appContext;
            foreach (var authenticateValidator in authenticateValidators)
            {
                this.authenticateValidators.Add(authenticateValidator.GetType(), authenticateValidator);
                var limit = authenticateValidator.AuthProtocolLimit;
                if (limit == null)
                    continue;
                foreach (var protocol in limit)
                {
                    this.authenticateValidators.Add(protocol, authenticateValidator);
                }
            }
            commandExecuteEvent = COMMAND_EXECUTE_EVENT_BUS.ForkChild();
            commandDoneEvent = COMMAND_DONE_EVENT_BUS.ForkChild();

        }

        public IList<ICommandPlugin> CommandPlugins { get; }

        public string AppType => appContext.AppType;

        public string ScopeType => appContext.ScopeType;

        public IAuthenticateValidator Validator(Type type)
        {
            if (type == null)
                return null;
            return authenticateValidators.TryGetValue(type, out var validator) ? validator : null;
        }

        public IAuthenticateValidator Validator(int protocolId)
        {
            return authenticateValidators.TryGetValue(protocolId, out var validator) ? validator : null;
        }

        internal void FireExecute(MessageCommand messageCommand)
        {
            commandExecuteEvent.Notify(messageCommand);
        }

        internal void FireDone(MessageCommand messageCommand, Exception cause)
        {
            commandDoneEvent.Notify(messageCommand, cause);
        }
    }

}
