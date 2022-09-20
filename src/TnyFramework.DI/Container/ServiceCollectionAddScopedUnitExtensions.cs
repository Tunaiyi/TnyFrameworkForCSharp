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
using TnyFramework.DI.Extensions;

namespace TnyFramework.DI.Container
{

    public static class ServiceCollectionAddScopedUnitExtensions
    {
        public static IServiceCollection AddScopedUnit(this IServiceCollection services, Type instanceType)
        {
            return services.AddScopedUnit(string.Empty, instanceType);
        }

        public static IServiceCollection AddScopedUnit(this IServiceCollection services, string name, Type instanceType)
        {
            var serviceInstance = new ScopedServiceInstance(new TypeServiceFactory(instanceType));
            return services.RegisterScopedUnit(name, serviceInstance, instanceType);
        }

        public static IServiceCollection AddScopedUnit(this IServiceCollection services, Type serviceType,
            Func<IServiceProvider, object> instanceFactory)
        {
            return services.AddScopedUnit(string.Empty, serviceType, instanceFactory);
        }

        public static IServiceCollection AddScopedUnit(this IServiceCollection services, string name, Type serviceType,
            Func<IServiceProvider, object> instanceFactory)
        {
            var serviceInstance = new ScopedServiceInstance(new FuncServiceFactory<object>(instanceFactory));
            services.RegisterScopedUnit(name, serviceInstance, serviceType);
            return services;
        }

        public static IServiceCollection AddScopedUnit(this IServiceCollection services, Type serviceType, Type instanceType)
        {
            return services.AddScopedUnit(string.Empty, serviceType, instanceType);
        }

        public static IServiceCollection AddScopedUnit(this IServiceCollection services, string name, Type serviceType, Type instanceType)
        {
            var serviceInstance = new ScopedServiceInstance(new TypeServiceFactory(instanceType));
            services.RegisterScopedUnit(name, serviceInstance, serviceType);
            return services;
        }

        public static IServiceCollection AddScopedUnit<TInstance>(this IServiceCollection services)
            where TInstance : class
        {
            return services.AddScopedUnit<TInstance>(string.Empty);
        }

        public static IServiceCollection AddScopedUnit<TInstance>(this IServiceCollection services, string name)
            where TInstance : class
        {
            var serviceType = typeof(TInstance);
            var serviceInstance = new ScopedServiceInstance(new TypeServiceFactory(serviceType));
            services.RegisterScopedUnit(name, serviceInstance, serviceType);
            return services;
        }

        public static IServiceCollection AddScopedUnit<TService>(this IServiceCollection services,
            Func<IServiceProvider, TService> instanceFactory)
            where TService : class
        {
            return services.AddScopedUnit(string.Empty, instanceFactory);
        }

        public static IServiceCollection AddScopedUnit<TService>(this IServiceCollection services, string name,
            Func<IServiceProvider, TService> instanceFactory)
            where TService : class
        {
            var serviceType = typeof(TService);
            var serviceInstance = new ScopedServiceInstance(new FuncServiceFactory<TService>(instanceFactory));
            return services.RegisterScopedUnit(name, serviceInstance, serviceType);
        }

        public static IServiceCollection AddScopedUnit<TService, TImplementation>(this IServiceCollection services)
            where TService : class
            where TImplementation : class, TService
        {
            return services.AddScopedUnit<TService, TImplementation>(string.Empty);
        }

        public static IServiceCollection AddScopedUnit<TService, TImplementation>(this IServiceCollection services, string name)
            where TService : class
            where TImplementation : class, TService
        {
            var serviceType = typeof(TService);
            var instanceType = typeof(TImplementation);
            var serviceInstance = new ScopedServiceInstance(new TypeServiceFactory(instanceType));
            return services.RegisterScopedUnit(name, serviceInstance, serviceType);
        }

        public static IServiceCollection AddScopedUnit<TService, TImplementation>(this IServiceCollection services,
            Func<IServiceProvider, TImplementation> instanceFactory)
            where TService : class
            where TImplementation : class, TService
        {
            return services.AddScopedUnit<TService, TImplementation>(string.Empty, instanceFactory);
        }

        public static IServiceCollection AddScopedUnit<TService, TImplementation>(this IServiceCollection services, string name,
            Func<IServiceProvider, TImplementation> instanceFactory)
            where TService : class
            where TImplementation : class, TService
        {
            var serviceType = typeof(TService);
            var serviceInstance = new ScopedServiceInstance(new FuncServiceFactory<TImplementation>(instanceFactory));
            return services.RegisterScopedUnit(name, serviceInstance, serviceType);
        }

        private static IServiceCollection RegisterScopedUnit(this IServiceCollection services, string name,
            IServiceInstance instance, Type instanceType)
        {
            var types = ServiceCollectionUtils.FindRegisterTypes(instanceType);
            return services.AddScopedUnits(name, instance, types);
        }

        private static IServiceCollection TryScopedUnits(this IServiceCollection services)
        {
            services.TryAdd(ServiceDescriptor.Scoped(typeof(IUnitCollection<>), typeof(UnitCollection<>)));
            return services;
        }

        private static IServiceCollection AddScopedUnits(this IServiceCollection services, string name,
            IServiceInstance instance, IEnumerable<Type> types)
        {
            services.TryScopedUnits();
            foreach (var type in types)
            {
                services.AddScopedServiceInstance(instance, type);
                services.AddScoped(Unit.UnitType(type), _ => Unit.Create(type, instance, name));
            }
            return services;
        }
    }

}
