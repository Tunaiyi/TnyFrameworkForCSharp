// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TnyFramework.Common.Exceptions;
using TnyFramework.Common.Extensions;
using TnyFramework.Common.Logger;
using TnyFramework.Common.Reflection.FastInvoke;
using TnyFramework.Common.Reflection.FastInvoke.FuncInvoke;
using TnyFramework.Common.Result;
using TnyFramework.Net.Command.Auth;
using TnyFramework.Net.Common;
using TnyFramework.Net.Exceptions;
using TnyFramework.Net.Message;
using TnyFramework.Net.Rpc;
using TnyFramework.Net.Transport;

namespace TnyFramework.Net.Command.Dispatcher
{

    public class RpcInvokeCommand : RpcHandleCommand
    {
        private static readonly ILogger LOGGER = LogFactory.Logger<RpcInvokeCommand>();

        /// <summary>
        /// Task 结果获取调用器
        /// </summary>
        private static readonly ConcurrentDictionary<Type, IFastInvoker> TASK_RESULT_INVOKER = new();

        /// <summary>
        /// Task 结果的名字
        /// </summary>
        private const string TASK_RESULT_PROPERTY_NAME = nameof(Task<object>.Result);

        private readonly RpcInvokeContext invokeContext;

        private readonly MessageDispatcherContext dispatcherContext;

        private readonly IContactAuthenticator contactAuthenticator;

        public RpcInvokeCommand(MessageDispatcherContext dispatcherContext, RpcInvokeContext handleContext,
            IContactAuthenticator contactAuthenticator)
            : base(handleContext.RpcMessageContext)
        {
            invokeContext = handleContext;
            this.dispatcherContext = dispatcherContext;
            this.contactAuthenticator = contactAuthenticator;
        }

        protected override Task OnRun()
        {
            rpcMessageContext.Invoke(RpcMessageTransactionContext.RpcOperation(invokeContext.Name, rpcMessageContext.NetMessage));
            // 调用逻辑业务
            return Invoke();
        }

        protected override void OnException(Exception? cause)
        {
            invokeContext.Complete(cause);
        }

        protected override void OnDone(Exception? cause)
        {
            HandleResult();
        }

        public override string Name => invokeContext.Name;

        private async Task Invoke()
        {
            var message = rpcMessageContext.NetMessage;
            var tunnel = rpcMessageContext.NetTunnel;
            var controller = invokeContext.Controller;
            if (controller.IsNull())
            {
                var head = message.Head;
                LOGGER.LogWarning("Controller {Mode}[{Protocol}] 没有存在对应Controller ", head.Mode, head.ProtocolId);
                invokeContext.Intercept(NetResultCode.SERVER_NO_SUCH_PROTOCOL);
                return;
            }

            // 检测登录认证
            if (controller.IsHasAuthValidator && !tunnel.IsAuthenticated())
            {
                contactAuthenticator.Authenticate(dispatcherContext, rpcMessageContext, controller.AuthValidatorType!);
            }

            var appType = invokeContext.AppType;
            // 检测当前 app 类型是否可以调用
            if (!controller.IsActiveByAppType(appType))
            {
                LOGGER.LogWarning("Controller [{Name}] App类型 {AppType} 无法此协议", Name, appType);
                invokeContext.Intercept(NetResultCode.SERVER_NO_SUCH_PROTOCOL);
                return;
            }

            var scopeType = invokeContext.ScopeType;
            // 检测当前 环境作用域 是否可以调用
            if (!controller.IsActiveByScope(scopeType))
            {
                LOGGER.LogWarning("Controller [{Name}] Scope类型 {ScopeType} 无法此协议", Name, scopeType);
                invokeContext.Intercept(NetResultCode.SERVER_NO_SUCH_PROTOCOL);
                await Task.CompletedTask;
            }

            // 判断是否需要登录检测
            LOGGER.LogDebug("Controller [{Name}] 检测已登陆认证", Name);
            if (controller.IsAuth() && !tunnel.IsAuthenticated())
            {
                LOGGER.LogError("Controller [{Name}] 用户未登陆", Name);
                invokeContext.Intercept(NetResultCode.NO_LOGIN);
                return;
            }

            // 判断身份是否符合
            LOGGER.LogDebug("Controller [{Name}] 检测用户组调用权限", Name);
            if (!controller.IsContactGroup(invokeContext.ContactType))
            {
                LOGGER.LogError("Controller [{Name}] , 用户组 [{User}] 无法调用此协议", Name, tunnel.ContactGroup);
                invokeContext.Intercept(NetResultCode.NO_PERMISSIONS);
                return;
            }

            // 执行调用前插件
            LOGGER.LogDebug("Controller [{Name}] 执行BeforePlugins", Name);
            controller.BeforeInvoke(tunnel, message, invokeContext);
            if (invokeContext.Done)
            {
                return;
            }

            // 执行调用
            LOGGER.LogDebug("Controller [{Name}] 执行业务", Name);
            var result = controller.Invoke(tunnel, message);
            if (result is Task task) // 如果是 Task
            {
                await task; // 等待
                var taskType = task.GetType();
                if (taskType.GetGenericTypeDefinition() == typeof(Task<>)) // 有返回值的 task
                {
                    var resultValue = TaskResultInvoker(taskType).Invoke(task);
                    invokeContext.Complete(resultValue);
                } else // 无返回值的 task
                {
                    invokeContext.Complete();
                }
            } else // 非 Task
            {
                invokeContext.Complete(result);
            }

            // var returnType = controller.ReturnType;
            // if (returnType == typeof(void))
            // {
            //     invokeContext.Complete(null);
            // }
            LOGGER.LogDebug("Controller [{Name}] 处理Message完成!", Name);
        }

