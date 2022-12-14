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
using TnyFramework.DI.Container;
using TnyFramework.Net.Base;
using TnyFramework.Net.DotNetty.Configuration;
using TnyFramework.Net.DotNetty.Configuration.Guide;
using TnyFramework.Net.Rpc;

namespace TnyFramework.Net.DotNetty.NetCore
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
            Action<INetServerGuideSpec<RpcAccessIdentify>> guideConfigure)
        {
            return builder.ConfigureRpcHost(guideConfigure, null);
        }

        public static IHostBuilder UseNoopServerDiscovery<TUserId>(this IHostBuilder builder)
        {
            return builder.ConfigureServices((_, services) => { services.AddSingletonUnit<NoopServerDiscoveryService>(); });
        }

        public static IHostBuilder ConfigureRpcHost(this IHostBuilder builder,
            Action<INetServerGuideSpec<RpcAccessIdentify>> serverGuideSpec,
            Action<INettyServerConfiguration> configure)
        {
            builder.ConfigureServices((hostBuilder, services) => {
                var configuration = hostBuilder.Configuration;
                var options = new NetApplicationHostOptions();
                configuration.Bind(NetApplicationHostOptions.ROOT_PATH, options);
                if (options.RpcServer == null)
                    throw new IllegalArgumentException("NetApplicationHostOptions RpcServer ?????????");
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
