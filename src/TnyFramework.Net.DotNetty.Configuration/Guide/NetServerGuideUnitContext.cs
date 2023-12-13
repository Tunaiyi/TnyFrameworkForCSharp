// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using Microsoft.Extensions.DependencyInjection;
using TnyFramework.DI.Units;
using TnyFramework.Net.Base;
using TnyFramework.Net.DotNetty.Transport;

namespace TnyFramework.Net.DotNetty.Configuration.Guide
{

    public class NetServerGuideUnitContext<TUserId> : NetGuideUnitContext<TUserId>, INetServerGuideUnitContext<TUserId>
    {
        public ServerSettingSpec ServerSettingSpec { get; }

        private UnitSpec<INetworkContext, INetServerGuideUnitContext<TUserId>> NetworkContextSpec { get; }

        public NetServerGuideUnitContext(INetUnitContext unitContext, IServiceCollection unitContainer) : base(unitContext, unitContainer)
        {
            ServerSettingSpec = new ServerSettingSpec();
            TunnelFactorySpec.Default<ServerTunnelFactory<TUserId>>();
            NetworkContextSpec = UnitSpec.Unit<INetworkContext, INetServerGuideUnitContext<TUserId>>()
                .Default(DefaultNetworkContext<TUserId>);
        }

        protected override void OnSetName(string name)
        {
            ServerSettingSpec.WithNamePrefix(name);
            TunnelFactorySpec.WithNamePrefix(name);
            NetworkContextSpec.WithNamePrefix(name);
        }

        public IServerSetting LoadServerSetting()
        {
            return ServerSettingSpec.Load(this, UnitContainer);
        }

        public override INetworkContext LoadNetworkContext()
        {
            return NetworkContextSpec.Load(this, UnitContainer);
        }

        private static INetworkContext DefaultNetworkContext<T>(INetServerGuideUnitContext<TUserId> context)
        {
            var unitContext = context.UnitContext;
            return new NetworkContext<TUserId>(
                context.LoadServerSetting(),
                unitContext.LoadMessageDispatcher(),
                unitContext.LoadCommandTaskProcessor(),
                context.LoadMessageFactory(),
                context.LoadContactFactory(),
                context.LoadCertificateFactory(),
                context.LoadRpcMonitor());
        }
    }

}
