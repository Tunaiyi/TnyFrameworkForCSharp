// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System.Threading.Tasks;
using TnyFramework.Net.Endpoint;
using TnyFramework.Net.Message;
using TnyFramework.Net.Rpc.Extensions;
using TnyFramework.Net.Transport;

namespace TnyFramework.Net.Rpc
{

    public class RpcEndpointAccessPoint : IRpcRemoteAccessPoint
    {
        public RpcAccessIdentify AccessId => Endpoint.GetRpcAccessIdentify();

        public ForwardPoint ForwardPoint { get; }

        private IEndpoint Endpoint { get; }

        public RpcEndpointAccessPoint(IEndpoint endpoint)
        {
            Endpoint = endpoint;
            ForwardPoint = new ForwardPoint(AccessId);
        }

        public ValueTask<IMessageSent> Send(MessageContent content, bool waitWritten = false)
        {
            return Endpoint.Send(content, waitWritten);
        }

        public int CompareTo(IRpcRemoteAccessPoint? other)
        {
            if (ReferenceEquals(this, other)) return 0;
            return ReferenceEquals(null, other) ? 1 : AccessId.CompareTo(other.AccessId);
        }
    }

}
