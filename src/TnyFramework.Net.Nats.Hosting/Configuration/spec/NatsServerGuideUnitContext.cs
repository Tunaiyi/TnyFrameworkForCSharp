using System;
using Microsoft.Extensions.DependencyInjection;
using NATS.Client.Core;
using TnyFramework.DI.Units;
using TnyFramework.Net.Hosting;
using TnyFramework.Net.Hosting.Guide;
using TnyFramework.Net.Nats.Codecs.TypeProtobuf;
using TnyFramework.Net.Nats.Options;
using TnyFramework.Net.Nats.Transports;
using TnyFramework.Net.Rpc.Client;

namespace TnyFramework.Net.Nats.Hosting.Configuration.spec;

public class NatsServerGuideUnitContext : NetGuideUnitContext<INatsServerGuideUnitContext>, INatsServerGuideUnitContext
{
    public NatsServerSettingSpec ServerSettingSpec { get; } = new();

    public NatsOpsSpec NatsOpsSpec { get; } = new();

    // public UnitSpec<IRpcClientFactory, INatsServerGuideUnitContext> RpcClientFactorySpec { get; }

    public UnitSpec<IMessageCodec, INatsServerGuideUnitContext> MessageCodecSpec { get; }

    public NatsServerGuideUnitContext(INetUnitContext unitContext, IServiceCollection unitContainer) : base(unitContext,
        unitContainer)
    {
        // MessageCodec
        MessageCodecSpec = UnitSpec.Unit<IMessageCodec, INatsServerGuideUnitContext>()
            .Default(DefaultMessageCodec);
        // RpcClientFactorySpec = UnitSpec.Unit<IRpcClientFactory, INatsServerGuideUnitContext>()
        //     .Default(DefaultClientFactory);
    }

    protected override void OnSetName(string name)
    {
        // RpcClientFactorySpec.WithNamePrefix(name);
        MessageCodecSpec.WithNamePrefix(name);
    }

    public IMessageCodec LoadMessageCodec()
    {
        return MessageCodecSpec.Load(Self, UnitContainer);
    }

    public NatsOpts LoadNatsOpts()
    {
        return NatsOpsSpec.Load(Self, UnitContainer);
    }

    public INatsServerSetting LoadNatsServerSetting()
    {
        return ServerSettingSpec.Load(Self, UnitContainer);
    }

    public IRpcClientFactory LoadRpcClientFactory()
    {
        throw new NotImplementedException();
    }

    private IMessageCodec DefaultMessageCodec(INatsServerGuideUnitContext context)
    {
        return new TypeProtobufMessageCodec();
    }
}