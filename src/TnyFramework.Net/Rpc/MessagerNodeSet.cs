// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System.Collections.Generic;
using System.Collections.Immutable;
using TnyFramework.Net.Base;
using TnyFramework.Net.Endpoint;

namespace TnyFramework.Net.Rpc
{

    public class ContactNodeSet : IRpcInvokeNodeSet, IRpcInvokeNode
    {
        private readonly IList<IRpcInvokeNode> remoterList;

        private IEndpointKeeper? keeper;

        public ContactNodeSet(IContactType contactType)
        {
            this.ServiceType = contactType;
            remoterList = ImmutableList.Create((IRpcInvokeNode) this);
        }

        internal void Bind(IEndpointKeeper keeper)
        {
            this.keeper = keeper;
        }

        public IList<IRpcInvokeNode> GetOrderInvokeNodes()
        {
            return remoterList;
        }

        public IRpcInvokeNode FindInvokeNode(long nodeId) => this;

        public IRpcAccess? FindInvokeAccess(long nodeId, long accessId) => GetAccess(accessId);

        public long NodeId => 0;

        public IContactType ServiceType { get; }

        public IList<IRpcAccess> GetOrderAccesses() => ImmutableList<IRpcAccess>.Empty;

        public IRpcAccess? GetAccess(long accessId)
        {
            var endpoint = keeper?.GetEndpoint(accessId);
            return endpoint != null ? RpcContactAccess.Of(endpoint) : null;
        }

        public bool IsActive() => true;
    }

}
