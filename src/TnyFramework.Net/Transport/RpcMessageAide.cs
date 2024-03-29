// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System.Threading.Tasks;
using TnyFramework.Common.Extensions;
using TnyFramework.Common.Result;
using TnyFramework.Net.Command.Dispatcher;
using TnyFramework.Net.Message;
using TnyFramework.Net.Message.Extensions;

namespace TnyFramework.Net.Transport;

public static class RpcMessageAide
{
    /// <summary>
    /// 发送响应消息, 如果 code 为 Error, 则发送完后断开连接
    /// </summary>
    /// <param name="context">rpc上下文</param>
    /// <param name="code">消息码</param>
    /// <param name="body">消息体</param>
    /// <return>发送回执l</return>
    public static MessageContent ToMessage(IRpcMessageEnterContext context, IResultCode code, object? body)
    {
        var request = context.NetMessage;
        return ToMessage(request, code, body);
    }

    /// <summary>
    /// 发送响应消息, 如果 code 为 Error, 则发送完后断开连接
    /// </summary>
    /// <param name="request">请求</param>
    /// <param name="code">消息码</param>
    /// <param name="body">消息体</param>
    /// <return>发送回执l</return>
    public static MessageContent ToMessage(IMessage request, IResultCode code, object? body)
    {
        return request.Mode == MessageMode.Request ? Response(request, code, body) : Push(request, code, body);
    }

    /// <summary>
    /// 发送响应消息, 如果 code 为 Error, 则发送完后断开连接
    /// </summary>
    /// <param name="request">响应请求</param>
    /// <param name="code">消息码</param>
    /// <param name="body">消息体</param>
    /// <returns>发送回执</returns>>
    private static MessageContent Response(IMessage request, IResultCode code, object? body)
    {
        var forwardHeader = request.GetHeader(MessageHeaderKeys.RPC_FORWARD_HEADER);
        var backForward = CreateBackForwardHeader(forwardHeader);
        return PutTransitiveHeaders(request, MessageContents.Respond(request, code, body, request.GetOriginalId()))
            .WithHeader(backForward);
        // Send(tunnel, context, backForward == null);
    }

    /// <summary>
    /// 创建推送消息, 如果 code 为 Error, 则发送完后断开连接
    /// </summary>
    /// <param name="request">响应请求</param>
    /// <param name="code">消息码</param>
    /// <param name="body">消息体</param>
    /// <returns>发送回执</returns>>
    private static MessageContent Push(IMessage request, IResultCode code, object? body)
    {
        var messageForwardHeader = request.GetHeader(MessageHeaderKeys.RPC_FORWARD_HEADER);
        var backForward = CreateBackForwardHeader(messageForwardHeader);
        return PutTransitiveHeaders(request, MessageContents.Push(request, code, body)
            .WithHeader(backForward));
        // Send(tunnel, context, backForward == null);
    }

    private static RpcForwardHeader? CreateBackForwardHeader(RpcForwardHeader? messageForwardHeader)
    {
        if (messageForwardHeader != null)
        {
            return new RpcForwardHeader()
                .SetFrom(messageForwardHeader.To!)
                .SetSender(messageForwardHeader.Receiver!)
                .SetTo(messageForwardHeader.From!)
                .SetReceiver(messageForwardHeader.Sender!);
        }
        return null;
    }

    private static MessageContent PutTransitiveHeaders(IMessageHeaderContainer request, MessageContent content)
    {
        var headers = request.GetAllHeaders();
        if (headers.IsNull())
        {
            return content;
        }
        foreach (var header in headers)
        {
            if (header.HeaderKey.Usage.IsInherit(content.Mode))
            {
                content.WithHeader(header);
            }
        }
        return content;
    }

    /// <summary>
    /// 发送响应消息, 如果 code 为 Error, 则发送完后断开连接
    /// </summary>
    /// <param name="tunnel">通道</param>
    /// <param name="content">消息信息上下文</param>
    /// <param name="autoClose"></param>
    /// <returns>发送回执</returns>>
    public static ValueTask<IMessageSent> Send(INetTunnel tunnel, MessageContent content, bool autoClose = true)
    {
        return DoSend(tunnel, content, autoClose);
    }

    private static async ValueTask<IMessageSent> DoSend(INetTunnel tunnel, MessageContent content, bool autoClose = true)
    {
        var close = autoClose && content.ResultCode.Level == ResultLevel.Error;
        var sent = await tunnel.Send(content, close);
        if (!close)
        {
            return sent;
        }
        tunnel.Close();
        return sent;
    }
}
