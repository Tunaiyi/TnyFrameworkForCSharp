using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TnyFramework.Common.Assemblies;
using TnyFramework.Common.Logger;
using TnyFramework.DI.Attributes;
using TnyFramework.DI.Container;

namespace TnyFramework.DI.Extensions
{

    public static class ServiceCollectionComponentExtensions
    {
        private static readonly ILogger LOGGER = LogFactory.Logger<IServiceCollection>();


        public static IServiceCollection AddComponents(this IServiceCollection service)
        {
            return service.DoAddComponents(AssemblyUtils.LoadAllAssemblies());
        }


        public static IServiceCollection AddComponents(this IServiceCollection service, ICollection<Assembly> assemblies)
        {
            return service.DoAddComponents(AssemblyUtils.LoadAllAssemblies(assemblies));
        }


        private static IServiceCollection DoAddComponents(this IServiceCollection service, IEnumerable<Assembly> assemblies)
        {
            foreach (var assembly in assemblies)
            {
                foreach (var type in assembly.GetTypes())
                {
                    var componentAttribute = type.GetCustomAttribute<ComponentAttribute>() ?? type.GetCustomAttribute<ServiceAttribute>();
                    if (componentAttribute != null)
                    {
                        LOGGER.LogInformation("Add Component : {Type}", type);
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
                }
            }
            return service;
        }
    }

}
