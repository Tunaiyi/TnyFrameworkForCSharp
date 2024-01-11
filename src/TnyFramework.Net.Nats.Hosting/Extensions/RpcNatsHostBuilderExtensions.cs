using System;
using Microsoft.Extensions.Hosting;
using TnyFramework.Common.Exceptions;
using TnyFramework.DI.Extensions;
using TnyFramework.Extensions.Configuration.Configuration;
using TnyFramework.Net.Application;
using TnyFramework.Net.Hosting.Rpc;
using TnyFramework.Net.Nats.Hosting.Configuration;
using TnyFramework.Net.Nats.Hosting.Configuration.spec;

namespace TnyFramework.Net.Nats.Hosting.Extensions;

public static class RpcNatsHostBuilderExtensions
{
    public static IHostBuilder ConfigureNatsRpcHost(this IHostBuilder builder)
    {
        return builder.ConfigureRpcHost(null, null);
    }

    public static IHostBuilder ConfigureNatsRpcHost(this IHostBuilder builder,
        Action<INatsRpcHostServerConfiguration> configure)
    {
        return builder.ConfigureRpcHost(null, configure);
    }

    public static IHostBuilder ConfigureNatsRpcHost(this IHostBuilder builder,
        Action<INatsServerGuideSpec> guideConfigure)
    {
        return builder.ConfigureRpcHost(guideConfigure, null);
    }

    public static IHostBuilder UseNoopServerDiscovery(this IHostBuilder builder)
    {
        return builder.ConfigureServices((_, services) => {
            services.AddSingletonUnit<NoopServerDiscoveryService>();
        });
    }

    public static IHostBuilder ConfigureRpcHost(this IHostBuilder builder,
        Action<INatsServerGuideSpec>? serverGuideSpec,
        Action<INatsRpcHostServerConfiguration>? configure)
    {
        builder.ConfigureServices((hostBuilder, services) => {
            var configuration = hostBuilder.Configuration;
            var appOptions = configuration.BindOptions<NetApplicationOptions>(NetApplicationOptions.APP_ROOT_PATH);
            var natsHostOptions = configuration.BindOptions<NatsAppHostOptions>(NatsAppHostOptions.NATS_ROOT_PATH);
            if (natsHostOptions.RpcServer == null)
                throw new IllegalArgumentException(
                    $"NettyAppHostOptions RpcServer 未配置, 配置路径 {NatsAppHostOptions.NATS_ROOT_PATH}:{nameof(natsHostOptions.RpcServer)}");
            var natsSetting = natsHostOptions.RpcServer;
            if (natsSetting == null)
                throw new IllegalArgumentException("NetApplicationHostOptions RpcServer 未配置");
            var serverConfiguration = NatsRpcHostServerConfiguration.CreateRpcServer(services);
            serverConfiguration
                .RpcServer(natsSetting, spec => {
                    spec.ConfigureNats(natsOpts => { configuration.BindOptions(natsOpts, "Nats", false); });
                    serverGuideSpec?.Invoke(spec);
                })
                .AppContextConfigure(spec => {
                    spec.ServerId(appOptions.ServerId)
                        .AppName(appOptions.Name)
                        .AppType(appOptions.AppType)
                        .ScopeType(appOptions.ScopeType)
                        .Locale(appOptions.Locale);
                })
                .AddControllers()
                .Initialize();
            configure?.Invoke(serverConfiguration);
        });
        return builder;
    }

    public static IHostBuilder ConfigureRpcRemote(this IHostBuilder builder,
        Action<IRpcRemoteServiceConfiguration>? configure = null)
    {
        builder.ConfigureServices((_, services) => {
            var serverConfiguration = RpcRemoteServiceConfiguration.CreateRpcRemoteService(services)
                .AddRemoteServices();
            configure?.Invoke(serverConfiguration);
            serverConfiguration.Initialize();
        });
        return builder;
    }
}