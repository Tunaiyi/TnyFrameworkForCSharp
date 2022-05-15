using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace TnyFramework.Common.Scanner
{

    public abstract class TypeSelectorDefinition : ITypeSelectorDefinition
    {
        private readonly List<TypeSelector> selectors = new List<TypeSelector>();

        protected TypeSelectorDefinition()
        {
        }

        public IEnumerable<TypeSelector> Selectors => selectors.ToImmutableList();

        protected TypeSelectorDefinition Selector(Action<TypeSelector> action)
        {
            var selector = TypeSelector.Create();
            action(selector);
            selectors.Add(selector);
            return this;
        }
    }

    public interface ITypeSelectorDefinition
    {
        IEnumerable<TypeSelector> Selectors { get; }
    }

}
