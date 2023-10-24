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
using TnyFramework.DI.Container;

namespace TnyFramework.DI.Extensions
{

    public static class ServiceCollectionBindSingletonExtensions
    {
        public static IServiceCollection BindSingleton(this IServiceCollection services, object instance)
        {
            services.RegisterByInstance(instance.GetType(), instance);
            return services;
        }

        public static IServiceCollection BindSingleton(this IServiceCollection services, Type instanceType)
        {
            services.RegisterByType(instanceType);
            return services;
        }

        public static IServiceCollection BindSingleton(this IServiceCollection services, Type serviceType, object instance)
        {
            services.RegisterByInstance(serviceType, instance);
            return services;
        }

        public static IServiceCollection BindSingleton(this IServiceCollection services, Type serviceType,
            Func<IServiceProvider, object> instanceFactory)
        {
            services.RegisterByFactory(serviceType, instanceFactory);
            return services;
        }

        public static IServiceCollection BindSingleton(this IServiceCollection services, Type serviceType, Type instanceType)
        {
            services.RegisterByType(serviceType, instanceType);
            return services;
        }

        public static IServiceCollection BindSingleton<TService>(this IServiceCollection services)
            where TService : class
        {
            var serviceType = typeof(TService);
            services.RegisterByType(serviceType, serviceType);
            return services;
        }

        public static IServiceCollection BindSingleton<TService>(this IServiceCollection services, TService instance) where TService : class
        {
            var serviceType = typeof(TService);
            services.RegisterByInstance(serviceType, instance);
            return services;
        }

        public static IServiceCollection BindSingleton<TService>(this IServiceCollection services, Func<IServiceProvider, TService> instanceFactory)
            where TService : class
        {
            var serviceType = typeof(TService);
            services.RegisterByFactory(serviceType, instanceFactory);
            return services;
        }

        public static IServiceCollection BindSingleton<TService, TImplementation>(this IServiceCollection services)
            where TService : class
            where TImplementation : class, TService
        {
            var serviceType = typeof(TService);
            var instanceType = typeof(TImplementation);
            services.RegisterByType(serviceType, instanceType);
            return services;
        }

        public static IServiceCollection BindSingleton<TService, TImplementation>(this IServiceCollection services,
            TImplementation instance) where TService : class where TImplementation : class, TService
        {
            var serviceType = typeof(TService);
            services.RegisterByInstance(serviceType, instance);
            return services;
        }

        public static IServiceCollection BindSingleton<TService, TImplementation>(this IServiceCollection services,
            Func<IServiceProvider, TImplementation> instanceFactory) where TService : class where TImplementation : class, TService
        {
            var serviceType = typeof(TService);
            services.RegisterByFactory(serviceType, instanceFactory);
            return services;
        }

        internal static IServiceInstance ToSingletonServiceInstance(this IServiceCollection services, Type instanceType)
        {
            return new SingletonServiceInstance(new TypeServiceFactory(instanceType));
        }

        internal static IServiceInstance ToSingletonServiceInstance(this IServiceCollection services, object instance)
        {
            return new ObjectServiceInstance<object>(instance);
        }

        internal static IServiceInstance ToSingletonServiceInstance<TInstance>(this IServiceCollection services,
            Func<IServiceProvider, TInstance> instanceFactory)
        {
            return new SingletonServiceInstance(new FuncServiceFactory<TInstance>(instanceFactory));
        }

        private static void RegisterByInstance(this IServiceCollection services, Type serviceType,
            object instance, bool force = true)
        {
            services.RegisterServiceInstance(serviceType, services.ToSingletonServiceInstance(instance), force);
        }

        private static void RegisterByFactory<TInstance>(this IServiceCollection services, Type checkType,
            Func<IServiceProvider, TInstance> factory, bool force = true)
        {
            services.RegisterServiceInstance(checkType, services.ToSingletonServiceInstance(factory), force);
        }

        private static void RegisterByType(this IServiceCollection services, Type checkType, Type? instanceType = null, bool force = true)
        {
            if (instanceType == null)
            {
                instanceType = checkType;
            }
            services.RegisterServiceInstance(checkType, services.ToSingletonServiceInstance(instanceType), force);
        }

        private static void RegisterServiceInstance(this IServiceCollection services, Type checkType, IServiceInstance instance, bool force = true)
        {
            var types = ServiceCollectionUtils.FindRegisterTypes(checkType, force);
            services.AddSingletonServiceInstances(instance, types);
        }

        internal static void AddSingletonServiceInstance(this IServiceCollection services, IServiceInstance instance, Type type)
        {
            services.AddSingleton(type, instance.Get);
        }

        internal static void AddSingletonServiceInstances(this IServiceCollection services, IServiceInstance instance, IEnumerable<Type> serviceTypes)
        {
            foreach (var type in serviceTypes)
            {
                services.AddSingleton(type, instance.Get);
            }
        }
    }

}
