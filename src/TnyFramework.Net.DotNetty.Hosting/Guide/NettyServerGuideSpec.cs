// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using Microsoft.Extensions.DependencyInjection;
using TnyFramework.Net.DotNetty.Guide;
using TnyFramework.Net.Hosting;

namespace TnyFramework.Net.DotNetty.Hosting.Guide;

public class NettyServerGuideSpec :
    NettyGuideSpec<INettyServerGuide, INettyServerGuideUnitContext, NettyServerGuideUnitContext, INettyServerGuideSpec>,
    INettyServerGuideSpec
{
    public NettyServerGuideSpec(string name, INetUnitContext unitContext, IServiceCollection unitContainer) :
        base(unitContainer, new NettyServerGuideUnitContext(unitContext, unitContainer))
    {
        WithNamePrefix(name);

        Default(c => new NettyServerGuide(
            c.LoadServerSetting(),
            c.LoadTunnelFactory(),
            c.LoadNetworkContext(),
            c.LoadChannelMaker()));
        context.ServerSettingSpec.ServiceName(name);
    }

    protected override INettyServerGuideSpec Self()
    {
        return this;
    }

    public INettyServerGuideSpec Server(INettyServerSetting setting)
    {
        var serverSettings = context.ServerSettingSpec;
        serverSettings.Unit(setting);
        return this;
    }

    public INettyServerGuideSpec Server(int port)
    {
        var serverSettings = context.ServerSettingSpec;
        serverSettings.Port(port);
        return this;
    }

    public INettyServerGuideSpec Server(string host, int port)
    {
        var serverSettings = context.ServerSettingSpec;
        serverSettings.Host(host).Port(port);
        return this;
    }

    public INettyServerGuideSpec Server(string host, int port, bool libuv)
    {
        var serverSettings = context.ServerSettingSpec;
        serverSettings.Host(host).Port(port).Libuv(libuv);
        return this;

    }

    public INettyServerGuideSpec Server(string serveName, string host, int port)
    {
        var serverSettings = context.ServerSettingSpec;
        serverSettings.ServeName(serveName).Host(host).Port(port);
        return this;
    }

    public INettyServerGuideSpec Server(string serveName, string host, int port, bool libuv)
    {
        var serverSettings = context.ServerSettingSpec;
        serverSettings.ServeName(serveName).Host(host).Port(port).Libuv(libuv);
        return this;
    }
}
