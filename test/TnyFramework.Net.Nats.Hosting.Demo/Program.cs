using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TnyFramework.DI.Extensions;
using TnyFramework.Extensions.Configuration;
using TnyFramework.Extensions.Configuration.Yaml;
using TnyFramework.Net.Application;
using TnyFramework.Net.Hosting.Extensions;
using TnyFramework.Net.Nats.Hosting.Extensions;
using TnyFramework.NLog.Hosting.Extensions;

namespace TnyFramework.Net.Nats.Hosting.Demo;

internal class Program
{
    public static async Task Main(string[] args)
    {
        // 启动服务器
        var host = Host.CreateDefaultBuilder(args)
            .UseNLogForApplication(builder => builder
                .AddYamlFileWithEnvironment("appsettings")
                .AddYamlFileWithEnvironment("nlog")
            )
            .UseNetHost(new[] {"TnyFramework", "ProjV"})
            .ConfigureServices(s => s.BindSingleton<INetServerDiscoveryService, NoopServerDiscoveryService>())
            .ConfigureServices(s => {
                var env = TnyEnvironments.GetEnvironment();
                if (env.Contains("Client"))
                {
                    s.AddHostedService<TestHostService>();
                }
            })
            .ConfigureNatsRpcHost()
            .Build();
        await host.RunAsync();
    }
}