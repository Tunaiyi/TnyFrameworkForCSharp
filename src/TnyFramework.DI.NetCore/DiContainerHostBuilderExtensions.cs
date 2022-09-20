// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace TnyFramework.DI.NetCore
{

    public static class DiContainerHostBuilderExtensions
    {
        public static IHostBuilder ConfigureDiHost(this IHostBuilder builder)
        {
            return builder.ConfigureServices((hostBuilder, services) => { services.AddHostedService<DiContainerHostedService>(); });
        }
    }

}
