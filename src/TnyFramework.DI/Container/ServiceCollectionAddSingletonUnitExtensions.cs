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

    public static class ServiceCollectionAddSingletonUnitExtensions
    {
        public static IServiceCollection AddSingletonUnit(this IServiceCollection services, Type instanceType)
        {
            return services.AddSingletonUnit(string.Empty, instanceType);
        }

        public static IServiceCollection AddSingletonUnit(this IServiceCollection services, string name, Type instanceType)
        {
            var serviceInstance = new SingletonServiceInstance(new TypeServiceFactory(instanceType));
            return services.RegisterSingletonUnit(name, serviceInstance, instanceType);
        }

        public static IServiceCollection AddSingletonUnit(this IServiceCollection services, Type serviceType, object instance)
        {
            return services.AddSingletonUnit(string.Empty, serviceType, instance);
        }

        public static IServiceCollection AddSingletonUnit(this IServiceCollection services, string name, Type serviceType, object instance)
        {
            var serviceInstance = new SingletonServiceInstance(new ObjectServiceFactory(instance));
            services.RegisterSingletonUnit(name, serviceInstance, serviceType);
            return services;
        }

        public static IServiceCollection AddSingletonUnit(this IServiceCollection services, Type serviceType,
            Func<IServiceProvider, object> instanceFactory)
        {
            return services.AddSingletonUnit(string.Empty, serviceType, instanceFactory);
        }

        public static IServiceCollection AddSingletonUnit(this IServiceCollection services, string name, Type serviceType,
            Func<IServiceProvider, object> instanceFactory)
        {
            var serviceInstance = new SingletonServiceInstance(new FuncServiceFactory<object>(instanceFactory));
            services.RegisterSingletonUnit(name, serviceInstance, serviceType);
            return services;
        }

        public static IServiceCollection AddSingletonUnit(this IServiceCollection services, Type serviceType, Type instanceType)
        {
            return services.AddSingletonUnit(string.Empty, serviceType, instanceType);
        }

        public static IServiceCollection AddSingletonUnit(this IServiceCollection services, string name, Type serviceType, Type instanceType)
        {
            var serviceInstance = new SingletonServiceInstance(new TypeServiceFactory(instanceType));
            services.RegisterSingletonUnit(name, serviceInstance, serviceType);
            return services;
        }

        public static IServiceCollection AddSingletonUnit<TInstance>(this IServiceCollection services)
            where TInstance : class
        {
            return services.AddSingletonUnit<TInstance>(string.Empty);
        }

        public static IServiceCollection AddSingletonUnit<TInstance>(this IServiceCollection services, string name)
            where TInstance : class
        {
            var serviceType = typeof(TInstance);
            var serviceInstance = new SingletonServiceInstance(new TypeServiceFactory(serviceType));
            services.RegisterSingletonUnit(name, serviceInstance, serviceType);
            return services;
        }

        public static IServiceCollection AddSingletonUnit<TInstance>(this IServiceCollection services, TInstance instance)
        {
            return services.AddSingletonUnit(string.Empty, instance);
        }

        public static IServiceCollection AddSingletonUnit<TInstance>(this IServiceCollection services, string name, TInstance instance)
        {
            var serviceInstance = new SingletonServiceInstance(new ObjectServiceFactory(instance!));
            return services.RegisterSingletonUnit(name, serviceInstance, typeof(TInstance));
        }

        public static IServiceCollection AddSingletonUnit<TService>(this IServiceCollection services,
            Func<IServiceProvider, TService> instanceFactory)
            where TService : class
        {
            return services.AddSingletonUnit(string.Empty, instanceFactory);
        }

        public static IServiceCollection AddSingletonUnit<TService>(this IServiceCollection services, string name,
            Func<IServiceProvider, TService> instanceFactory)
            where TService : class
        {
            var serviceType = typeof(TService);
            var serviceInstance = new SingletonServiceInstance(new FuncServiceFactory<TService>(instanceFactory));
            return services.RegisterSingletonUnit(name, serviceInstance, serviceType);
        }

        public static IServiceCollection AddSingletonUnit<TService, TImplementation>(this IServiceCollection services)
            where TService : class
            where TImplementation : class, TService
        {
            return services.AddSingletonUnit<TService, TImplementation>(string.Empty);
        }

        public static IServiceCollection AddSingletonUnit<TService, TImplementation>(this IServiceCollection services, string name)
            where TService : class
            where TImplementation : class, TService
        {
            var serviceType = typeof(TService);
            var instanceType = typeof(TImplementation);
            var serviceInstance = new SingletonServiceInstance(new TypeServiceFactory(instanceType));
            return services.RegisterSingletonUnit(name, serviceInstance, serviceType);
        }

        public static IServiceCollection AddSingletonUnit<TService, TImplementation>(this IServiceCollection services,
            TImplementation instance)
            where TService : class
            where TImplementation : class, TService
        {
            return services.AddSingletonUnit<TService, TImplementation>(string.Empty, instance);
        }

        public static IServiceCollection AddSingletonUnit<TService, TImplementation>(this IServiceCollection services, string name,
            TImplementation instance)
            where TService : class
            where TImplementation : class, TService
        {
            var serviceType = typeof(TService);
            var serviceInstance = new SingletonServiceInstance(new ObjectServiceFactory(instance));
            return services.RegisterSingletonUnit(name, serviceInstance, serviceType);
        }

        public static IServiceCollection AddSingletonUnit<TService, TImplementation>(this IServiceCollection services,
            Func<IServiceProvider, TImplementation> instanceFactory)
            where TService : class
            where TImplementation : class, TService
        {
            return services.AddSingletonUnit<TService, TImplementation>(string.Empty, instanceFactory);
        }

        public static IServiceCollection AddSingletonUnit<TService, TImplementation>(this IServiceCollection services, string name,
            Func<IServiceProvider, TImplementation> instanceFactory)
            where TService : class
            where TImplementation : class, TService
        {
            var serviceType = typeof(TService);
            var serviceInstance = new SingletonServiceInstance(new FuncServiceFactory<TImplementation>(instanceFactory));
            return services.RegisterSingletonUnit(name, serviceInstance, serviceType);
        }

        private static IServiceCollection RegisterSingletonUnit(this IServiceCollection services, string name,
            IServiceInstance instance, Type instanceType)
        {
            var types = ServiceCollectionUtils.FindRegisterTypes(instanceType);
            return services.AddSingletonUnits(name, instance, types);
        }

        private static IServiceCollection TrySingletonUnits(this IServiceCollection services)
        {
            services.TryAdd(ServiceDescriptor.Singleton(typeof(IUnitCollection<>), typeof(UnitCollection<>)));
            return services;
        }

        private static IServiceCollection AddSingletonUnits(this IServiceCollection services, string name,
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
