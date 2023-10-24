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

    public static class ServiceCollectionSingletonUnitWithExtensions
    {
        public static IServiceCollection AddSingletonUnit(this IServiceCollection services, Type instanceType, params Type[] serviceTypes)
        {
            return services.AddSingletonUnit(string.Empty, instanceType, serviceTypes);
        }

        public static IServiceCollection AddSingletonUnit(this IServiceCollection services, string name, Type instanceType,
            params Type[] serviceTypes)
        {
            var serviceInstance = new SingletonServiceInstance(new TypeServiceFactory(instanceType));
            return services.RegisterSingletonUnitWith(name, serviceInstance, instanceType, serviceTypes);
        }

        public static IServiceCollection AddSingletonUnit(this IServiceCollection services, Type serviceType, object instance,
            params Type[] serviceTypes)
        {
            return services.AddSingletonUnit(string.Empty, serviceType, instance, serviceTypes);
        }

        public static IServiceCollection AddSingletonUnit(this IServiceCollection services, string name, Type serviceType, object instance,
            params Type[] serviceTypes)
        {
            var serviceInstance = new SingletonServiceInstance(new ObjectServiceFactory(instance));
            services.RegisterSingletonUnitWith(name, serviceInstance, serviceType, serviceTypes);
            return services;
        }

        public static IServiceCollection AddSingletonUnit(this IServiceCollection services, Type serviceType,
            Func<IServiceProvider, object> instanceFactory, params Type[] serviceTypes)
        {
            return services.AddSingletonUnit(string.Empty, serviceType, instanceFactory, serviceTypes);
        }

        public static IServiceCollection AddSingletonUnit(this IServiceCollection services, string name, Type serviceType,
            Func<IServiceProvider, object> instanceFactory, params Type[] serviceTypes)
        {
            var serviceInstance = new SingletonServiceInstance(new FuncServiceFactory<object>(instanceFactory));
            services.RegisterSingletonUnitWith(name, serviceInstance, serviceType, serviceTypes);
            return services;
        }

        public static IServiceCollection AddSingletonUnit(this IServiceCollection services, Type serviceType, Type instanceType,
            params Type[] serviceTypes)
        {
            return services.AddSingletonUnit(string.Empty, serviceType, instanceType, serviceTypes);
        }

        public static IServiceCollection AddSingletonUnit(this IServiceCollection services, string name, Type serviceType, Type instanceType,
            params Type[] serviceTypes)
        {
            var serviceInstance = new SingletonServiceInstance(new TypeServiceFactory(instanceType));
            services.RegisterSingletonUnitWith(name, serviceInstance, serviceType, serviceTypes);
            return services;
        }

        public static IServiceCollection AddSingletonUnit<TInstance>(this IServiceCollection services, params Type[] serviceTypes)
            where TInstance : class
        {
            return services.AddSingletonUnit<TInstance>(string.Empty, serviceTypes);
        }

        public static IServiceCollection AddSingletonUnit<TInstance>(this IServiceCollection services, string name, params Type[] serviceTypes)
            where TInstance : class
        {
            var serviceType = typeof(TInstance);
            var serviceInstance = new SingletonServiceInstance(new TypeServiceFactory(serviceType));
            services.RegisterSingletonUnitWith(name, serviceInstance, serviceType, serviceTypes);
            return services;
        }

        public static IServiceCollection AddSingletonUnit<TInstance>(this IServiceCollection services, TInstance instance, params Type[] serviceTypes)
        {
            return services.AddSingletonUnit(string.Empty, instance, serviceTypes);
        }

        public static IServiceCollection AddSingletonUnit<TInstance>(this IServiceCollection services, string name, TInstance instance,
            params Type[] serviceTypes)
        {
            var serviceInstance = new SingletonServiceInstance(new ObjectServiceFactory(instance!));
            return services.RegisterSingletonUnitWith(name, serviceInstance, typeof(TInstance), serviceTypes);
        }

        public static IServiceCollection AddSingletonUnit<TService>(this IServiceCollection services,
            Func<IServiceProvider, TService> instanceFactory, params Type[] serviceTypes)
            where TService : class
        {
            return services.AddSingletonUnit(string.Empty, instanceFactory, serviceTypes);
        }

        public static IServiceCollection AddSingletonUnit<TService>(this IServiceCollection services, string name,
            Func<IServiceProvider, TService> instanceFactory, params Type[] serviceTypes)
            where TService : class
        {
            var serviceType = typeof(TService);
            var serviceInstance = new SingletonServiceInstance(new FuncServiceFactory<TService>(instanceFactory));
            return services.RegisterSingletonUnitWith(name, serviceInstance, serviceType, serviceTypes);
        }

        private static IServiceCollection RegisterSingletonUnitWith(this IServiceCollection services, string name,
            IServiceInstance instance, Type instanceType, params Type[] serviceTypes)
        {
            services.AddSingletonUnitWith(name, instance, instanceType);
            services.AddSingletonUnitsWith(name, instance, serviceTypes);
            return services;
        }

        private static IServiceCollection TrySingletonUnits(this IServiceCollection services)
        {
            services.TryAdd(ServiceDescriptor.Singleton(typeof(IUnitCollection<>), typeof(UnitCollection<>)));
            return services;
        }

        private static IServiceCollection AddSingletonUnitWith(this IServiceCollection services, string name,
            IServiceInstance instance, Type type)
        {
            services.TrySingletonUnits();
            services.AddSingletonServiceInstance(instance, type);
            var unit = Unit.Create(type, instance, name);
            services.AddSingleton(Unit.UnitType(type), unit);
            return services;
        }

        private static IServiceCollection AddSingletonUnitsWith(this IServiceCollection services, string name,
            IServiceInstance instance, IEnumerable<Type> types)
        {
            services.TrySingletonUnits();
            foreach (var type in types)
            {
                services.AddSingletonServiceInstance(instance, type);
                var unit = Unit.Create(type, instance, name);
                services.AddSingleton(Unit.UnitType(type), unit);
            }
            return services;
        }
    }

}
