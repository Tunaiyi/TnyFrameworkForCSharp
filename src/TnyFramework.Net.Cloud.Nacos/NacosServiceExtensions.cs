using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nacos.AspNetCore.V2;
using Nacos.Microsoft.Extensions.Configuration;
using Nacos.V2.DependencyInjection;
using TnyFramework.DI.Container;

namespace TnyFramework.Net.Cloud.Nacos
{

    public static class NacosServiceExtensions
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

        public static IHostBuilder UseNacosConfiguration(this IHostBuilder builder, string section = "nacos",
            Action<NacosV2ConfigurationSource> action = null, Action<ILoggingBuilder> logAction = null)
        {
            return builder.ConfigureAppConfiguration((context, configBuilder) => {
                var root = configBuilder.Build();
                var nacos = root.GetSection(section);
                if (nacos != null)
                {
                    configBuilder.AddNacosV2Configuration(source => {
                        nacos.Bind(source);
                        if (logAction != null)
                        {
                            source.LoggingBuilder = logAction;
                        }
                        source.NacosConfigurationParser = JsonConfigurationStringParser.instance;
                        action?.Invoke(source);
                    });
                }
            });
        }

        public static IHostBuilder UseNacos(this IHostBuilder builder, string section = "nacos",
            Action<NacosV2ConfigurationSource> action = null, Action<ILoggingBuilder> logAction = null)
        {
            return builder
                .UseNacosConfiguration(section, action, logAction)
                .UseNacosServerDiscovery(section);
        }
    }

}
