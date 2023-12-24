// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TnyFramework.DI.Extensions;
using TnyFramework.Extensions.Configuration.Configuration;
using TnyFramework.Hosting.Extensions;
using TnyFramework.Net.Application;
using TnyFramework.Net.Extensions;
using TnyFramework.Net.Hosting.Host;
using TnyFramework.Net.Hosting.Options;
using TnyFramework.Net.Rpc.Client;
using TnyFramework.Net.Rpc.Configuration;

namespace TnyFramework.Net.Hosting.Extensions;

public static class TnyNetHostExtensions
{
    public static IHostBuilder UseTnyNetHost(this IHostBuilder builder, string[] assembles)
    {
        return builder
            .UseTnyHost(assembles)
            .ConfigureServices((hostBuilder, services) => {
                var configuration = hostBuilder.Configuration;
                var appOptions = configuration.BindOptions<NetApplicationOptions>(NetApplicationOptions.APP_ROOT_PATH);
                services
                    .BindSingleton<INetApplicationOptions>(appOptions)
                    .AddHostedService<NetHostedService>()
                    .AddSingleton(p => p);
            })
            .UseRpcClientService();
    }

    private static IHostBuilder UseRpcClientService(this IHostBuilder builder)
    {
        return builder
            .ConfigureServices((hostBuilder, services) => {
                var configuration = hostBuilder.Configuration;
                var rpcClientOption =
                    configuration.BindOptionsIfExists<RpcClientOptions>(RpcClientOptions.RPC_CLIENT_ROOT_PATH);
                if (rpcClientOption == null)
                {
                    return;
                }
                services.AddSingleton<IRpcClientSetting>(rpcClientOption);
                foreach (var setting in rpcClientOption.Services)
                {
                    var beanName = setting.ServiceName() + nameof(IRpcClientSetting)[1..];
                    services.AddSingletonUnit(beanName, provider =>
                        new RpcClientFactory(setting, provider.GetRequiredService<INetApplicationOptions>(),
                            string.IsNullOrEmpty(setting.Guide)
                                ? provider.GetRequiredService<INetClientGuide>()
                                : provider.GetRequiredKeyedService<INetClientGuide>(setting.Guide)
                        ));
                }
            });
    }
}
