using TnyFramework.Net.Hosting.Rpc;
using TnyFramework.Net.Nats.Hosting.Configuration.spec;
using TnyFramework.Net.Nats.Options;
using TnyFramework.Net.Nats.Transports;

namespace TnyFramework.Net.Nats.Hosting.Configuration;

public interface INatsRpcHostServerConfiguration
    : IRpcHostConfiguration<INatsServerGuide, INatsServerSetting, INatsServerGuideUnitContext,
        INatsRpcHostServerConfiguration, INatsServerGuideSpec>
{
}