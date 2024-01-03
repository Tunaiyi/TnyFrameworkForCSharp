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
using TnyFramework.Common.Scanner;
using TnyFramework.Common.Scanner.Assemblies;
using TnyFramework.DI.Extensions;
using TnyFramework.DI.Hosting.Extensions;
using TnyFramework.Extensions.Configuration;
using TnyFramework.Extensions.Configuration.Configuration;

namespace TnyFramework.Hosting.Extensions;

public static class AppHostExtensions
{
    public static IHostBuilder UseAppHost(this IHostBuilder builder, string[] assembles)
    {
        return builder
            .ConfigureHostConfiguration(CreateAppConfiguration(assembles)) // 构建配置
            .UseDiContainerHost()
            .ConfigureServices(RegisterServices);
    }

    private static Action<IConfigurationBuilder> CreateAppConfiguration(params string[] loadAssemblies)
    {
        return builder => {
            AssemblyUtils.LoadAllAssemblies(loadAssemblies);
            TypeScanner.Instance().Scan().GetAwaiter().GetResult();
            var root = builder.Build();
            TnyEnvironments.InitAppEnvironment(root.AsReadOnlyDictionary());
        };
    }

    private static void RegisterServices(HostBuilderContext hostBuilder, IServiceCollection services)
    {
        services.AddComponents();
    }
}
