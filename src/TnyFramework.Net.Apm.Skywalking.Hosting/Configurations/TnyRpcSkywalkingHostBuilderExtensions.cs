// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using TnyFramework.DI.Extensions;
using TnyFramework.Net.Apm.Skywalking.Hosting.Handler;
using TnyFramework.Net.Apm.Skywalking.Hosting.Setting;

namespace TnyFramework.Net.Apm.Skywalking.Hosting.Configurations;

public static class RpcHostNetSkywalkingBuilderExtensions
{
    private static readonly string ROOT_PATH = ConfigurationPath.Combine("Tny", "Apm", "Skywalking");

    /// <summary>
    /// 为 HostBuilder 配置 TnyRpcSkyApm
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static IHostBuilder ConfigureTnyRpcSkyApm(this IHostBuilder builder)
    {
        return builder.ConfigureServices((hostBuilder, services) => {
            var configuration = hostBuilder.Configuration;
            var properties = new SkywalkingRpcMonitorProperties();
            configuration.Bind(ROOT_PATH, properties);
            if (!properties.Enable)
                return;
            services.BindSingleton(_ => properties)
                .BindSingleton<SkywalkingRpcMonitorHandler>()
                .AddTnyRpcSkyApm();
        });
    }
}
