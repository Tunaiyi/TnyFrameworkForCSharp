using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.Logging;
using TnyFramework.Common.Logger;
using TnyFramework.Common.Scanner;
using TypeFilter = TnyFramework.Common.Scanner.TypeFilter;

namespace TnyFramework.Common.Enum
{

    public class EnumLoaderTypeSelector : TypeSelectorDefinition
    {
        private static readonly ILogger LOGGER = LogFactory.Logger<EnumLoaderTypeSelector>();

        public EnumLoaderTypeSelector()
        {
            Selector(selector => selector
                .AddFilter(SubOfTypeFilter.OfInclude<BaseEnum>())
                .AddFilter(TypeFilter.Include(type => !(type.IsInterface || type.IsAbstract)))
                .WithHandler(OnLoadEnumTypes)
            );
        }

        private static void OnLoadEnumTypes(ICollection<Type> types)
        {
            foreach (var type in types)
            {
                var current = type;
                while (current != null && current != typeof(object))
                {
                    var method = current.GetMethod("LoadAll", BindingFlags.Public | BindingFlags.Static);
                    if (method != null)
                    {
                        try
                        {
                            method.Invoke(null, Array.Empty<object>());
                        } catch (Exception e)
                        {
                            LOGGER.LogError(e, "{Current} invoker method {Method}", current, method);
                            throw;
                        }
                        break;
                    }
                    current = current.BaseType;
                }
            }
        }
    }

}
