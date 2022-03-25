#region

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nacos.AspNetCore.V2;
using Nacos.V2.DependencyInjection;
using TnyFramework.DI.Container;

#endregion

namespace TnyFramework.Net.Cloud.Nacos
{

    public static class GenericHostNacosServerServiceExtensions
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


        public static IHostBuilder UseNacosConfiguration(this IHostBuilder builder, string section = "nacos")
        {
            return builder.ConfigureAppConfiguration((context, configBuilder) => {
                var root = configBuilder.Build();
                configBuilder.AddNacosV2Configuration(root.GetSection(section));
            });
        }


        public static IHostBuilder UseNacos(this IHostBuilder builder, string section = "nacos")
        {
            return builder
                .UseNacosServerDiscovery(section)
                .UseNacosConfiguration(section);
        }
    }

}
