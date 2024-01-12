using System;
using Microsoft.Extensions.DependencyInjection;
using TnyFramework.Net.Hosting.Rpc;
using TnyFramework.Net.Nats.Guide;
using TnyFramework.Net.Nats.Hosting.Configuration.spec;
using TnyFramework.Net.Nats.Options;
using TnyFramework.Net.Nats.Transports;

namespace TnyFramework.Net.Nats.Hosting.Configuration;

public class NatsRpcHostServerConfiguration :
    RpcHostServerConfiguration<INatsRpcHostServerConfiguration, INatsServerGuideUnitContext, INatsServerGuide,
        INatsServerSetting, INatsServerGuideSpec>,
    INatsRpcHostServerConfiguration
{
    public static INatsRpcHostServerConfiguration CreateRpcServer(IServiceCollection unitContainer)
    {
        return new NatsRpcHostServerConfiguration(unitContainer);
    }

    private NatsRpcHostServerConfiguration(IServiceCollection unitContainer) : base(unitContainer)
    {

    }

    public override INatsRpcHostServerConfiguration RpcServer(INatsServerSetting setting,
        Action<INatsServerGuideSpec>? action = null)
    {
        Server(setting.Service, spec => {
            spec.Server(setting);
            action?.Invoke(spec);
        });
        OnAddRpcServer(setting.Service);
        return this;
    }

    protected override INatsServerGuideSpec CreateServerGuideSpec(string name)
    {
        return new NatsServerGuideSpec(name, NetUnitContext, UnitContainer);
    }

    protected override void OnAddRpcServer(string name)
    {
        // EndpointConfigure(endpointSpec => endpointSpec
        //     .CustomSessionConfigure(UnitContainer.UnitName<ISessionKeeperSettingSpec>(name), settingSpec => settingSpec
        //         .UserType(name)
        //         .KeeperFactory(RPC_SESSION_KEEPER_NAME)));
    }

    // protected override void OnPostInitialize(INatsServerGuide guide)
    // {
    //     var key = new NatsRpcClientFactory(unit, RpcServiceSetting setting);
    // }
}