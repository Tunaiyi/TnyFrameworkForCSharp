using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TnyFramework.Common.Exception;
using TnyFramework.DI.Container;
using TnyFramework.Net.Base;
using TnyFramework.Net.DotNetty.Configuration;
using TnyFramework.Net.DotNetty.Configuration.Guide;
using TnyFramework.Net.Rpc.Auth;
namespace TnyFramework.Net.DotNetty.AspNetCore
{
    public static class GenericHostRpcServerBuilderExtensions
    {
        public static IHostBuilder ConfigureRpcHost(this IHostBuilder builder)
        {
            return builder.ConfigureRpcHost(null, null);
        }


        public static IHostBuilder ConfigureRpcHost(this IHostBuilder builder,
            Action<INettyServerConfiguration> configure)
        {
            return builder.ConfigureRpcHost(null, configure);
        }


        public static IHostBuilder ConfigureRpcHost(this IHostBuilder builder,
            Action<INetServerGuideSpec<IRpcLinkerId>> guideConfigure)
        {
            return builder.ConfigureRpcHost(guideConfigure, null);
        }


        public static IHostBuilder UseNoopServerDiscovery<TUserId>(this IHostBuilder builder)
        {
            return builder.ConfigureServices((_, services) => { services.AddSingletonUnit<NoopServerDiscoveryService>(); });
        }


        public static IHostBuilder ConfigureRpcHost(this IHostBuilder builder,
            Action<INetServerGuideSpec<IRpcLinkerId>> serverGuideSpec, Action<INettyServerConfiguration> configure)
        {
            builder.ConfigureServices((hostBuilder, services) => {
                var configuration = hostBuilder.Configuration;
                var options = new NetApplicationHostOptions();
                configuration.Bind(NetApplicationHostOptions.ROOT_PATH, options);
                if (options.RpcServer == null)
                    throw new IllegalArgumentException("NetApplicationHostOptions RpcServer 未配置");
                var serverSetting = options.RpcServer;
                var serverConfiguration = RpcServerConfiguration.CreateRpcServer(services);
                serverConfiguration
                    .RpcServer(serverSetting, serverGuideSpec)
                    .AppContextConfigure(spec => {
                        spec.ServerId(options.ServerId)
                            .AppName(options.Name)
                            .AppType(options.AppType)
                            .ScopeType(options.ScopeType)
                            .Locale(options.Locale);
                    })
                    .Initialize();
                services.AddHostedService<NetHostedService>();
                configure?.Invoke(serverConfiguration);
            });
            return builder;
        }
    }
}
