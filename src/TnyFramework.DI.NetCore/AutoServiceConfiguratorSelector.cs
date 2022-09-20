using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using TnyFramework.Common.Scanner;

namespace TnyFramework.DI.NetCore
{

    public class AutoServiceConfiguratorSelector : TypeSelectorDefinition
    {
        internal static IList<Type> Types { get; private set; } = ImmutableList<Type>.Empty;

        public AutoServiceConfiguratorSelector()
        {
            Selector(selector => selector
                .AddFilter(SubOfTypeFilter.OfInclude<IAutoServiceConfigurator>())
                .AddFilter(TypeFilter.Include(type => !(type.IsInterface || type.IsAbstract)))
                .WithHandler(OnLoadTypes)
            );
        }

        private void OnLoadTypes(ICollection<Type> obj)
        {
            Types = obj.ToImmutableList();
        }
    }

}
