using Microsoft.Extensions.Configuration;
using TnyFramework.Net.Nats.Options;

namespace TnyFramework.Net.Nats.Hosting.Configuration;

public class NatsAppHostOptions
{
    public static readonly string NATS_ROOT_PATH = ConfigurationPath.Combine("Tny", "Net", "Nats");

    public NatsServerSetting? RpcServer { get; set; } = null!;
}