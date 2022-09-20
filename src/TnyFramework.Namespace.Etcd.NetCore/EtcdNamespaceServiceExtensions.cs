// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TnyFramework.Codec;
using TnyFramework.Common.Extensions;
using TnyFramework.DI.Extensions;

namespace TnyFramework.Namespace.Etcd.NetCore
{

    public static class EtcdNamespaceServiceExtensions
    {
        private static readonly string DEFAULT_SECTION = ConfigurationPath.Combine("Namespace", "Etcd");

        public static IHostBuilder UseEtcdNamespace(this IHostBuilder builder, string section = null)
        {
            if (section.IsBlank())
            {
                section = DEFAULT_SECTION;
            }
            return builder.ConfigureServices((hostBuilder, services) => {
                var configuration = hostBuilder.Configuration;
                var config = new EtcdConfig();
                configuration.GetSection(section).Bind(config);
                services.AddSingleton(_ => config);
                services.AddSingleton<ObjectCodecAdapter>();
                services.BindSingleton<EtcdNamespaceExplorerFactory>();
                services.BindSingleton((p) => p.GetService<EtcdNamespaceExplorerFactory>()?.Create());
            });

        }
    }

}
