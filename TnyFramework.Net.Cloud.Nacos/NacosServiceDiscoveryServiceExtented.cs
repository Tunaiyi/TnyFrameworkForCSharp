using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nacos.AspNetCore.V2;
using Nacos.V2.DependencyInjection;
using TnyFramework.DI.Container;
namespace TnyFramework.Net.Cloud
{
    public static class GenericHostNacosServerDiscoveryServiceExtensions
    {
        public static IHostBuilder UseNacosServerDiscovery(this IHostBuilder builder, string section = "nacos")
        {
            return builder.ConfigureServices((hostBuilder, services) => {
                var configuration = hostBuilder.Configuration;
                services.Configure<NacosAspNetOptions>(configuration.GetSection(section));
                services.AddNacosV2Naming(configuration, sectionName: section);
                services.AddSingletonUnit<NacosServerDiscoveryService>();
            });
        }
    }
}
