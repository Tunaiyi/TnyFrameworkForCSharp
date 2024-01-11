// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TnyFramework.DI.Container;
using TnyFramework.DI.Units;

namespace TnyFramework.DI.Extensions;

public static class ServiceCollectionAddTransientUnitExtensions
{
    public static IServiceCollection AddTransientUnit(this IServiceCollection services, Type instanceType)
    {
        return services.AddTransientUnit(string.Empty, instanceType);
    }

    public static IServiceCollection AddTransientUnit(this IServiceCollection services, string name, Type instanceType)
    {
        var serviceInstance = new TransientServiceInstance(new TypeServiceFactory(instanceType));
        return services.RegisterTransientUnit(name, serviceInstance, instanceType);
    }

    public static IServiceCollection AddTransientUnit(this IServiceCollection services, Type serviceType,
        Func<IServiceProvider, object> instanceFactory)
    {
        return services.AddTransientUnit(string.Empty, serviceType, instanceFactory);
    }

    public static IServiceCollection AddTransientUnit(this IServiceCollection services, string name, Type serviceType,
        Func<IServiceProvider, object> instanceFactory)
    {
        var serviceInstance = new TransientServiceInstance(new FuncServiceFactory<object>(instanceFactory));
        services.RegisterTransientUnit(name, serviceInstance, serviceType);
        return services;
    }

    public static IServiceCollection AddTransientUnit(this IServiceCollection services, Type serviceType, Type instanceType)
    {
        return services.AddTransientUnit(string.Empty, serviceType, instanceType);
    }

    public static IServiceCollection AddTransientUnit(this IServiceCollection services, string name, Type serviceType, Type instanceType)
    {
        var serviceInstance = new TransientServiceInstance(new TypeServiceFactory(instanceType));
        services.RegisterTransientUnit(name, serviceInstance, serviceType);
        return services;
    }

    public static IServiceCollection AddTransientUnit<TInstance>(this IServiceCollection services)
        where TInstance : class
    {
        return services.AddTransientUnit<TInstance>(string.Empty);
    }

    public static IServiceCollection AddTransientUnit<TInstance>(this IServiceCollection services, string name)
        where TInstance : class
    {
        var serviceType = typeof(TInstance);
        var serviceInstance = new TransientServiceInstance(new TypeServiceFactory(serviceType));
        services.RegisterTransientUnit(name, serviceInstance, serviceType);
        return services;
    }

    public static IServiceCollection AddTransientUnit<TService>(this IServiceCollection services,
        Func<IServiceProvider, TService> instanceFactory)
        where TService : class
    {
        return services.AddTransientUnit(string.Empty, instanceFactory);
    }

    public static IServiceCollection AddTransientUnit<TService>(this IServiceCollection services, string name,
        Func<IServiceProvider, TService> instanceFactory)
        where TService : class
    {
        var serviceType = typeof(TService);
        var serviceInstance = new TransientServiceInstance(new FuncServiceFactory<TService>(instanceFactory));
        return services.RegisterTransientUnit(name, serviceInstance, serviceType);
    }

    public static IServiceCollection AddTransientUnit<TService, TImplementation>(this IServiceCollection services)
        where TService : class
        where TImplementation : class, TService
    {
        return services.AddTransientUnit<TService, TImplementation>(string.Empty);
    }

    public static IServiceCollection AddTransientUnit<TService, TImplementation>(this IServiceCollection services, string name)
        where TService : class
        where TImplementation : class, TService
    {
        var serviceType = typeof(TService);
        var instanceType = typeof(TImplementation);
        var serviceInstance = new TransientServiceInstance(new TypeServiceFactory(instanceType));
        return services.RegisterTransientUnit(name, serviceInstance, serviceType);
    }

    public static IServiceCollection AddTransientUnit<TService, TImplementation>(this IServiceCollection services,
        Func<IServiceProvider, TImplementation> instanceFactory)
        where TService : class
        where TImplementation : class, TService
    {
        return services.AddTransientUnit<TService, TImplementation>(string.Empty, instanceFactory);
    }

    public static IServiceCollection AddTransientUnit<TService, TImplementation>(this IServiceCollection services, string name,
        Func<IServiceProvider, TImplementation> instanceFactory)
        where TService : class
        where TImplementation : class, TService
    {
        var serviceType = typeof(TService);
        var serviceInstance = new TransientServiceInstance(new FuncServiceFactory<TImplementation>(instanceFactory));
        return services.RegisterTransientUnit(name, serviceInstance, serviceType);
    }

    private static IServiceCollection RegisterTransientUnit(this IServiceCollection services, string name,
        IServiceInstance instance, Type instanceType)
    {
        var types = ServiceCollectionUtils.FindRegisterTypes(instanceType);
        return services.AddTransientUnits(name, instance, types);
    }

    private static IServiceCollection TryTransientUnits(this IServiceCollection services)
    {
        services.TryAdd(ServiceDescriptor.Transient(typeof(IUnitCollection<>), typeof(UnitCollection<>)));
        return services;
    }

    private static IServiceCollection AddTransientUnits(this IServiceCollection services, string name,
        IServiceInstance instance, IEnumerable<Type> types)
    {
        services.TryTransientUnits();
        foreach (var type in types)
        {
            services.AddTransientServiceInstance(instance, type);
            services.AddTransient(Unit.UnitType(type), _ => Unit.Create(type, instance, name));
        }
        return services;
    }
}