        private static IFastInvoker TaskResultInvoker(Type type)
        {
            if (TASK_RESULT_INVOKER.TryGetValue(type, out var invoker))
            {
                return invoker;
            }
            var property = type.GetProperty(TASK_RESULT_PROPERTY_NAME);
            var fastInvoker = FastFuncFactory.Invoker(property!);
            return TASK_RESULT_INVOKER.TryAdd(type, fastInvoker) ? fastInvoker : TASK_RESULT_INVOKER[type];
        }

        private void AfterInvoke(ITunnel tunnel, IMessage message, Exception? cause)
        {
            var controller = invokeContext.Controller;
            if (controller.IsNotNull())
            {
                LOGGER.LogDebug("Controller [{}] 执行AfterPlugins", Name);
                controller.AfterInvoke(tunnel, message, this.invokeContext);
                LOGGER.LogDebug("Controller [{}] 处理Message完成!", Name);
            }
            this.FireDone(cause);
        }

        // /// <summary>
        // /// 身份认证
        // /// </summary>
        // /// <param name="controller"></param>
        // private void CheckAuthenticate(ControllerHolder controller)
        // {
        //     
        //     if (t.IsAuthenticated())
        //         return;
        //     var validator = FindValidator(controller);
        //     if (validator == null)
        //     {
        //         return;
        //     }
        //     var certificateFactory = tunnel.CertificateFactory;
        //     Authenticate(validator, certificateFactory);
        // }

        // private IAuthenticateValidator FindValidator(ControllerHolder controller)
        // {
        //     var validator = DispatcherContext.Validator(controller.AuthValidatorType);
        //     return validator ?? DispatcherContext.Validator(Message.ProtocolId);
        // }
        //
        // private void Authenticate(IAuthenticateValidator validator, ICertificateFactory certificateFactory)
        // {
        //     LOGGER.LogDebug("Controller [{Name}] 开始进行登陆认证", Name);
        //     var certificate = validator.Validate(Tunnel, Message, certificateFactory);
        //     // 是否需要做登录校验,判断是否已经登录
        //     if (certificate != null && certificate.IsAuthenticated())
        //     {
        //         SessionKeeperManager.Online(certificate, tunnel);
        //     }
        // }

        private void HandleResult()
        {
            // var controller = invokeContext.Controller;
            var message = rpcMessageContext.NetMessage;
            var tunnel = rpcMessageContext.NetTunnel;
            IResultCode code;
            object? body = null;
            var cause = invokeContext.Cause;
            var result = cause != null ? ResultOfException(cause) : invokeContext.Result;
            if (cause == null)
            {
                switch (result)
                {
                    case IRpcResult rpcResult:
                        code = rpcResult.ResultCode;
                        body = rpcResult.Body;
                        break;
                    case IResultCode resultCode:
                        code = resultCode;
                        break;
                    default:
                        code = ResultCode.SUCCESS;
                        body = result;
                        break;
                }
            } else
            {
                var commandResult = ResultOfException(cause);
                code = commandResult.ResultCode;
                body = commandResult.Body;
            }
            // 执行调用后插件
            AfterInvoke(tunnel, message, cause);

            MessageContent? content = null;
            if (message.Mode == MessageMode.Request || (message.Mode == MessageMode.Push && body != null))
            {
                content = RpcMessageAide.ToMessage(invokeContext.RpcMessageContext, code, body);
            }
            if (content != null)
            {
                rpcMessageContext.Complete(content, cause);
            } else
            {
                rpcMessageContext.CompleteSilently();
            }
        }

        private IRpcResult ResultOfException(Exception e)
        {

            LOGGER.LogError(e, "Controller [{Name}] exception", Name);
            switch (e)
            {
                case CommandException ce:
                    return RpcResults.Result(ce.Code, ce.Body);
                case ResultCodeException rce:
                    return RpcResults.Result(rce.Code);
                default:
                    return RpcResults.Result(NetResultCode.SERVER_EXECUTE_EXCEPTION);
            }
        }

        private void FireDone(Exception? cause)
        {
            dispatcherContext.FireDone(this, cause);
        }

        // private void FireExecute()
        // {
        //     dispatcherContext.FireExecute(this);
        // }
    }

}
