// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TnyFramework.Common.Logger;
using TnyFramework.Common.Result;
using TnyFramework.Net.Command.Dispatcher;
using TnyFramework.Net.Command.Dispatcher.Monitor;
using TnyFramework.Net.Common;
using TnyFramework.Net.Message;
using TnyFramework.Net.Rpc.Exceptions;
using TnyFramework.Net.Session;

namespace TnyFramework.Net.Rpc.Remote
{

    public interface IRpcRemoteInvoker
    {
        object? Invoke(object[] arguments);
    }

    public class RpcRemoteInvoker : IRpcRemoteInvoker
    {
        private static readonly ILogger LOGGER = LogFactory.Logger<RpcRemoteInvoker>();

        /// <summary>
        /// 协议描述
        /// </summary>
        private readonly RpcRemoteMethod method;

        /// <summary>
        /// rpc实例
        /// </summary>
        private readonly RpcRemoteInstance instance;

        /// <summary>
        /// 远程服务
        /// </summary>
        private readonly IRpcInvokeNodeSet serviceSet;

        /// <summary>
        /// 路由
        /// </summary>
        private readonly IRpcRouter router;

        /// <summary>
        /// 异步调用 CompleteSource 生成器
        /// </summary>
        private readonly Func<IRpcCompleteSource>? completeSourceFactory;

        /// <summary>
        /// 异步调用 CompleteSource 生成器
        /// </summary>
        private readonly Func<IMessage, object?>? returnValueFormatter;

        /// <summary>
        /// 监视器
        /// </summary>
        private readonly RpcMonitor rpcMonitor;

        public RpcRemoteInvoker(RpcRemoteMethod method, RpcRemoteInstance instance, IRpcRouter router, RpcMonitor rpcMonitor)
        {
            this.method = method;
            this.instance = instance;
            serviceSet = instance.ServiceSet;
            this.router = router;
            this.rpcMonitor = rpcMonitor;
            if (!method.IsAsync())
                return;
            returnValueFormatter = CreateMessageToReturnValue(); // 创建 Message转返回值转化器
            var sourceFactory = RpcInvokerFastInvokers.SourceFactory(method.BodyType); // 创建RpcCompleteSource构造调用器
            completeSourceFactory = () => (IRpcCompleteSource) sourceFactory.Invoke(null!, returnValueFormatter);
        }

        public object? Invoke(params object[] parameters)
        {
            try
            {
                var invokeParams = method.GetParams(parameters);
                var accessPoint = router.Route(serviceSet, method, invokeParams);
                if (accessPoint == null)
                {
                    throw new RpcInvokeException(NetResultCode.REMOTE_EXCEPTION, $"调用 {method} 异常, 未找到有效的远程服务节点");
                }
                var session = accessPoint.Session;
                var timeout = Timeout;
                return method.Mode switch {
                    MessageMode.Push => Push(session, timeout, invokeParams),
                    MessageMode.Request => Request(session, timeout, invokeParams),
                    _ => throw new RpcInvokeException(NetResultCode.REMOTE_EXCEPTION, $"调用 {method} 异常, 非法 rpc 模式")
                };
            } catch (Exception e)
            {
                return HandleRequestException(e);
            }
        }

        private int Timeout => method.GetTimeout(instance.Setting.InvokeTimeout);

        private IProtocol Protocol()
        {
            return Protocols.Protocol(method.Protocol, method.Line);
        }

        private Func<IMessage, object?> CreateMessageToReturnValue()
        {
            if (Equals(method.BodyMode, RpcBodyMode.RESULT))
            {
                var resultCreator = RpcInvokerFastInvokers.RcpResultCreator(method.BodyType);
                return message => resultCreator.Invoke(null!, ResultCode.ForId(message.Code), message);
            }
            if (Equals(method.BodyMode, RpcBodyMode.MESSAGE) || Equals(method.BodyMode, RpcBodyMode.MESSAGE_HEAD))
            {
                return message => message;
            }
            if (Equals(method.BodyMode, RpcBodyMode.VOID))
            {
                return _ => null;
            }
            if (Equals(method.BodyMode, RpcBodyMode.BODY))
            {
                return message => message.Body!;
            }
            if (Equals(method.BodyMode, RpcBodyMode.RESULT_CODE_ID))
            {
                return message => message.Code;
            }
            if (Equals(method.BodyMode, RpcBodyMode.RESULT_CODE))
            {
                return message => ResultCode.ForId(message.Code);
            }
            // return null;
            throw new NullReferenceException();
        }

