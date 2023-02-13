// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using TnyFramework.Common.Attribute;
using TnyFramework.Net.Endpoint;

namespace TnyFramework.Net.Rpc
{

    public class RpcMessagerAccess : IRpcAccess
    {
        private static readonly AttrKey<RpcMessagerAccess> REMOTER_ACCESS = AttrKeys.Key<RpcMessagerAccess>("REMOTER_ACCESS");

        private readonly IEndpoint endpoint;

        public static RpcMessagerAccess Of(IEndpoint endpoint)
        {
            var attributes = endpoint.Attributes;
            var access = attributes.Get(REMOTER_ACCESS);
            return access ?? attributes.Load(REMOTER_ACCESS, () => new RpcMessagerAccess(endpoint));
        }

        private RpcMessagerAccess(IEndpoint endpoint)
        {
            this.endpoint = endpoint;
        }

        public long AccessId => endpoint.MessagerId;

        public bool IsActive() => endpoint.IsActive();

        public IEndpoint Endpoint => endpoint;
    }

}
