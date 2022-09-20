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
using TnyFramework.Common.Exceptions;
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

        protected readonly INetTunnel tunnel;

        internal ITunnel Tunnel => tunnel;

        internal IEndpoint Endpoint => tunnel.GetEndpoint();

        internal IMessage Message { get; }

        internal RpcForwardHeader Forward { get; }

        internal string AppType => DispatcherContext.AppType;

        internal string ScopeType => DispatcherContext.ScopeType;

        protected MessageCommand(MessageCommandContext context, INetTunnel tunnel, IMessage message, MessageDispatcherContext dispatcherContext,
            IEndpointKeeperManager endpointKeeperManager)
        {
            this.tunnel = tunnel;
            CommandContext = context;
            Message = message;
            DispatcherContext = dispatcherContext;
            EndpointKeeperManager = endpointKeeperManager;
            Forward = message.GetHeader(MessageHeaderConstants.RPC_FORWARD_HEADER);
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
            switch (Message.Mode)
            {
                case MessageMode.Push:
                    if (body != null)
                    {
                        MessageSendAide.Push(tunnel, Message, code, body);
                    }
                    break;
                case MessageMode.Request:
                    MessageSendAide.Response(tunnel, Message, code, body);
                    break;
                case MessageMode.Response:
                case MessageMode.Ping:
                case MessageMode.Pong:
                default:
                    throw new ArgumentOutOfRangeException();
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
                    ToForwarder = messageForwardHeader.FromForwarder
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
