// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using Microsoft.Extensions.DependencyInjection;
using TnyFramework.Net.DotNetty.Configuration.Guide;
using TnyFramework.Net.Rpc.Auth;

namespace TnyFramework.Net.DotNetty.Configuration.Rpc
{

    public class RpcUnitContext : IRpcUnitContext
    {
        private IServiceCollection UnitContainer { get; }

        public NetUnitContext NetUnitContext { get; }

        public RpcAuthServiceSpec RpcAuthServiceSpec { get; }

        public RpcUnitContext(NetUnitContext netUnitContext, IServiceCollection unitContainer)
        {
            UnitContainer = unitContainer;
            NetUnitContext = netUnitContext;
            RpcAuthServiceSpec = new RpcAuthServiceSpec(this, unitContainer);
        }

        public IRpcAuthService LoadRpcAuthService()
        {
            return RpcAuthServiceSpec.Load(this, UnitContainer);
        }

        public IRpcUserPasswordManager LoadRpcUserPasswordManager()
        {
            return RpcAuthServiceSpec.LoadRpcUserPasswordManager();
        }
    }

}
