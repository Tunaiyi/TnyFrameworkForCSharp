using System;
using NATS.Client.Core;
using TnyFramework.DI.Units;

namespace TnyFramework.Net.Nats.Hosting.Configuration.spec;

public class NatsOpsSpec : UnitSpec<NatsOpts, object>, INatsOpsSpec
{
    private NatsOpts Opts { get; set; } = new();

    public NatsOpsSpec(string unitName = "") : base(unitName)
    {
        Default(_ => Opts);
    }

    /// <summary>
    /// 配置Opts
    /// </summary>
    /// <param name="opts"></param>
    /// <returns></returns>
    public NatsOpsSpec Nats(NatsOpts opts)
    {
        Opts = opts;
        return this;
    }

    /// <summary>
    /// 配置Opts
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    public NatsOpsSpec ConfigureNats(Action<NatsOpts> action)
    {
        action(Opts);
        return this;
    }

    /// <summary>
    /// 配置Opts
    /// </summary>
    /// <param name="factory">配置行为</param>
    /// <returns></returns>
    public NatsOpsSpec ConfigureNats(Func<NatsOpts> factory)
    {
        Creator(context => factory());
        return this;
    }
}