using System;
using NATS.Client.Core;

namespace TnyFramework.Net.Nats.Hosting.Configuration.spec;

public interface INatsOpsSpec
{
    /// <summary>
    /// 配置Opts
    /// </summary>
    /// <param name="opts"></param>
    /// <returns></returns>
    NatsOpsSpec Nats(NatsOpts opts);

    /// <summary>
    /// 配置Opts
    /// </summary>
    /// <param name="factory">配置行为</param>
    /// <returns></returns>
    NatsOpsSpec ConfigureNats(Func<NatsOpts> factory);
}