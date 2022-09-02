using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TnyFramework.Common.FastInvoke;
using TnyFramework.Common.FastInvoke.FuncInvoke;
using TnyFramework.Common.Logger;
using TnyFramework.Net.Base;
using TnyFramework.Net.Command;
using TnyFramework.Net.Common;
using TnyFramework.Net.Endpoint;
using TnyFramework.Net.Message;
using TnyFramework.Net.Rpc;
using TnyFramework.Net.Transport;

namespace TnyFramework.Net.Dispatcher
{

    public class ControllerMessageCommand : MessageCommand
    {
        private static readonly ILogger LOGGER = LogFactory.Logger<ControllerMessageCommand>();

        /// <summary>
        /// Task 结果获取调用器
        /// </summary>
        private static readonly ConcurrentDictionary<Type, IFastInvoker> TASK_RESULT_INVOKER = new ConcurrentDictionary<Type, IFastInvoker>();

        /// <summary>
        /// Task 结果的名字
        /// </summary>
        private const string TASK_RESULT_PROPERTY_NAME = nameof(Task<object>.Result);

        public ControllerMessageCommand(MethodControllerHolder methodHolder, INetTunnel tunnel, IMessage message,
            MessageDispatcherContext dispatcherContext, IEndpointKeeperManager endpointKeeperManager) :
            base(new MessageCommandContext(methodHolder), tunnel, message, dispatcherContext, endpointKeeperManager)
        {
        }

        protected override async Task Invoke()
        {
            var controller = CommandContext.Controller;
            if (controller == null)
            {
                var head = Message.Head;
                LOGGER.LogWarning("Controller {Mode}[{Protocol}] 没有存在对应Controller ", head.Mode, head.ProtocolId);
                CommandContext.Intercept(NetResultCode.SERVER_NO_SUCH_PROTOCOL);
                return;
            }

            // 检测登录认证
            CheckAuthenticate(controller);

            // 检测当前 app 类型是否可以调用
            if (!controller.IsActiveByAppType(AppType))
            {
                LOGGER.LogWarning("Controller [{Name}] App类型 {AppType} 无法此协议", Name, AppType);
                CommandContext.Intercept(NetResultCode.SERVER_NO_SUCH_PROTOCOL);
                return;
            }

            // 检测当前 环境作用域 是否可以调用
            if (!controller.IsActiveByScope(ScopeType))
            {
                LOGGER.LogWarning("Controller [{Name}] Scope类型 {ScopeType} 无法此协议", Name, ScopeType);
                CommandContext.Intercept(NetResultCode.SERVER_NO_SUCH_PROTOCOL);
                await Task.CompletedTask;
            }

            // 判断是否需要登录检测
            LOGGER.LogDebug("Controller [{Name}] 检测已登陆认证", Name);
            if (controller.IsAuth() && !Tunnel.IsAuthenticated())
            {
                LOGGER.LogError("Controller [{Name}] 用户未登陆", Name);
                CommandContext.Intercept(NetResultCode.NO_LOGIN);
                return;
            }

            // 判断身份是否符合
            LOGGER.LogDebug("Controller [{Name}] 检测用户组调用权限", Name);
            if (!controller.IsUserGroup(MessagerType))
            {
                LOGGER.LogError("Controller [{Name}] , 用户组 [{User}] 无法调用此协议", Name, Tunnel.UserGroup);
                CommandContext.Intercept(NetResultCode.NO_PERMISSIONS);
                return;
            }

            // 执行调用前插件
            LOGGER.LogDebug("Controller [{Name}] 执行BeforePlugins", Name);
            controller.BeforeInvoke(Tunnel, Message, CommandContext);
            if (CommandContext.Done)
            {
                return;
            }

            // 执行调用
            LOGGER.LogDebug("Controller [{Name}] 执行业务", Name);
            var result = controller.Invoke(tunnel, Message);
            if (result is Task task) // 如果是 Task
            {
                await task; // 等待
                var taskType = task.GetType();
                if (taskType.GetGenericTypeDefinition() == typeof(Task<>)) // 有返回值的 task
                {
                    var resultValue = TaskResultInvoker(taskType).Invoke(task);
                    CommandContext.Complete(resultValue);
                } else // 无返回值的 task
                {
                    CommandContext.Complete(null);
                }
            } else // 非 Task
            {
                CommandContext.Complete(result);
            }

            // 执行调用后插件
            LOGGER.LogDebug("Controller [{Name}] 执行AfterPlugins", Name);
            controller.AfterInvoke(Tunnel, Message, CommandContext);
            // var returnType = controller.ReturnType;
            // if (returnType == typeof(void))
            // {
            //     CommandContext.Complete(null);
            // }
            LOGGER.LogDebug("Controller [{Name}] 处理Message完成!", Name);
        }

        private IMessagerType MessagerType {
            get {
                if (Forward == null)
                    return Tunnel.MessagerType;
                var from = Forward.From;
                return from != null ? from.ServiceType : Tunnel.MessagerType;
            }
        }

        private static IFastInvoker TaskResultInvoker(Type type)
        {
            if (TASK_RESULT_INVOKER.TryGetValue(type, out var invoker))
            {
                return invoker;
            }
            var property = type.GetProperty(TASK_RESULT_PROPERTY_NAME);
            var fastInvoker = FastFuncFactory.Invoker(property);
            return TASK_RESULT_INVOKER.TryAdd(type, fastInvoker) ? fastInvoker : TASK_RESULT_INVOKER[type];
        }

        /// <summary>
        /// 身份认证
        /// </summary>
        /// <param name="controller"></param>
        private void CheckAuthenticate(ControllerHolder controller)
        {
            if (Tunnel.IsAuthenticated())
                return;
            var validator = FindValidator(controller);
            if (validator == null)
            {
                return;
            }
            var certificateFactory = tunnel.CertificateFactory;
            Authenticate(validator, certificateFactory);
        }

        private IAuthenticateValidator FindValidator(ControllerHolder controller)
        {
            var validator = DispatcherContext.Validator(controller.AuthValidatorType);
            return validator ?? DispatcherContext.Validator(Message.ProtocolId);
        }

        private void Authenticate(IAuthenticateValidator validator, ICertificateFactory certificateFactory)
        {
            LOGGER.LogDebug("Controller [{Name}] 开始进行登陆认证", Name);
            var certificate = validator.Validate(Tunnel, Message, certificateFactory);
            // 是否需要做登录校验,判断是否已经登录
            if (certificate != null && certificate.IsAuthenticated())
            {
                EndpointKeeperManager.Online(certificate, tunnel);
            }
        }
    }

}
