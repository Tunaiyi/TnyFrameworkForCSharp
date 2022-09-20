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

namespace TnyFramework.DI.Extensions
{

    public static class ServiceCollectionBindSingletonWithExtensions
    {
        public static IServiceCollection BindSingletonWith(this IServiceCollection services, Type instanceType, params Type[] serviceTypes)
        {
            services.RegisterByTypeWith(instanceType, instanceType, serviceTypes);
            return services;
        }

        public static IServiceCollection BindSingletonWith(this IServiceCollection services, object instance, params Type[] serviceTypes)
        {
            var instanceType = instance.GetType();
            services.RegisterByInstanceWith(instance, instanceType, serviceTypes);
            return services;
        }

        public static IServiceCollection BindSingletonWith(this IServiceCollection services, Type instanceType, object instance,
            params Type[] serviceTypes)
        {
            services.RegisterByInstanceWith(instance, instanceType, serviceTypes);
            return services;
        }

        public static IServiceCollection BindSingletonWith(this IServiceCollection services, Type instanceType,
            Func<IServiceProvider, object> instanceFactory, params Type[] serviceTypes)
        {
            services.RegisterByFactoryWith(instanceFactory, instanceType, serviceTypes);
            return services;
        }

        public static IServiceCollection BindSingletonWith<TService>(this IServiceCollection services,
            params Type[] serviceTypes)
            where TService : class
        {
            var instanceType = typeof(TService);
            services.RegisterByTypeWith(instanceType, instanceType, serviceTypes);
            return services;
        }

        public static IServiceCollection BindSingletonWith<TService>(this IServiceCollection services,
            TService instance, params Type[] serviceTypes)
            where TService : class
        {
            var instanceType = typeof(TService);
            services.RegisterByInstanceWith(instance, instanceType, serviceTypes);
            return services;
        }

        public static IServiceCollection BindSingletonWith<TService>(this IServiceCollection services,
            Func<IServiceProvider, TService> instanceFactory, params Type[] serviceTypes)
            where TService : class
        {
            var instanceType = typeof(TService);
            services.RegisterByFactoryWith(instanceFactory, instanceType, serviceTypes);
            return services;
        }

        public static IServiceCollection BindSingletonWith<TService, TImplement>(this IServiceCollection services,
            params Type[] serviceTypes)
            where TService : class
        {
            var instanceType = typeof(TImplement);
            var insServiceType = typeof(TService);
            services.RegisterByTypeWith(instanceType, insServiceType, serviceTypes);
            return services;
        }

        public static IServiceCollection BindSingletonWith<TService, TImplement>(this IServiceCollection services,
            TImplement instance, params Type[] serviceTypes)
            where TService : class
        {
            var insServiceType = typeof(TService);
            services.RegisterByInstanceWith(instance, insServiceType, serviceTypes);
            return services;
        }

        public static IServiceCollection BindSingletonWith<TService, TImplement>(this IServiceCollection services,
            Func<IServiceProvider, TImplement> instanceFactory, params Type[] serviceTypes)
            where TService : class
        {
            var insServiceType = typeof(TService);
            services.RegisterByFactoryWith(instanceFactory, insServiceType, serviceTypes);
            return services;
        }

        private static void RegisterByTypeWith(this IServiceCollection services, Type instanceType,
            Type insServiceType, IEnumerable<Type> serviceTypes)
        {
            var serviceInstance = services.ToSingletonServiceInstance(instanceType);
            if (insServiceType != null)
            {
                services.AddSingletonServiceInstance(serviceInstance, insServiceType);
            }
            services.AddSingletonServiceInstances(serviceInstance, serviceTypes);
        }

        private static void RegisterByInstanceWith(this IServiceCollection services, object instance,
            Type insServiceType, IEnumerable<Type> serviceTypes)
        {
            var serviceInstance = services.ToSingletonServiceInstance(instance);
            if (insServiceType != null)
            {
                services.AddSingletonServiceInstance(serviceInstance, insServiceType);
            }
            services.AddSingletonServiceInstances(serviceInstance, serviceTypes);
        }

        private static void RegisterByFactoryWith<TInstance>(this IServiceCollection services, Func<IServiceProvider, TInstance> instanceFactory,
            Type insServiceType, IEnumerable<Type> serviceTypes)
        {
            var serviceInstance = services.ToSingletonServiceInstance(instanceFactory);
            if (insServiceType != null)
            {
                services.AddSingletonServiceInstance(serviceInstance, insServiceType);
            }
            services.AddSingletonServiceInstances(serviceInstance, serviceTypes);
        }
    }

}
