using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TnyFramework.Codec;
using TnyFramework.Common.Extensions;
using TnyFramework.DI.Extensions;

namespace TnyFramework.Namespace.Etcd.NetCore
{

    public static class EtcdNamespaceServiceExtensions
    {
        private static readonly string DEFAULT_SECTION = ConfigurationPath.Combine("Namespace", "Etcd");

        public static IHostBuilder UseEtcdNamespace(this IHostBuilder builder, string section = null)
        {
            if (section.IsBlank())
            {
                section = DEFAULT_SECTION;
            }
            return builder.ConfigureServices((hostBuilder, services) => {
                var configuration = hostBuilder.Configuration;
                var config = new EtcdConfig();
                configuration.GetSection(section).Bind(config);
                services.AddSingleton(_ => config);
                services.AddSingleton<ObjectCodecAdapter>();
                services.BindSingleton<EtcdNamespaceExplorerFactory>();
                services.BindSingleton((p) => p.GetService<EtcdNamespaceExplorerFactory>()?.Create());
            });

        }
    }

}
