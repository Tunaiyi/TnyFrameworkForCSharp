// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using TnyFramework.Common.Result;
using TnyFramework.Net.Message;

namespace TnyFramework.Net.Transport
{

    public static class MessageContents
    {
        /// <summary>
        /// 创建推送消息上下文
        /// </summary>
        /// <param name="protocol">协议号</param>
        /// <returns>创建的消息上下文</returns>
        public static MessageContent Push(IProtocol protocol)
        {
            return Push(protocol, ResultCode.SUCCESS);
        }

        /// <summary>
        /// 创建推送消息上下文
        /// </summary>
        /// <param name="protocol">协议号</param>
        /// <param name="code">消息结果码</param>
        /// <returns>创建的消息上下文</returns>
        public static MessageContent Push(IProtocol protocol, IResultCode code)
        {
            return new DefaultMessageContent(MessageMode.Push, protocol, code);
        }

        /// <summary>
        /// 创建推送消息上下文
        /// </summary>
        /// <param name="protocol">协议号</param>
        /// <param name="body">消息体</param>
        /// <returns>创建的消息上下文</returns>
        public static MessageContent Push(IProtocol protocol, object body)
        {
            return new DefaultMessageContent(MessageMode.Push, protocol, ResultCode.SUCCESS)
                .WithBody(body);
        }

        /// <summary>
        /// 创建推送消息上下文
        /// </summary>
        /// <param name="protocol">协议号</param>
        /// <param name="code">消息结果码</param>
        /// <param name="body">消息体</param>
        /// <returns>创建的消息上下文</returns>
        public static MessageContent Push(IProtocol protocol, IResultCode code, object body)
        {
            return new DefaultMessageContent(MessageMode.Push, protocol, code)
                .WithBody(body);
        }

        /// <summary>
        /// 创建请求消息上下文
        /// </summary>
        /// <param name="protocol">协议号</param>
        /// <param name="requestParams">请求参数, Body 转为 List</param>
        /// <returns>创建的消息上下文</returns>
        public static RequestContent Request(IProtocol protocol, params object[] requestParams)
        {
            return new DefaultMessageContent(MessageMode.Request, protocol, ResultCode.SUCCESS)
                .WithRequestBody(new MessageParamList(requestParams));
        }

        /// <summary>
        /// 创建响应消息上下文
        /// </summary>
        /// <param name="protocol">协议号</param>
        /// <param name="toMessage">响应的请求消息Id</param>
        /// <returns>创建的消息上下文</returns>
        public static MessageContent Respond(IProtocol protocol, long toMessage)
        {
            return Respond(protocol, ResultCode.SUCCESS, toMessage);
        }

        /// <summary>
        /// 创建响应消息上下文
        /// </summary>
        /// <param name="protocol">协议号</param>
        /// <param name="code">消息结果码</param>
        /// <param name="toMessage">响应的请求消息Id</param>
        /// <returns>创建的消息上下文</returns>
        public static MessageContent Respond(IProtocol protocol, IResultCode code, long toMessage)
        {
            return new DefaultMessageContent(MessageMode.Response, protocol, code, toMessage);
        }

        /// <summary>
        /// 创建响应消息上下文
        /// </summary>
        /// <param name="protocol">协议号</param>
        /// <param name="body">消息体</param>
        /// <param name="toMessage">响应的请求消息Id</param>
        /// <returns>创建的消息上下文</returns>
        public static MessageContent Respond(IProtocol protocol, object body, long toMessage)
        {
            return new DefaultMessageContent(MessageMode.Response, protocol, ResultCode.SUCCESS, toMessage)
                .WithRequestBody(body);
        }

        /// <summary>
        /// 创建响应消息上下文
        /// </summary>
        /// <param name="protocol">协议号</param>
        /// <param name="code">消息结果码</param>
        /// <param name="body">消息体</param>
        /// <param name="toMessage">响应的请求消息Id</param>
        /// <returns>创建的消息上下文</returns>
        public static MessageContent Respond(IProtocol protocol, IResultCode code, object body, long toMessage)
        {

            return new DefaultMessageContent(MessageMode.Response, protocol, code, toMessage)
                .WithRequestBody(body);
        }

        /// <summary>
        /// 创建响应消息上下文
        /// </summary>
        /// <param name="code">消息结果码</param>
        /// <param name="request">响应的请求消息</param>
        /// <returns>创建的消息上下文</returns>
        public static MessageContent Respond(IResultCode code, IMessageHead request)
        {
            return new DefaultMessageContent(MessageMode.Response, request, code, request.Id);
        }
    }

}
