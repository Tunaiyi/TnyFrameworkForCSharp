// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TnyFramework.DI.Extensions;

namespace TnyFramework.DI.NetCore
{

    public static class BindPropertiesBuilderExtensions
    {
        public static IServiceCollection BindProperties<TProperties>(this IServiceCollection services, IConfiguration configuration,
            string propertiesPath)
            where TProperties : new()
        {
            var properties = new TProperties();
            configuration.Bind(propertiesPath, properties);
            services.BindSingleton(properties);
            return services;
        }

        public static IHostBuilder BindProperties<TProperties>(this IHostBuilder builder, string propertiesPath) where TProperties : new()
        {
            return builder.ConfigureServices((hostBuilder, services) => {
                services.BindProperties<TProperties>(hostBuilder.Configuration, propertiesPath);
            });
        }

    }

}
