using TnyFramework.Net.Hosting.Guide;
using TnyFramework.Net.Nats.Transports;

namespace TnyFramework.Net.Nats.Hosting.Configuration.spec;

public interface INatsGuideUnitContext : INetGuideUnitContext
{
    IMessageCodec LoadMessageCodec();
}