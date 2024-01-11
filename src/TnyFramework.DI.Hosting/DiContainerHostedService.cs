// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using TnyFramework.DI.Attributes;
using TnyFramework.DI.Extensions;

namespace TnyFramework.DI.Hosting;

public class DiContainerHostedService : IHostedService
{
    private readonly IServiceProvider provider;

    public DiContainerHostedService(IServiceProvider provider)
    {
        this.provider = provider;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        foreach (var type in ComponentTypeSelector.Types)
        {
            var component = type.GetCustomAttribute<ComponentAttribute>();
            if (component == null || component.Lazy)
            {
                continue;
            }
            provider.GetService(type);
        }
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
