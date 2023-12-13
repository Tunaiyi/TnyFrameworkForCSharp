// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using TnyFramework.Net.Endpoint;
using TnyFramework.Net.Message;
using TnyFramework.Net.Rpc.Extensions;

namespace TnyFramework.Net.Rpc
{

    public class RpcRemoteServiceAccess : IRpcServiceAccess
    {
        private readonly IEndpoint endpoint;

        public IEndpoint Endpoint => endpoint;

        public long AccessId => Endpoint.ContactId;

        public ForwardPoint ForwardPoint { get; }

        public RpcRemoteServiceAccess(IEndpoint endpoint)
        {
            this.endpoint = endpoint;
            ForwardPoint = new ForwardPoint(endpoint.GetRpcAccessIdentify());
        }

        public bool IsActive()
        {
            return Endpoint.IsActive();
        }

        public RpcAccessIdentify Identify => endpoint.GetRpcAccessIdentify();
    }

}
