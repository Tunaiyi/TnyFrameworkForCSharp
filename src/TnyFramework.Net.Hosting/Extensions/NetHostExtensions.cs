// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TnyFramework.DI.Extensions;
using TnyFramework.Extensions.Configuration.Configuration;
using TnyFramework.Hosting.Extensions;
using TnyFramework.Net.Application;
using TnyFramework.Net.Extensions;
using TnyFramework.Net.Hosting.Host;
using TnyFramework.Net.Hosting.Options;
using TnyFramework.Net.Hosting.Rpc;
using TnyFramework.Net.Rpc.Client;
using TnyFramework.Net.Rpc.Configuration;

namespace TnyFramework.Net.Hosting.Extensions;

public static class NetHostExtensions
{
    public static IHostBuilder UseNetHost(this IHostBuilder builder, string[] assembles)
    {
        return builder
            .UseAppHost(assembles)
            .ConfigureServices((hostBuilder, services) => {
                var configuration = hostBuilder.Configuration;
                var appOptions = configuration.BindOptions<NetApplicationOptions>(NetApplicationOptions.APP_ROOT_PATH);
                services
                    .BindSingleton<INetApplicationOptions>(appOptions)
                    .AddHostedService<NetHostedService>()
                    .AddSingleton(p => p);
            })
            .UseRpcService();
    }

    private static IHostBuilder UseRpcService(this IHostBuilder builder)
    {
        return builder
            .ConfigureServices((hostBuilder, services) => {
                var configuration = hostBuilder.Configuration;
                var clientServiceOptions =
                    configuration.BindOptionsIfExists<RpcClientOptions>(RpcClientOptions.RPC_CLIENT_ROOT_PATH);
                if (clientServiceOptions == null)
                {
                    return;
                }
                services.AddSingleton<IRpcClientOptions>(clientServiceOptions);
                foreach (var setting in clientServiceOptions.Services)
                {
                    var beanName = setting.ServiceName() + nameof(IRpcClientFactory)[1..];
                    services.AddSingletonUnit<IRpcClientFactory>(beanName, provider =>
                        new RpcClientFactory(setting, provider.GetRequiredService<INetApplicationOptions>(),
                            string.IsNullOrEmpty(setting.Guide)
                                ? provider.GetRequiredService<IClientGuide>()
                                : provider.GetRequiredKeyedService<IClientGuide>(setting.Guide)
                        ));
                }
                var serverConfiguration = RpcRemoteServiceConfiguration
                    .CreateRpcRemoteService(services)
                    .AddRemoteServices();
                serverConfiguration.Initialize();
            });
    }

}
