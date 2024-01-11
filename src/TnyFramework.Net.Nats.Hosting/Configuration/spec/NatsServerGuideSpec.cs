using System;
using Microsoft.Extensions.DependencyInjection;
using NATS.Client.Core;
using TnyFramework.DI.Extensions;
using TnyFramework.DI.Units;
using TnyFramework.Net.Hosting;
using TnyFramework.Net.Hosting.Guide;
using TnyFramework.Net.Nats.Options;
using TnyFramework.Net.Nats.Transports;

namespace TnyFramework.Net.Nats.Hosting.Configuration.spec;

public class NatsServerGuideSpec
    : NetGuideSpec<INatsServerGuide, INatsServerGuideUnitContext, NatsServerGuideUnitContext, INatsServerGuideSpec>,
        INatsServerGuideSpec
{
    public NatsServerGuideSpec(string name, INetUnitContext unitContext, IServiceCollection unitContainer)
        : base(unitContainer, new NatsServerGuideUnitContext(unitContext, unitContainer))
    {
        WithNamePrefix(name);
        context.SetName(name);
        Default(c => new NatsServerGuide(
            c.LoadNatsOpts(),
            c.LoadNatsServerSetting(),
            unitContext.LoadAppContext(),
            c.LoadNetworkContext(),
            c.LoadMessageCodec()));
        context.ServerSettingSpec.ServiceName(name);
    }

    protected override INatsServerGuideSpec Self()
    {
        return this;
    }

    public INatsServerGuideSpec Host(string value)
    {
        context.ServerSettingSpec.Host(value);
        return this;
    }

    public INatsServerGuideSpec Port(int port)
    {
        context.ServerSettingSpec.Port(port);
        return this;
    }

    public INatsServerGuideSpec Server(INatsServerSetting setting)
    {
        var serverSettings = context.ServerSettingSpec;
        serverSettings.Unit(setting);
        return this;
    }

    public INatsServerGuideSpec ConfigureServer(Action<INatsServerSettingSpec> action)
    {
        action(context.ServerSettingSpec);
        return this;
    }

    public INatsServerGuideSpec ConfigureNats(Action<NatsOpts> action)
    {
        context.NatsOpsSpec.ConfigureNats(action);
        return this;
    }

    public INatsServerGuideSpec MessageCodecConfigure(
        Action<UnitSpec<IMessageCodec, INatsServerGuideUnitContext>> action)
    {
        action(context.MessageCodecSpec);
        return Self();
    }

    protected override void OnRegister(IServiceCollection services, string unitName, INatsServerGuide unit)
    {
        if (unit is INatsClientGuide clientGuide)
        {
            services.AddSingletonUnit(unitName, clientGuide);
        }
    }
}