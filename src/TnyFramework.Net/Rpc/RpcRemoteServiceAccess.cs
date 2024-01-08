// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using TnyFramework.Net.Message;
using TnyFramework.Net.Rpc.Extensions;
using TnyFramework.Net.Session;

namespace TnyFramework.Net.Rpc
{

    public class RpcRemoteServiceAccess : IRpcServiceAccess
    {
        private readonly ISession session;

        public ISession Session => session;

        public long AccessId => Session.ContactId;

        public ForwardPoint ForwardPoint { get; }

        public RpcRemoteServiceAccess(ISession session)
        {
            this.session = session;
            ForwardPoint = new ForwardPoint(session.GetRpcAccessIdentify());
        }

        public bool IsActive()
        {
            return Session.IsActive();
        }

        public RpcAccessIdentify Identify => session.GetRpcAccessIdentify();
    }

}
