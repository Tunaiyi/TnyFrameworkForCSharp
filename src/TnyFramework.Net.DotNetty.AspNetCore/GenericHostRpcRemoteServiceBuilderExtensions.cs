using System;
using Microsoft.Extensions.Hosting;
using TnyFramework.Net.DotNetty.Configuration;

namespace TnyFramework.Net.DotNetty.AspNetCore
{

    public static class GenericHostRpcRemoteServiceBuilderExtensions
    {
        public static IHostBuilder ConfigureRpcRemote(this IHostBuilder builder, Action<IRpcRemoteServiceConfiguration> configure = null)
        {
            builder.ConfigureServices((hostBuilder, services) => {
                var serverConfiguration = RpcRemoteServiceConfiguration.CreateRpcRemoteService(services)
                    .AddRemoteServices();
                configure?.Invoke(serverConfiguration);
                serverConfiguration.Initialize();
            });
            return builder;
        }
    }

}
