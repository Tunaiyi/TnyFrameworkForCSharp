using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TnyFramework.Common.Result;
using TnyFramework.Net.Message;

namespace TnyFramework.Net.Transport
{

    public abstract class MessageContext : ISendReceipt, IMessageContent
    {
        /// <summary>
        /// 获取结果码
        /// </summary>
        public abstract IResultCode ResultCode { get; }

        /// <summary>
        /// 设置消息 body
        /// </summary>
        /// <param name="messageBody"> 消息body</param>
        /// <returns>返回当前 context</returns>
        public abstract MessageContext WithBody(object messageBody);

        /// <summary>
        /// 取消 是否打断
        /// </summary>
        /// <param name="mayInterruptIfRunning"></param>
        public abstract void Cancel(bool mayInterruptIfRunning);

        /// <summary>
        /// 因为异常而取消
        /// </summary>
        /// <param name="cause">取消的的异常</param>
        public abstract void Cancel(Exception cause);

        public bool IsOwn(IProtocol protocol)
        {
            return protocol.ProtocolId == ProtocolId;
        }

        public int GetCode()
        {
            return ResultCode.Value;
        }

        public abstract Task<IMessage> Respond();

        public abstract bool IsRespondAwaitable();

        public abstract Task Written();

        public abstract bool IsWriteAwaitable();

        public abstract int ProtocolId { get; }

        public abstract int Line { get; }

        public abstract long ToMessage { get; }

        public abstract MessageType Type { get; }

        public abstract MessageMode Mode { get; }

        public abstract bool ExistBody { get; }

        public abstract object Body { get; protected set; }

        public abstract IDictionary<string, MessageHeader> Headers { get; }

        public abstract MessageContext WithHeader(MessageHeader header);

        public abstract MessageContext WithHeader<TH>(Action<TH> action) where TH : MessageHeader, new();

        public abstract MessageContext WithHeaders(IEnumerable<MessageHeader> values);

        public abstract T BodyAs<T>();
    }

}
