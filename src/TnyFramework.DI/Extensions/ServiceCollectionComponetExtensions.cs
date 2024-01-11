// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TnyFramework.Common.Logger;
using TnyFramework.DI.Attributes;

namespace TnyFramework.DI.Extensions;

public static class ServiceCollectionComponentExtensions
{
    private static ILogger? _LOGGER;

    private static ILogger Logger => _LOGGER ??= LogFactory.Logger(typeof(ServiceCollectionComponentExtensions));

    public static IServiceCollection AddComponents(this IServiceCollection service)
    {
        foreach (var type in ComponentTypeSelector.Types)
        {
            AddComponent(service, type);
        }
        return service;
    }

    public static IServiceCollection AddComponents(this IServiceCollection service, IEnumerable<Assembly> assemblies)
    {
        return service.DoAddComponents(assemblies);
    }

    private static void AddComponent(IServiceCollection service, Type type)
    {
        var componentAttribute = type.GetCustomAttribute<ComponentAttribute>() ?? type.GetCustomAttribute<ServiceAttribute>();
        if (componentAttribute == null)
            return;
        Logger.LogInformation("Add Component : {Type}", type);
        var name = componentAttribute.Named() ? "" : componentAttribute.Name;
        switch (componentAttribute.Mode)
        {
            case DIMode.Singleton:
                service.AddSingletonUnit(name, type);
                break;
            case DIMode.Scope:
                service.AddScopedUnit(name, type);
                break;
            case DIMode.Transient:
                service.AddTransientUnit(name, type);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private static IServiceCollection DoAddComponents(this IServiceCollection service, IEnumerable<Assembly> assemblies)
    {
        foreach (var assembly in assemblies)
        {
            foreach (var type in assembly.GetTypes())
            {
                AddComponent(service, type);
            }
        }
        return service;
    }
}
