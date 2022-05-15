using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using TnyFramework.Common.Exceptions;
using TnyFramework.Common.Extensions;
using TnyFramework.Common.Result;
using TnyFramework.Net.Message;

namespace TnyFramework.Net.Rpc.Remote
{

    public class RpcRemoteInvokeParams
    {
        public object[] Params { get; }

        public IRpcServicer From { get; internal set; }

        public IMessager Sender { get; internal set; }

        public IRpcServicer To { get; internal set; }

        public IMessager Receiver { get; internal set; }

        public object RouteValue { get; internal set; }

        public IResultCode Code { get; internal set; } = ResultCode.SUCCESS;

        private readonly IDictionary<string, MessageHeader> headerMap = new Dictionary<string, MessageHeader>();

        public IDictionary<string, MessageHeader> HeaderMap => headerMap.ToImmutableDictionary();

        public RpcRemoteInvokeParams(int size)
        {
            Params = new object[size];
        }

        public IList<MessageHeader> GetAllHeaders()
        {
            if (headerMap.ContainsKey(MessageHeaderConstants.RPC_FORWARD_HEADER_KEY))
                return headerMap.Values.ToImmutableList();
            if (To.IsNull() && !From.IsNull() && Receiver.IsNull() && Sender.IsNull())
                return headerMap.Values.ToImmutableList();
            var forwardHeader = new RpcForwardHeader()
                .SetFrom(From)
                .SetTo(To)
                .SetSender(Sender)
                .SetReceiver(Receiver);
            headerMap.Add(forwardHeader.Key, forwardHeader);
            return headerMap.Values.ToImmutableList();
        }

        internal void SetParams(int index, Object value)
        {
            if (Params[index] == null)
            {
                Params[index] = value;
            } else
            {
                throw new IllegalArgumentException($"参数 {index} 已设置");
            }
        }

        public object GetBody()
        {
            return Params.Length == 0 ? null : Params[0];
        }

        internal RpcRemoteInvokeParams PutHeader(MessageHeader messageHeader)
        {
            headerMap[messageHeader.Key] = messageHeader;
            return this;
        }

        internal RpcRemoteInvokeParams SetTo(IRpcServiceType toService)
        {
            To = new ForwardRpcServicer(toService);
            return this;
        }

        internal RpcRemoteInvokeParams SetCode(object code)
        {
            switch (code)
            {
                case int id:
                    Code = ResultCode.ForId(id);
                    break;
                case ResultCode resultCode:
                    Code = resultCode;
                    break;
                default:
                    throw new InvalidCastException($"{code.GetType()}类型参数 {code}, 无法解析位 {typeof(IResultCode)}");
            }
            return this;
        }

        internal RpcRemoteInvokeParams SetBody(Object body)
        {
            SetParams(0, body);
            return this;
        }
    }

}
