// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TnyFramework.Common.Result;
using TnyFramework.Net.Transport;

namespace TnyFramework.Net.Message
{

    public abstract class MessageContent : MessageHeaderContainer, IMessageWritable, IMessageSent, IMessageSubject
    {
        /// <summary>
        /// 获取结果码
        /// </summary>
        public abstract IResultCode ResultCode { get; }

        /// <summary>
        /// 获取结果码
        /// </summary>
        public int Code => ResultCode.Value;

        /// <summary>
        /// 设置消息 body
        /// </summary>
        /// <param name="messageBody"> 消息body</param>
        /// <returns>返回当前 context</returns>
        public abstract MessageContent WithBody(object? messageBody);

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

        public abstract bool Respond(out Task<IMessage> task);

        public abstract bool IsWaitRespond { get; }

        public bool Written { get; private set; }

        public abstract int ProtocolId { get; }

        public abstract int Line { get; }

        public abstract long ToMessage { get; }

        public abstract MessageMode Mode { get; }

        public abstract bool ExistBody { get; }

        public abstract object? Body { get; protected set; }

        public abstract MessageContent WithHeader(MessageHeader? header);

        public abstract MessageContent WithHeader<TH>(Action<TH> action) where TH : MessageHeader, new();

        public abstract MessageContent WithHeaders(IEnumerable<MessageHeader> values);

        public abstract T? BodyAs<T>();

        void IMessageWritable.Written()
        {
            Written = true;
        }
    }

}
