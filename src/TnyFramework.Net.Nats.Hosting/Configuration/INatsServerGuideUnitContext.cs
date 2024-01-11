using NATS.Client.Core;
using TnyFramework.Net.Nats.Hosting.Configuration.spec;
using TnyFramework.Net.Nats.Options;

namespace TnyFramework.Net.Nats.Hosting.Configuration;

public interface INatsServerGuideUnitContext : INatsGuideUnitContext
{
    NatsOpts LoadNatsOpts();

    INatsServerSetting LoadNatsServerSetting();

    // IRpcClientFactory LoadRpcClientFactory();
}