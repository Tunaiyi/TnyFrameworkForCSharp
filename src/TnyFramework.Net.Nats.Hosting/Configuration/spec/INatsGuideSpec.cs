using System;
using TnyFramework.DI.Units;
using TnyFramework.Net.Application;
using TnyFramework.Net.Hosting.Guide;
using TnyFramework.Net.Nats.Options;
using TnyFramework.Net.Nats.Transports;

namespace TnyFramework.Net.Nats.Hosting.Configuration.spec;

public interface INatsGuideSpec<TGuide, TContext, out TSpec> : INetGuideSpec<TGuide, TContext, TSpec>
    where TGuide : IServerGuide<INatsServerSetting>
    where TContext : INatsGuideUnitContext
    where TSpec : INetGuideSpec<TGuide, TContext, TSpec>
{
    public TSpec MessageCodecConfigure(Action<UnitSpec<IMessageCodec, TContext>> action);
}