        private Task ToReturnTask(Task<IMessage> messageTask)
        {
            var source = completeSourceFactory?.Invoke();
            messageTask.ContinueWith(task => {
                if (task.IsFaulted)
                {
                    source?.SetException(task.Exception!);
                    LOGGER.LogError(task.Exception, $"Invoke Remote method {method.Name} exception");
                } else
                {
                    try
                    {
                        source?.SetResult(task.Result);
                    } catch (Exception e)
                    {
                        source?.SetException(e);
                        LOGGER.LogError(e, $"Invoke Remote method {method.Name} exception");
                    }
                }
            });
            return source?.Task ?? Task.CompletedTask;
        }

        private object? Request(ISession session, long timeout, RpcRemoteInvokeParams invokeParams)
        {
            var content = MessageContents.Request(Protocol(), invokeParams.Params);
            content.WillRespondAwaiter(timeout)
                .WithHeaders(invokeParams.GetAllHeaders());
            var invokeContext = RpcMessageTransactionContext.CreateExit(session, content, rpcMonitor, method.IsAsync());
            invokeContext.Invoke(RpcMessageTransactionContext.RpcOperation(method.Name, content));
            try
            {
                content.Respond(out var respondTask);
                respondTask.ContinueWith(task => {
                    if (task.IsFaulted)
                    {
                        invokeContext.Complete(task.Exception!);
                    } else
                    {
                        invokeContext.Complete(task.Result);
                    }
                });
                _ = session.Send(content, true);
                if (method.IsAsync())
                {
                    return ToReturnTask(respondTask);
                }
                var message = respondTask.Result;
                invokeContext.Complete(message);
                return returnValueFormatter?.Invoke(message);
            } catch (Exception e)
            {
                invokeContext.Complete(e);
                return HandleRequestException(e);
            }
        }

        // private void HandleException(Exception e) {
        //     if (method.isSilently()) {
        //         LOGGER.warn("{} invoke exception", this.method, e);
        //     } else {
        //         ResultCode code = ResultCodeExceptionAide.codeOf(e, NetResultCode.REMOTE_EXCEPTION);
        //         throw new RpcInvokeException(code, e, "调用 {} 异常", this.method);
        //     }
        // }

        private object? Push(ISession session, int timeout, RpcRemoteInvokeParams invokeParams)
        {
            var code = invokeParams.Code;
            var content = MessageContents.Push(Protocol(), code)
                .WithBody(invokeParams.GetBody())
                .WithHeaders(invokeParams.GetAllHeaders());
            var invokeContext = RpcMessageTransactionContext.CreateExit(session, content, rpcMonitor, false);
            try
            {
                invokeContext.Invoke(RpcMessageTransactionContext.RpcOperation(method.Name, content));
                var sent = session.Send(content, true);
                invokeContext.Complete();
                if (method.IsAsync())
                {
                    return sent.AsTask();
                }
                sent.AsTask().Wait(timeout);
                return null;
            } catch (Exception e)
            {
                invokeContext.Complete(e);
                return null;
            }
        }

        private object? HandleRequestException(Exception e)
        {
            if (method.Silently)
            {
                LOGGER.LogWarning(e, "{Method} invoke exception", method);
                // TODO 根据返回值优雅处理!!
                if (!method.IsAsync())
                    return null;
                var source = completeSourceFactory?.Invoke();
                source?.SetResult(null);
                return source?.Task ?? Task.CompletedTask;
            }
            var code = ResultCodeExceptionAide.CodeOf(e, NetResultCode.REMOTE_EXCEPTION);
            throw new RpcInvokeException(code, e, $"调用 {this.method} 异常");
        }
    }

}
