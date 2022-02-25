using TnyFramework.Common.Result;
using TnyFramework.Net.Message;
namespace TnyFramework.Net.Transport
{
    public static class MessageContexts
    {
        /// <summary>
        /// 创建推送消息上下文
        /// </summary>
        /// <param name="protocol">协议号</param>
        /// <returns>创建的消息上下文</returns>
        public static MessageContext Push(IProtocol protocol)
        {
            return Push(protocol, DefaultResultCode.SUCCESS);
        }


        /// <summary>
        /// 创建推送消息上下文
        /// </summary>
        /// <param name="protocol">协议号</param>
        /// <param name="code">消息结果码</param>
        /// <returns>创建的消息上下文</returns>
        public static MessageContext Push(IProtocol protocol, IResultCode code)
        {
            return new DefaultMessageContext(MessageMode.Push, protocol, code);
        }


        /// <summary>
        /// 创建推送消息上下文
        /// </summary>
        /// <param name="protocol">协议号</param>
        /// <param name="body">消息体</param>
        /// <returns>创建的消息上下文</returns>
        public static MessageContext Push(IProtocol protocol, object body)
        {
            return new DefaultMessageContext(MessageMode.Push, protocol, DefaultResultCode.SUCCESS)
                .WithBody(body);
        }


        /// <summary>
        /// 创建推送消息上下文
        /// </summary>
        /// <param name="protocol">协议号</param>
        /// <param name="code">消息结果码</param>
        /// <param name="body">消息体</param>
        /// <returns>创建的消息上下文</returns>
        public static MessageContext Push(IProtocol protocol, IResultCode code, object body)
        {
            return new DefaultMessageContext(MessageMode.Push, protocol, code)
                .WithBody(body);
        }



        /// <summary>
        /// 创建请求消息上下文
        /// </summary>
        /// <param name="protocol">协议号</param>
        /// <returns>创建的消息上下文</returns>
        public static RequestContext Request(IProtocol protocol)
        {
            return new DefaultMessageContext(MessageMode.Request, protocol, DefaultResultCode.SUCCESS);
        }


        /// <summary>
        /// 创建请求消息上下文
        /// </summary>
        /// <param name="protocol">协议号</param>
        /// <param name="body">请求消息体</param>
        /// <returns>创建的消息上下文</returns>
        public static RequestContext Request(IProtocol protocol, object body)
        {
            return new DefaultMessageContext(MessageMode.Request, protocol, DefaultResultCode.SUCCESS)
                .WithRequestBody(body);
        }


        /// <summary>
        /// 创建请求消息上下文
        /// </summary>
        /// <param name="protocol">协议号</param>
        /// <param name="requestParams">请求参数, Body 转为 List</param>
        /// <returns>创建的消息上下文</returns>
        public static RequestContext RequestWithParams(IProtocol protocol, params object[] requestParams)
        {
            return new DefaultMessageContext(MessageMode.Request, protocol, DefaultResultCode.SUCCESS)
                .WithRequestBody(new MessageParamList(requestParams));
        }


        /// <summary>
        /// 创建响应消息上下文
        /// </summary>
        /// <param name="protocol">协议号</param>
        /// <param name="toMessage">响应的请求消息Id</param>
        /// <returns>创建的消息上下文</returns>
        public static MessageContext Respond(IProtocol protocol, long toMessage)
        {
            return Respond(protocol, DefaultResultCode.SUCCESS, toMessage);
        }


        /// <summary>
        /// 创建响应消息上下文
        /// </summary>
        /// <param name="protocol">协议号</param>
        /// <param name="code">消息结果码</param>
        /// <param name="toMessage">响应的请求消息Id</param>
        /// <returns>创建的消息上下文</returns>
        public static MessageContext Respond(IProtocol protocol, IResultCode code, long toMessage)
        {
            return new DefaultMessageContext(MessageMode.Response, protocol, code, toMessage);
        }


        /// <summary>
        /// 创建响应消息上下文
        /// </summary>
        /// <param name="protocol">协议号</param>
        /// <param name="body">消息体</param>
        /// <param name="toMessage">响应的请求消息Id</param>
        /// <returns>创建的消息上下文</returns>
        public static MessageContext Respond(IProtocol protocol, object body, long toMessage)
        {
            return new DefaultMessageContext(MessageMode.Response, protocol, DefaultResultCode.SUCCESS, toMessage)
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
        public static MessageContext Respond(IProtocol protocol, IResultCode code, object body, long toMessage)
        {

            return new DefaultMessageContext(MessageMode.Response, protocol, code, toMessage)
                .WithRequestBody(body);
        }


        /// <summary>
        /// 创建响应消息上下文
        /// </summary>
        /// <param name="code">消息结果码</param>
        /// <param name="request">响应的请求消息</param>
        /// <returns>创建的消息上下文</returns>
        public static MessageContext Respond(IResultCode code, IMessageHead request)
        {
            return new DefaultMessageContext(MessageMode.Response, request, code, request.Id);
        }
    }
}
