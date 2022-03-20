using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
namespace TnyFramework.DI.Extensions
{
    public static class ServiceCollectionBindScopedWithExtensions
    {
        public static IServiceCollection BindScopedWith(this IServiceCollection services, Type instanceType, params Type[] serviceTypes)
        {
            services.RegisterByTypeWith(instanceType, instanceType, serviceTypes);
            return services;
        }


        public static IServiceCollection BindScopedWith(this IServiceCollection services, Type instanceType,
            Func<IServiceProvider, object> instanceFactory, params Type[] serviceTypes)
        {
            services.RegisterByFactoryWith(instanceFactory, instanceType, serviceTypes);
            return services;
        }


        public static IServiceCollection BindScopedWith<TService>(this IServiceCollection services,
            params Type[] serviceTypes)
            where TService : class
        {
            var instanceType = typeof(TService);
            services.RegisterByTypeWith(instanceType, instanceType, serviceTypes);
            return services;
        }


        public static IServiceCollection BindScopedWith<TService>(this IServiceCollection services,
            Func<IServiceProvider, TService> instanceFactory, params Type[] serviceTypes)
            where TService : class
        {
            var instanceType = typeof(TService);
            services.RegisterByFactoryWith(instanceFactory, instanceType, serviceTypes);
            return services;
        }


        public static IServiceCollection BindScopedWith<TService, TImplement>(this IServiceCollection services,
            params Type[] serviceTypes)
            where TService : class
        {
            var instanceType = typeof(TImplement);
            var insServiceType = typeof(TService);
            services.RegisterByTypeWith(instanceType, insServiceType, serviceTypes);
            return services;
        }


        public static IServiceCollection BindScopedWith<TService, TImplement>(this IServiceCollection services,
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
            var serviceInstance = services.ToScopedServiceInstance(instanceType);
            if (insServiceType != null)
            {
                services.AddScopedServiceInstance(serviceInstance, insServiceType);
            }
            services.AddScopedServiceInstances(serviceInstance, serviceTypes);
        }


        private static void RegisterByFactoryWith<TInstance>(this IServiceCollection services, Func<IServiceProvider, TInstance> instanceFactory,
            Type insServiceType, IEnumerable<Type> serviceTypes)
        {
            var serviceInstance = services.ToScopedServiceInstance(instanceFactory);
            if (insServiceType != null)
            {
                services.AddScopedServiceInstance(serviceInstance, insServiceType);
            }
            services.AddScopedServiceInstances(serviceInstance, serviceTypes);
        }
    }
}