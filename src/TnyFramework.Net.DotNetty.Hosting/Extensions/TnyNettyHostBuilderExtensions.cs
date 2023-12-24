// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TnyFramework.Common.Exceptions;
using TnyFramework.Common.Extensions;
using TnyFramework.DI.Extensions;
using TnyFramework.Extensions.Configuration.Configuration;
using TnyFramework.Net.Application;
using TnyFramework.Net.DotNetty.Hosting.Configuration;
using TnyFramework.Net.DotNetty.Hosting.Guide;
using TnyFramework.Net.DotNetty.Hosting.Options;
using TnyFramework.Net.Hosting.App;
using TnyFramework.Net.Hosting.Host;
using TnyFramework.Net.Hosting.Rpc;

namespace TnyFramework.Net.DotNetty.Hosting.Extensions
{

    public static class TnyNettyHostBuilderExtensions
    {
        public static IHostBuilder ConfigureNetHost(this IHostBuilder builder)
        {
            return builder.ConfigureNetHost(null, null);
        }

        public static IHostBuilder ConfigureNetHost(this IHostBuilder builder,
            Action<INettyNetHostServerConfiguration> configure)
        {
            return builder.ConfigureNetHost(null, configure);
        }

        public static IHostBuilder ConfigureNetHost(this IHostBuilder builder,
            Action<INettyServerGuideSpec> guideConfigure)
        {
            return builder.ConfigureNetHost(guideConfigure, null);
        }

        public static IHostBuilder ConfigureRpcHost(this IHostBuilder builder)
        {
            return builder.ConfigureRpcHost(null, null);
        }

        public static IHostBuilder ConfigureRpcHost(this IHostBuilder builder,
            Action<INettyRpcHostServerConfiguration> configure)
        {
            return builder.ConfigureRpcHost(null, configure);
        }

        public static IHostBuilder ConfigureRpcHost(this IHostBuilder builder,
            Action<INettyServerGuideSpec> guideConfigure)
        {
            return builder.ConfigureRpcHost(guideConfigure, null);
        }

        public static IHostBuilder UseNoopServerDiscovery(this IHostBuilder builder)
        {
            return builder.ConfigureServices((_, services) => { services.AddSingletonUnit<NoopServerDiscoveryService>(); });
        }


        public static IHostBuilder ConfigureRpcHost(this IHostBuilder builder,
            Action<INettyServerGuideSpec>? serverGuideSpec,
            Action<INettyRpcHostServerConfiguration>? configure)
        {
            builder.ConfigureServices((hostBuilder, services) => {
                var configuration = hostBuilder.Configuration;
                var appOptions = configuration.BindOptions<NetApplicationOptions>(NetApplicationOptions.APP_ROOT_PATH);
                var nettyOptions = configuration.BindOptions<NettyAppHostOptions>(NettyAppHostOptions.NETTY_ROOT_PATH);
                if (nettyOptions.RpcServer.IsNull())
                    throw new IllegalArgumentException($"NettyAppHostOptions RpcServer 未配置, 配置路径 {NettyAppHostOptions.NETTY_ROOT_PATH}:{nameof(nettyOptions.RpcServer)}");
                var serverSetting = nettyOptions.RpcServer;
                var serverConfiguration = NettyRpcHostServerConfiguration.CreateRpcServer(services);
                serverConfiguration
                    .RpcServer(serverSetting, serverGuideSpec)
                    .AppContextConfigure(spec => {
                        spec.ServerId(appOptions.ServerId)
                            .AppName(appOptions.Name)
                            .AppType(appOptions.AppType)
                            .ScopeType(appOptions.ScopeType)
                            .Locale(appOptions.Locale);
                    })
                    .Initialize();
                services.AddHostedService<NetHostedService>();
                configure?.Invoke(serverConfiguration);
            });
            return builder;
        }

        public static IHostBuilder ConfigureNetHost(this IHostBuilder builder,
            Action<INettyServerGuideSpec>? serverGuideSpec, Action<INettyNetHostServerConfiguration>? configure)
        {
            return builder.ConfigureServices((hostBuilder, services) => {
                var configuration = hostBuilder.Configuration;
                var appOptions = configuration.BindOptions<NetApplicationOptions>(NetApplicationOptions.APP_ROOT_PATH);
                var nettyOptions = configuration.BindOptions<NettyAppHostOptions>(NettyAppHostOptions.NETTY_ROOT_PATH);
                if (nettyOptions.NetServer.IsNull())
                    throw new IllegalArgumentException($"NettyAppHostOptions NetServer 未配置, 配置路径 {NettyAppHostOptions.NETTY_ROOT_PATH}:{nameof(nettyOptions.NetServer)}");
                var serverSetting = nettyOptions.NetServer;
                var serverConfiguration = NettyNetHostServerConfiguration.CreateNetServer(services)
                    .Server(serverSetting.Name, spec => {
                        spec.Server(serverSetting);
                        serverGuideSpec?.Invoke(spec);
                    })
                    .AppContextConfigure(spec => {
                        spec.ServerId(appOptions.ServerId)
                            .AppName(appOptions.Name)
                            .AppType(appOptions.AppType)
                            .ScopeType(appOptions.ScopeType)
                            .Locale(appOptions.Locale);
                    })
                    .Initialize();
                services.AddSingleton(p => p);
                services.AddHostedService<NetHostedService>();
                configure?.Invoke(serverConfiguration);
            });
        }

        public static IHostBuilder ConfigureRpcRemote(this IHostBuilder builder, Action<IRpcRemoteServiceConfiguration>? configure = null)
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

}
