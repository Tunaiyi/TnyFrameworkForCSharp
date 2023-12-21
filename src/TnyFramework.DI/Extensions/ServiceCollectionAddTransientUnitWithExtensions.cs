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

namespace TnyFramework.DI.Extensions
{

    public static class ServiceCollectionTransientUnitWithExtensions
    {
        public static IServiceCollection AddTransientUnit(this IServiceCollection services, Type instanceType, params Type[] serviceTypes)
        {
            return services.AddTransientUnit(string.Empty, instanceType, serviceTypes);
        }

        public static IServiceCollection AddTransientUnit(this IServiceCollection services, string name, Type instanceType,
            params Type[] serviceTypes)
        {
            var serviceInstance = new TransientServiceInstance(new TypeServiceFactory(instanceType));
            return services.RegisterTransientUnitWith(name, serviceInstance, instanceType, serviceTypes);
        }

        public static IServiceCollection AddTransientUnit(this IServiceCollection services, Type serviceType,
            Func<IServiceProvider, object> instanceFactory, params Type[] serviceTypes)
        {
            return services.AddTransientUnit(string.Empty, serviceType, instanceFactory, serviceTypes);
        }

        public static IServiceCollection AddTransientUnit(this IServiceCollection services, string name, Type serviceType,
            Func<IServiceProvider, object> instanceFactory, params Type[] serviceTypes)
        {
            var serviceInstance = new TransientServiceInstance(new FuncServiceFactory<object>(instanceFactory));
            services.RegisterTransientUnitWith(name, serviceInstance, serviceType, serviceTypes);
            return services;
        }

        public static IServiceCollection AddTransientUnit(this IServiceCollection services, Type serviceType, Type instanceType,
            params Type[] serviceTypes)
        {
            return services.AddTransientUnit(string.Empty, serviceType, instanceType, serviceTypes);
        }

        public static IServiceCollection AddTransientUnit(this IServiceCollection services, string name, Type serviceType, Type instanceType,
            params Type[] serviceTypes)
        {
            var serviceInstance = new TransientServiceInstance(new TypeServiceFactory(instanceType));
            services.RegisterTransientUnitWith(name, serviceInstance, serviceType, serviceTypes);
            return services;
        }

        public static IServiceCollection AddTransientUnit<TInstance>(this IServiceCollection services, params Type[] serviceTypes)
            where TInstance : class
        {
            return services.AddTransientUnit<TInstance>(string.Empty, serviceTypes);
        }

        public static IServiceCollection AddTransientUnit<TInstance>(this IServiceCollection services, string name, params Type[] serviceTypes)
            where TInstance : class
        {
            var serviceType = typeof(TInstance);
            var serviceInstance = new TransientServiceInstance(new TypeServiceFactory(serviceType));
            services.RegisterTransientUnitWith(name, serviceInstance, serviceType, serviceTypes);
            return services;
        }

        public static IServiceCollection AddTransientUnit<TService>(this IServiceCollection services,
            Func<IServiceProvider, TService> instanceFactory, params Type[] serviceTypes)
            where TService : class
        {
            return services.AddTransientUnit(string.Empty, instanceFactory, serviceTypes);
        }

        public static IServiceCollection AddTransientUnit<TService>(this IServiceCollection services, string name,
            Func<IServiceProvider, TService> instanceFactory, params Type[] serviceTypes)
            where TService : class
        {
            var serviceType = typeof(TService);
            var serviceInstance = new TransientServiceInstance(new FuncServiceFactory<TService>(instanceFactory));
            return services.RegisterTransientUnitWith(name, serviceInstance, serviceType, serviceTypes);
        }

        private static IServiceCollection RegisterTransientUnitWith(this IServiceCollection services, string name,
            IServiceInstance instance, Type instanceType, params Type[] serviceTypes)
        {
            services.AddTransientUnitWith(name, instance, instanceType);
            services.AddTransientUnitsWith(name, instance, serviceTypes);
            return services;
        }

        private static IServiceCollection TryTransientUnits(this IServiceCollection services)
        {
            services.TryAdd(ServiceDescriptor.Transient(typeof(IUnitCollection<>), typeof(UnitCollection<>)));
            return services;
        }

        private static IServiceCollection AddTransientUnitWith(this IServiceCollection services, string name,
            IServiceInstance instance, Type type)
        {
            services.TryTransientUnits();
            services.AddTransientServiceInstance(instance, type);
            services.AddTransient(Unit.UnitType(type), _ => Unit.Create(type, instance, name));
            return services;
        }

        private static IServiceCollection AddTransientUnitsWith(this IServiceCollection services, string name,
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

}
