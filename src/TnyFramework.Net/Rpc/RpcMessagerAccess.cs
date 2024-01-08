// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using TnyFramework.Common.Attribute;
using TnyFramework.Net.Session;

namespace TnyFramework.Net.Rpc
{

    public class RpcContactAccess : IRpcAccess
    {
        private static readonly AttrKey<RpcContactAccess> REMOTER_ACCESS = AttrKeys.Key<RpcContactAccess>("REMOTER_ACCESS");

        private readonly ISession session;

        public static RpcContactAccess Of(ISession session)
        {
            var attributes = session.Attributes;
            var access = attributes.Get(REMOTER_ACCESS);
            return access ?? attributes.Load(REMOTER_ACCESS, () => new RpcContactAccess(session));
        }

        private RpcContactAccess(ISession session)
        {
            this.session = session;
        }

        public long AccessId => session.ContactId;

        public bool IsActive() => session.IsActive();

        public ISession Session => session;
    }

}
