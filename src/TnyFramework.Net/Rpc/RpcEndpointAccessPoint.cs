// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using TnyFramework.Net.Endpoint;
using TnyFramework.Net.Transport;

namespace TnyFramework.Net.Rpc
{

    public class RpcEndpointAccessPoint : IRpcRemoteAccessPoint
    {
        public RpcAccessIdentify AccessId => Endpoint.UserId;

        public Message.ForwardPoint ForwardPoint { get; }

        internal IEndpoint<RpcAccessIdentify> Endpoint { get; }

        public RpcEndpointAccessPoint(IEndpoint<RpcAccessIdentify> endpoint)
        {
            Endpoint = endpoint;
            ForwardPoint = new Message.ForwardPoint(AccessId);
        }

        public ISendReceipt Send(MessageContext messageContext)
        {
            return Endpoint.Send(messageContext);
        }

        public int CompareTo(IRpcRemoteAccessPoint other)
        {
            if (ReferenceEquals(this, other)) return 0;
            return ReferenceEquals(null, other) ? 1 : AccessId.CompareTo(other.AccessId);
        }
    }

}
