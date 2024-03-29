// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.Collections.Generic;
using TnyFramework.Common.Event;
using TnyFramework.Common.EventBus;
using TnyFramework.Net.Application;
using TnyFramework.Net.Command.Auth;
using TnyFramework.Net.Plugin;
using CollectionExtensions = TnyFramework.Common.Extensions.CollectionExtensions;

namespace TnyFramework.Net.Command.Dispatcher;

public class MessageDispatcherContext
{
    private static readonly IEventBus<CommandExecute> COMMAND_EXECUTE_EVENT_BUS = EventBuses.Create<CommandExecute>();
    private static readonly IEventBus<CommandDone> COMMAND_DONE_EVENT_BUS = EventBuses.Create<CommandDone>();

    // private static readonly ILogger LOGGER = LogFactory.Logger<MessageDispatcherContext>();

    // /// <summary>
    // /// 激活事件总线, 可监听到所有 Command 的事件
    // /// </summary>
    // public static IEventWatch<CommandExecute> CommandExecuteEventBox => COMMAND_EXECUTE_EVENT_BUS;

    /// <summary>
    /// 断线事件总线, 可监听到所有 Command 的事件
    /// </summary>
    public static IEventWatch<CommandDone> CommandDoneEventBox => COMMAND_DONE_EVENT_BUS;

    private readonly IDictionary<object, IAuthenticationValidator> authenticationValidators = new Dictionary<object, IAuthenticationValidator>();

    private readonly IEventBus<CommandDone> commandDoneEvent;

    public IEventWatch<CommandDone> CommandDoneEvent => commandDoneEvent;

    public MessageDispatcherContext(
        INetAppContext appContext,
        IList<ICommandPlugin> commandPlugins,
        IList<IAuthenticationValidator> authenticateValidators)
    {
        CommandPlugins = commandPlugins;
        AppContext = appContext;
        foreach (var authenticateValidator in authenticateValidators)
        {
            this.authenticationValidators.Add(authenticateValidator.GetType(), authenticateValidator);
            var limit = authenticateValidator.AuthProtocolLimit;
            if (CollectionExtensions.IsNullOrEmpty(limit))
                continue;
            foreach (var protocol in limit)
            {
                authenticationValidators.Add(protocol, authenticateValidator);
            }
        }
        // commandExecuteEvent = COMMAND_EXECUTE_EVENT_BUS.ForkChild();
        commandDoneEvent = COMMAND_DONE_EVENT_BUS.ForkChild();

    }

    public IList<ICommandPlugin> CommandPlugins { get; }

    /// <summary>
    /// 获取应用配置
    /// </summary>
    public INetAppContext AppContext { get; }

    public IAuthenticationValidator? Validator(Type? type)
    {
        if (type == null)
            return null;
        return authenticationValidators.TryGetValue(type, out var validator) ? validator : null;
    }

    public IAuthenticationValidator? Validator(int protocolId)
    {
        return authenticationValidators.TryGetValue(protocolId, out var validator) ? validator : null;
    }

    internal void FireDone(RpcHandleCommand rpcHandleCommand, Exception? cause)
    {
        commandDoneEvent.Notify(rpcHandleCommand, cause);
    }
}
