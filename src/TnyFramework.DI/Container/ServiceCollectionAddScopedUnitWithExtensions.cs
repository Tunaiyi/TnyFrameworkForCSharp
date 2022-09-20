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

    public static class ServiceCollectionScopedUnitWithExtensions
    {
        public static IServiceCollection AddScopedUnit(this IServiceCollection services, Type instanceType, params Type[] serviceTypes)
        {
            return services.AddScopedUnit(string.Empty, instanceType, serviceTypes);
        }

        public static IServiceCollection AddScopedUnit(this IServiceCollection services, string name, Type instanceType,
            params Type[] serviceTypes)
        {
            var serviceInstance = new ScopedServiceInstance(new TypeServiceFactory(instanceType));
            return services.RegisterScopedUnitWith(name, serviceInstance, instanceType, serviceTypes);
        }

        public static IServiceCollection AddScopedUnit(this IServiceCollection services, Type serviceType,
            Func<IServiceProvider, object> instanceFactory, params Type[] serviceTypes)
        {
            return services.AddScopedUnit(string.Empty, serviceType, instanceFactory, serviceTypes);
        }

        public static IServiceCollection AddScopedUnit(this IServiceCollection services, string name, Type serviceType,
            Func<IServiceProvider, object> instanceFactory, params Type[] serviceTypes)
        {
            var serviceInstance = new ScopedServiceInstance(new FuncServiceFactory<object>(instanceFactory));
            services.RegisterScopedUnitWith(name, serviceInstance, serviceType, serviceTypes);
            return services;
        }

        public static IServiceCollection AddScopedUnit(this IServiceCollection services, Type serviceType, Type instanceType,
            params Type[] serviceTypes)
        {
            return services.AddScopedUnit(string.Empty, serviceType, instanceType, serviceTypes);
        }

        public static IServiceCollection AddScopedUnit(this IServiceCollection services, string name, Type serviceType, Type instanceType,
            params Type[] serviceTypes)
        {
            var serviceInstance = new ScopedServiceInstance(new TypeServiceFactory(instanceType));
            services.RegisterScopedUnitWith(name, serviceInstance, serviceType, serviceTypes);
            return services;
        }

        public static IServiceCollection AddScopedUnit<TInstance>(this IServiceCollection services, params Type[] serviceTypes)
            where TInstance : class
        {
            return services.AddScopedUnit<TInstance>(string.Empty, serviceTypes);
        }

        public static IServiceCollection AddScopedUnit<TInstance>(this IServiceCollection services, string name, params Type[] serviceTypes)
            where TInstance : class
        {
            var serviceType = typeof(TInstance);
            var serviceInstance = new ScopedServiceInstance(new TypeServiceFactory(serviceType));
            services.RegisterScopedUnitWith(name, serviceInstance, serviceType, serviceTypes);
            return services;
        }

        public static IServiceCollection AddScopedUnit<TService>(this IServiceCollection services,
            Func<IServiceProvider, TService> instanceFactory, params Type[] serviceTypes)
            where TService : class
        {
            return services.AddScopedUnit(string.Empty, instanceFactory, serviceTypes);
        }

        public static IServiceCollection AddScopedUnit<TService>(this IServiceCollection services, string name,
            Func<IServiceProvider, TService> instanceFactory, params Type[] serviceTypes)
            where TService : class
        {
            var serviceType = typeof(TService);
            var serviceInstance = new ScopedServiceInstance(new FuncServiceFactory<TService>(instanceFactory));
            return services.RegisterScopedUnitWith(name, serviceInstance, serviceType, serviceTypes);
        }

        private static IServiceCollection RegisterScopedUnitWith(this IServiceCollection services, string name,
            IServiceInstance instance, Type instanceType, params Type[] serviceTypes)
        {
            services.AddScopedUnitWith(name, instance, instanceType);
            services.AddScopedUnitsWith(name, instance, serviceTypes);
            return services;
        }

        private static IServiceCollection TryScopedUnits(this IServiceCollection services)
        {
            services.TryAdd(ServiceDescriptor.Scoped(typeof(IUnitCollection<>), typeof(UnitCollection<>)));
            return services;
        }

        private static IServiceCollection AddScopedUnitWith(this IServiceCollection services, string name,
            IServiceInstance instance, Type type)
        {
            services.TryScopedUnits();
            services.AddScopedServiceInstance(instance, type);
            services.AddScoped(Unit.UnitType(type), _ => Unit.Create(type, instance, name));
            return services;
        }

        private static IServiceCollection AddScopedUnitsWith(this IServiceCollection services, string name,
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
