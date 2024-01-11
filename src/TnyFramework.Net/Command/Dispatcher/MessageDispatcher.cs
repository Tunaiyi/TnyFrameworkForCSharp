// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System.Collections.Concurrent;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using TnyFramework.Common.Event;
using TnyFramework.Common.Exceptions;
using TnyFramework.Common.Logger;
using TnyFramework.Net.Command.Auth;
using TnyFramework.Net.Common;
using TnyFramework.Net.Message;

namespace TnyFramework.Net.Command.Dispatcher;

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

    private readonly IContactAuthenticator contactAuthenticator;

    // public IEventWatch<CommandExecute> CommandExecuteEvent => context.CommandExecuteEvent;

    public IEventWatch<CommandDone> CommandDoneEvent => context.CommandDoneEvent;

    public MessageDispatcher(MessageDispatcherContext context, IContactAuthenticator contactAuthenticator)
    {
        this.context = context;
        this.contactAuthenticator = contactAuthenticator;
    }

    public ICommand Dispatch(IRpcMessageEnterContext rpcMessageContext)
    {
        var message = rpcMessageContext.NetMessage;
        // 获取方法持有器
        var controller = SelectController(message.ProtocolId, message.Mode);
        if (controller != null)
        {
            var handleContext = new RpcInvokeContext(controller, rpcMessageContext, context.AppContext);
            return new RpcInvokeCommand(context, handleContext, contactAuthenticator);
        }
        if (message.Mode == MessageMode.Request)
        {
            LOGGER.LogWarning("{Mode} controller [{Name}] not exist", message.Mode, message.ProtocolId);
            return new RpcRespondCommand(rpcMessageContext, NetResultCode.SERVER_NO_SUCH_PROTOCOL, null);
        }
        return new RpcNoopCommand(rpcMessageContext);
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

    private MethodControllerHolder? SelectController(int protocol, MessageMode mode)
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
                if (holderMap.TryGetValue(controller.MessageMode, out var old))
                {
                    throw new IllegalArgumentException(
                        $"{old} 与 {controller} 对MessageMode {controller.MessageMode} 处理发生冲突");
                }
                holderMap.Add(controller.MessageMode, controller);

            }
        }
    }
}
