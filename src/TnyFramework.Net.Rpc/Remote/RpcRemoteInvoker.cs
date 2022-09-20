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
using TnyFramework.Net.Common;
using TnyFramework.Net.Message;
using TnyFramework.Net.Rpc.Exceptions;
using TnyFramework.Net.Transport;

namespace TnyFramework.Net.Rpc.Remote
{

    public interface IRpcRemoteInvoker
    {
        object Invoke(object[] arguments);
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
        private readonly RpcRemoteServiceSet servicer;

        /// <summary>
        /// 路由
        /// </summary>
        private readonly IRpcRouter router;

        /// <summary>
        /// 异步调用 CompleteSource 生成器
        /// </summary>
        private readonly Func<IRpcCompleteSource> completeSourceFactory;

        /// <summary>
        /// 异步调用 CompleteSource 生成器
        /// </summary>
        private readonly Func<IMessage, object> returnValueFormatter;

        public RpcRemoteInvoker(RpcRemoteMethod method, RpcRemoteInstance instance, IRpcRouter router)
        {
            this.method = method;
            this.instance = instance;
            servicer = instance.ServiceSet;
            this.router = router;
            if (!method.IsAsync())
                return;
            returnValueFormatter = CreateMessageToReturnValue(); // 创建 Message转返回值转化器
            var sourceFactory = RpcInvokerFastInvokers.SourceFactory(method.BodyType); // 创建RpcCompleteSource构造调用器
            completeSourceFactory = () => (IRpcCompleteSource) sourceFactory.Invoke(null, returnValueFormatter);
        }

        public object Invoke(params object[] parameters)
        {
            try
            {
                var invokeParams = method.GetParams(parameters);
                var accessPoint = router.Route(servicer, method, invokeParams.RouteValue, invokeParams);
                if (accessPoint == null)
                {
                    throw new RpcInvokeException(NetResultCode.REMOTE_EXCEPTION, $"调用 {method} 异常, 未找到有效的远程服务节点");
                }
                var timeout = Timeout;
                return method.Mode switch {
                    MessageMode.Push => Push(accessPoint, timeout, invokeParams),
                    MessageMode.Request => Request(accessPoint, timeout, invokeParams),
                    _ => throw new RpcInvokeException(NetResultCode.REMOTE_EXCEPTION, $"调用 {method} 异常, 非法 rpc 模式")
                };
            } catch (Exception e)
            {
                return HandleException(e);
            }
        }

        private int Timeout => method.GetTimeout(instance.Setting.InvokeTimeout);

        private IProtocol Protocol()
        {
            return Protocols.Protocol(method.Protocol, method.Line);
        }

        private Func<IMessage, object> CreateMessageToReturnValue()
        {
            if (Equals(method.BodyMode, RpcBodyMode.RESULT))
            {
                var resultCreator = RpcInvokerFastInvokers.RcpResultCreator(method.BodyType);
                return message => resultCreator.Invoke(null, ResultCode.ForId(message.Code), message);
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
                return message => message.Body;
            }
            if (Equals(method.BodyMode, RpcBodyMode.RESULT_CODE_ID))
            {
                return message => message.Code;
            }
            if (Equals(method.BodyMode, RpcBodyMode.RESULT_CODE))
            {
                return message => ResultCode.ForId(message.Code);
            }
            return null;
        }

        private Task ToReturnTask(Task<IMessage> messageTask)
        {
            var source = completeSourceFactory();
            messageTask.ContinueWith(task => {
                if (task.IsFaulted)
                {
                    source.SetException(task.Exception);
                    LOGGER.LogError(task.Exception, $"Invoke Remote method {method.Name} exception");
                } else
                {
                    try
                    {
                        source.SetResult(task.Result);
                    } catch (Exception e)
                    {
                        source.SetException(e);
                        LOGGER.LogError(e, $"Invoke Remote method {method.Name} exception");
                    }
                }
            });
            return source.Task;
        }

        private object Request(ISender accessPoint, long timeout, RpcRemoteInvokeParams invokeParams)
        {
            var requestContext = MessageContexts.Request(Protocol(), invokeParams.Params);
            requestContext.WillRespondAwaiter(timeout)
                .WithHeaders(invokeParams.GetAllHeaders());
            var receipt = accessPoint.Send(requestContext);
            if (method.IsAsync())
            {
                return ToReturnTask(receipt.Respond());
            }
            var message = receipt.Respond().Result;
            return returnValueFormatter(message);
        }

        private object Push(ISender accessPoint, int timeout, RpcRemoteInvokeParams invokeParams)
        {
            var code = invokeParams.Code ?? ResultCode.SUCCESS;
            var messageContext = MessageContexts.Push(Protocol(), code)
                .WithBody(invokeParams.GetBody())
                .WithHeaders(invokeParams.GetAllHeaders());
            var receipt = accessPoint.Send(messageContext);
            if (method.IsAsync())
            {
                return receipt.Written();
            }
            receipt.Written().Wait(timeout);
            return null;
        }

        private object HandleException(Exception e)
        {
            if (method.Silently)
            {
                LOGGER.LogWarning(e, "{Method} invoke exception", method);
                // TODO 根据返回值优雅处理!!
                return null;
            } else
            {
                var code = ResultCodeExceptionAide.CodeOf(e, NetResultCode.REMOTE_EXCEPTION);
                throw new RpcInvokeException(code, e, $"调用 {this.method} 异常");
            }
        }
    }

}
