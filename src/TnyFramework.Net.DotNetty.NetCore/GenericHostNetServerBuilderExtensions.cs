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
using TnyFramework.Net.DotNetty.Configuration;
using TnyFramework.Net.DotNetty.Configuration.Guide;

namespace TnyFramework.Net.DotNetty.NetCore
{

    public static class GenericHostNetServerBuilderExtensions
    {
        public static IHostBuilder ConfigureNetHost<TUserId>(this IHostBuilder builder)
        {
            return builder.ConfigureNetHost<TUserId>(null, null);
        }

        public static IHostBuilder ConfigureNetHost<TUserId>(this IHostBuilder builder,
            Action<INettyServerConfiguration> configure)
        {
            return builder.ConfigureNetHost<TUserId>(null, configure);
        }

        public static IHostBuilder ConfigureNetHost<TUserId>(this IHostBuilder builder,
            Action<INetServerGuideSpec<TUserId>> guideConfigure)
        {
            return builder.ConfigureNetHost(guideConfigure, null);
        }

        public static IHostBuilder ConfigureNetHost<TUserId>(this IHostBuilder builder,
            Action<INetServerGuideSpec<TUserId>> serverGuideSpec, Action<INettyServerConfiguration> configure)
        {
            return builder.ConfigureServices((hostBuilder, services) => {
                var configuration = hostBuilder.Configuration;
                var options = new NetApplicationHostOptions();
                configuration.Bind(NetApplicationHostOptions.ROOT_PATH, options);
                if (options.Server == null)
                    throw new IllegalArgumentException("NetApplicationHostOptions Server 未配置");
                var serverSetting = options.Server;
                var serverConfiguration = NettyServerConfiguration.CreateNetServer(services)
                    .AppContextConfigure(spec => {
                        spec.ServerId(options.ServerId)
                            .AppName(options.Name)
                            .AppType(options.AppType)
                            .ScopeType(options.ScopeType)
                            .Locale(options.Locale);
                    })
                    .Server<TUserId>(serverSetting.Name, spec => {
                        spec.Server(serverSetting);
                        serverGuideSpec?.Invoke(spec);
                    })
                    .Initialize();
                services.AddSingleton(p => p);
                services.AddHostedService<NetHostedService>();
                configure?.Invoke(serverConfiguration);
            });
        }
    }

}
