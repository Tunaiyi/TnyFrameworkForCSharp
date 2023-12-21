// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using Microsoft.Extensions.DependencyInjection;
using TnyFramework.Common.Extensions;
using TnyFramework.DI.Extensions;
using TnyFramework.DI.Units;
using TnyFramework.Net.Rpc.Remote;

namespace TnyFramework.Net.Hosting.Rpc
{

    public class RpcRemoteUnitContext : IRpcRemoteUnitContext
    {
        private IServiceCollection UnitContainer { get; }

        public UnitSpec<RpcRemoteSetting, IRpcRemoteUnitContext> RpcRemoteSettingSpec { get; }

        public UnitSpec<IRpcRouter, IRpcRemoteUnitContext> DefaultRpcRemoteRouterSpec { get; }

        public RpcRemoteUnitContext(IServiceCollection unitContainer)
        {
            UnitContainer = unitContainer;
            DefaultRpcRemoteRouterSpec = UnitSpec.Unit<IRpcRouter, IRpcRemoteUnitContext>()
                .Default<FirstRpcRouter>();
            RpcRemoteSettingSpec = UnitSpec.Unit<RpcRemoteSetting, IRpcRemoteUnitContext>()
                .Default<RpcRemoteSetting>();
            unitContainer.BindSingleton<RpcRemoteInstanceFactory>();
            unitContainer.BindSingleton(provider => {
                IRpcRouter? defaultRpcRouter = null;
                if (DefaultRpcRemoteRouterSpec.IsNotNull())
                {
                    defaultRpcRouter = DefaultRpcRemoteRouterSpec.Load(this, UnitContainer);
                }
                return new RpcRemoteRouteManager(defaultRpcRouter, provider.GetServices<IRpcRouter>());
            });

        }

        public void Load()
        {
            DefaultRpcRemoteRouterSpec.Load(this, UnitContainer);
            RpcRemoteSettingSpec.Load(this, UnitContainer);
        }
    }

}
