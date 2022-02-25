using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TnyFramework.Common.Exception;
using TnyFramework.Common.Logger;
using TnyFramework.Common.Result;
using TnyFramework.Net.Command;
using TnyFramework.Net.Common;
using TnyFramework.Net.Endpoint;
using TnyFramework.Net.Exceptions;
using TnyFramework.Net.Message;
using TnyFramework.Net.Transport;
namespace TnyFramework.Net.Dispatcher
{
    public abstract class MessageCommand : ICommand
    {
        private static readonly ILogger LOGGER = LogFactory.Logger<MessageCommand>();

        public string Name => CommandContext.Name;

        public bool Done { get; private set; }

        protected IEndpointKeeperManager EndpointKeeperManager { get; }

        protected MessageDispatcherContext DispatcherContext { get; }

        protected MessageCommandContext CommandContext { get; }

        protected INetTunnel Tunnel { get; }

        protected IMessage Message { get; }

        protected string AppType => DispatcherContext.AppType;

        protected string ScopeType => DispatcherContext.ScopeType;


        protected MessageCommand(MessageCommandContext context, INetTunnel tunnel, IMessage message, MessageDispatcherContext dispatcherContext,
            IEndpointKeeperManager endpointKeeperManager)
        {
            CommandContext = context;
            Tunnel = tunnel;
            Message = message;
            DispatcherContext = dispatcherContext;
            EndpointKeeperManager = endpointKeeperManager;

        }


        public async Task Execute()
        {
            try
            {
                FireExecute();
                await Invoke();
                Done = true;
            } catch (Exception e)
            {
                CommandContext.Complete(e);
            } finally
            {
                FireDone(CommandContext.Cause);
                HandleResult();
            }
        }


        public bool IsDone()
        {
            return Done;
        }


        private void HandleResult()
        {
            IResultCode code;
            object body = null;
            var cause = CommandContext.Cause;
            var result = cause != null ? GetResultOnException(cause) : CommandContext.Result;
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
                    code = DefaultResultCode.SUCCESS;
                    body = result;
                    break;
            }
            MessageContext messageContext = null;
            switch (Message.Mode)
            {
                case MessageMode.Push:
                    messageContext = MessageContexts.Push(Message.Head, code, body);
                    break;
                case MessageMode.Request:
                    messageContext = MessageContexts.Respond(Message, code, body, Message.Id);
                    break;
            }
            if (messageContext != null)
            {
                TunnelAide.ResponseMessage(Tunnel, messageContext);
            }
        }


        private IRpcResult GetResultOnException(Exception e)
        {

            LOGGER.LogError(e, "Controller [{}] exception", Name);
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


        protected abstract Task Invoke();


        private void FireDone(Exception cause)
        {
            DispatcherContext.FireDone(this, cause);
        }


        private void FireExecute()
        {
            DispatcherContext.FireExecute(this);
        }
    }
}
