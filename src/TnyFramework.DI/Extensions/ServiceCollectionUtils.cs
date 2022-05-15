using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using TnyFramework.DI.Attributes;

namespace TnyFramework.DI.Extensions
{

    public static class ServiceCollectionUtils
    {
        public static IList<Type> FindRegisterTypes(Type checkType, bool force = true)
        {
            if (checkType == null || checkType == typeof(object))
            {
                return ImmutableList<Type>.Empty;
            }
            var registeredTypes = new List<Type>();
            if (force || checkType.GetCustomAttribute<ServiceInterfaceAttribute>() != null)
            {
                registeredTypes.Add(checkType);
            }
            var baseType = checkType.BaseType;
            if (baseType != null)
            {
                registeredTypes.AddRange(FindRegisterTypes(baseType, force));
            }
            var interfaces = checkType.GetInterfaces();
            foreach (var type in interfaces)
            {
                registeredTypes.AddRange(FindRegisterTypes(type, force));
            }
            return registeredTypes.Distinct().ToImmutableList();
        }
    }

}
