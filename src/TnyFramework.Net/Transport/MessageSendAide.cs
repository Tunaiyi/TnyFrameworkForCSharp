// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TnyFramework.Common.Logger;
using TnyFramework.Common.Result;
using TnyFramework.Net.Message;

namespace TnyFramework.Net.Transport
{

    public static class MessageSendAide
    {
        private static readonly ILogger LOGGER = LogFactory.Logger(nameof(MessageSendAide));

        private static RpcForwardHeader CreateBackForwardHeader(RpcForwardHeader messageForwardHeader)
        {
            if (messageForwardHeader != null)
            {
                return new RpcForwardHeader()
                    .SetFrom(messageForwardHeader.To)
                    .SetSender(messageForwardHeader.Receiver)
                    .SetTo(messageForwardHeader.From)
                    .SetReceiver(messageForwardHeader.Sender);
            }
            return null;
        }

        /// <summary>
        /// 发送响应消息, 如果 code 为 Error, 则发送完后断开连接
        /// </summary>
        /// <param name="tunnel">通道</param>
        /// <param name="request">响应请求</param>
        /// <param name="code">消息码</param>
        /// <param name="body">消息体</param>
        /// <returns>发送回执</returns>>
        public static void Response(INetTunnel tunnel, IMessage request, IResultCode code, object body)
        {
            var toMessage = request.Id;
            var idHeader = request.GetHeader(MessageHeaderConstants.RPC_ORIGINAL_MESSAGE_ID);
            if (idHeader != null)
            {
                toMessage = idHeader.MessageId;
            }
            var forwardHeader = request.GetHeader(MessageHeaderConstants.RPC_FORWARD_HEADER);
            var backForward = CreateBackForwardHeader(forwardHeader);
            var context = MessageContexts.Respond(request, code, body, toMessage)
                .WithHeader(backForward);
            Send(tunnel, context, backForward == null);
        }

        public static void Push(INetTunnel tunnel, IMessage request, IResultCode code, object body)
        {
            var messageForwardHeader = request.GetHeader(MessageHeaderConstants.RPC_FORWARD_HEADER);
            var backForward = CreateBackForwardHeader(messageForwardHeader);
            var context = MessageContexts.Push(request, code, body)
                .WithHeader(backForward);
            Send(tunnel, context, backForward == null);
        }

        /// <summary>
        /// 发送响应消息, 如果 code 为 Error, 则发送完后断开连接
        /// </summary>
        /// <param name="tunnel">通道</param>
        /// <param name="context">消息信息上下文</param>
        /// <param name="autoClose"></param>
        /// <returns>发送回执</returns>>
        public static void Send(INetTunnel tunnel, MessageContext context, bool autoClose = true)
        {
            var _ = DoSend(tunnel, context, autoClose);
        }

        private static async Task<ISendReceipt> DoSend(INetTunnel tunnel, MessageContext context, bool autoClose = true)
        {
            var close = context.ResultCode.Level == ResultLevel.Error;
            var receipt = tunnel.Send(context);
            if (!autoClose || !close)
            {
                return receipt;
            }
            await receipt.Written();
            tunnel.Close();
            return receipt;
        }
    }

}
