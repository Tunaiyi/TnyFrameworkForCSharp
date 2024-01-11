// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using Microsoft.Extensions.DependencyInjection;
using TnyFramework.Net.DotNetty.Bootstrap;
using TnyFramework.Net.DotNetty.Transport;
using TnyFramework.Net.Hosting;

namespace TnyFramework.Net.DotNetty.Hosting.Guide;

public class NettyServerGuideUnitContext : NettyGuideUnitContext<INettyServerGuideUnitContext>, INettyServerGuideUnitContext
{
    public ServerSettingSpec ServerSettingSpec { get; }

    public NettyServerGuideUnitContext(INetUnitContext unitContext, IServiceCollection unitContainer) : base(unitContext, unitContainer)
    {
        ServerSettingSpec = new ServerSettingSpec();
        TunnelFactorySpec.Default<ServerTunnelFactory>();
    }

    protected override void OnGuideUnitSetName(string name)
    {
        ServerSettingSpec.WithNamePrefix(name);
        TunnelFactorySpec.WithNamePrefix(name);
    }

    public INettyServerSetting LoadServerSetting()
    {
        return ServerSettingSpec.Load(this, UnitContainer);
    }
}
