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

    public static class ServiceCollectionBindTransientExtensions
    {
        public static IServiceCollection BindTransient(this IServiceCollection services, Type instanceType)
        {
            services.RegisterByType(instanceType);
            return services;
        }

        public static IServiceCollection BindTransient(this IServiceCollection services, Type serviceType,
            Func<IServiceProvider, object> instanceFactory)
        {
            services.RegisterByFactory(serviceType, instanceFactory);
            return services;
        }

        public static IServiceCollection BindTransient(this IServiceCollection services, Type serviceType, Type instanceType)
        {
            services.RegisterByType(serviceType, instanceType);
            return services;
        }

        public static IServiceCollection BindTransient<TService>(this IServiceCollection services)
            where TService : class
        {
            var serviceType = typeof(TService);
            services.RegisterByType(serviceType, serviceType);
            return services;
        }

        public static IServiceCollection BindTransient<TService>(this IServiceCollection services, Func<IServiceProvider, TService> instanceFactory)
            where TService : class
        {
            var serviceType = typeof(TService);
            services.RegisterByFactory(serviceType, instanceFactory);
            return services;
        }

        public static IServiceCollection BindTransient<TService, TImplementation>(this IServiceCollection services)
            where TService : class
            where TImplementation : class, TService
        {
            var serviceType = typeof(TService);
            var instanceType = typeof(TImplementation);
            services.RegisterByType(serviceType, instanceType);
            return services;
        }

        public static IServiceCollection BindTransient<TService, TImplementation>(this IServiceCollection services,
            Func<IServiceProvider, TImplementation> instanceFactory) where TService : class where TImplementation : class, TService
        {
            var serviceType = typeof(TService);
            services.RegisterByFactory(serviceType, instanceFactory);
            return services;
        }

        internal static IServiceInstance ToTransientServiceInstance(this IServiceCollection services, Type instanceType)
        {
            return new TransientServiceInstance(new TypeServiceFactory(instanceType));
        }

        internal static IServiceInstance ToTransientServiceInstance<TInstance>(this IServiceCollection services,
            Func<IServiceProvider, TInstance> instanceFactory)
        {
            return new TransientServiceInstance(new FuncServiceFactory<TInstance>(instanceFactory));
        }

        private static void RegisterByFactory<TInstance>(this IServiceCollection services, Type checkType,
            Func<IServiceProvider, TInstance> factory, bool force = true)
        {
            services.RegisterServiceInstance(checkType, services.ToTransientServiceInstance(factory), force);
        }

        private static void RegisterByType(this IServiceCollection services, Type checkType, Type? instanceType = null, bool force = true)
        {
            if (instanceType == null)
            {
                instanceType = checkType;
            }
            services.RegisterServiceInstance(checkType, services.ToTransientServiceInstance(instanceType), force);
        }

        private static void RegisterServiceInstance(this IServiceCollection services, Type checkType, IServiceInstance instance, bool force = true)
        {
            var types = ServiceCollectionUtils.FindRegisterTypes(checkType, force);
            services.AddTransientServiceInstances(instance, types);
        }

        internal static void AddTransientServiceInstance(this IServiceCollection services, IServiceInstance instance, Type type)
        {
            services.AddTransient(type, instance.Get);
        }

        internal static void AddTransientServiceInstances(this IServiceCollection services, IServiceInstance instance, IEnumerable<Type> serviceTypes)
        {
            foreach (var type in serviceTypes)
            {
                services.AddTransient(type, instance.Get);
            }
        }
    }

}
