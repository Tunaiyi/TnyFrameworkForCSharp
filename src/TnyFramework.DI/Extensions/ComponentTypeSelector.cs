using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using TnyFramework.Common.Scanner;
using TnyFramework.DI.Attributes;

namespace TnyFramework.DI.Extensions
{

    public class ComponentTypeSelector : TypeSelectorDefinition
    {
        internal static IList<Type> Types { get; private set; }

        public ComponentTypeSelector()
        {
            Selector(selector => selector
                .AddFilter(AttributeTypeFilter.OfInclude<ComponentAttribute>())
                .WithHandler(OnLoadComponent)
            );
        }

        private void OnLoadComponent(ICollection<Type> obj)
        {
            Types = obj.ToImmutableList();
        }
    }

}
