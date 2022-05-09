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
                    code = ResultCode.SUCCESS;
                    body = result;
                    break;
            }
            MessageContext messageContext = null;
            var messageForwardHeader = Message.GetHeader(MessageHeaderConstants.RPC_FORWARD_HEADER);
            var messageIdHeader = Message.GetHeader(MessageHeaderConstants.RPC_ORIGINAL_MESSAGE_ID);
            switch (Message.Mode)
            {
                case MessageMode.Push:
                    if (body != null)
                    {
                        messageContext = MessageContexts.Push(Message.Head, code, body)
                            .WithHeader(CreateBackForwardHeader(messageForwardHeader));
                    }
                    break;
                case MessageMode.Request:
                    var toMessage = messageIdHeader?.MessageId ?? Message.Id;
                    messageContext = MessageContexts.Respond(Message, code, body, toMessage)
                        .WithHeader(CreateBackForwardHeader(messageForwardHeader));
                    break;
                case MessageMode.Response:
                case MessageMode.Ping:
                case MessageMode.Pong:
                default:
                    throw new ArgumentOutOfRangeException();
            }
            if (messageContext != null)
            {
                TunnelAide.ResponseMessage(Tunnel, messageContext);
            }
        }


        private RpcForwardHeader CreateBackForwardHeader(RpcForwardHeader messageForwardHeader)
        {
            if (messageForwardHeader != null)
            {
                return new RpcForwardHeader {
                    From = messageForwardHeader.To,
                    Sender = messageForwardHeader.Receiver,
                    To = messageForwardHeader.From,
                    Receiver = messageForwardHeader.Sender,
                    Forwarders = messageForwardHeader.Forwarders
                };
            }
            return null;
        }


        private IRpcResult GetResultOnException(Exception e)
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
