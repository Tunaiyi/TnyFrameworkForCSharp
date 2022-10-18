// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using Microsoft.Extensions.Hosting;
using TnyFramework.Net.DotNetty.Configuration;

namespace TnyFramework.Net.DotNetty.NetCore.Extensions
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
