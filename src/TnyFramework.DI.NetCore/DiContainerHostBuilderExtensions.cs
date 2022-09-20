using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace TnyFramework.DI.NetCore
{

    public static class DiContainerHostBuilderExtensions
    {
        public static IHostBuilder ConfigureDiHost(this IHostBuilder builder)
        {
            return builder.ConfigureServices((hostBuilder, services) => { services.AddHostedService<DiContainerHostedService>(); });
        }
    }

}
