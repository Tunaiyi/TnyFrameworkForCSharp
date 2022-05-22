using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TnyFramework.Common.Logger;
using TnyFramework.DI.Attributes;
using TnyFramework.DI.Container;

namespace TnyFramework.DI.Extensions
{

    public static class ServiceCollectionComponentExtensions
    {
        private static ILogger _LOGGER;

        private static ILogger Logger => _LOGGER ?? (_LOGGER = LogFactory.Logger(typeof(ServiceCollectionComponentExtensions))); 


        public static IServiceCollection AddComponents(this IServiceCollection service)
        {
            foreach (var type in ComponentTypeSelector.Types)
            {
                AddComponent(service, type);
            }
            return service;
        }

        public static IServiceCollection AddComponents(this IServiceCollection service, IEnumerable<Assembly> assemblies)
        {
            return service.DoAddComponents(assemblies);
        }

        private static void AddComponent(IServiceCollection service, Type type)
        {
            var componentAttribute = type.GetCustomAttribute<ComponentAttribute>() ?? type.GetCustomAttribute<ServiceAttribute>();
            if (componentAttribute == null)
                return;
            Logger.LogInformation("Add Component : {Type}", type);
            var name = componentAttribute.Named() ? "" : componentAttribute.Name;
            switch (componentAttribute.Mode)
            {
                case DIMode.Singleton:
                    service.AddSingletonUnit(name, type);
                    break;
                case DIMode.Scope:
                    service.AddScopedUnit(name, type);
                    break;
                case DIMode.Transient:
                    service.AddTransientUnit(name, type);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static IServiceCollection DoAddComponents(this IServiceCollection service, IEnumerable<Assembly> assemblies)
        {
            foreach (var assembly in assemblies)
            {
                foreach (var type in assembly.GetTypes())
                {
                    AddComponent(service, type);
                }
            }
            return service;
        }
    }

}
