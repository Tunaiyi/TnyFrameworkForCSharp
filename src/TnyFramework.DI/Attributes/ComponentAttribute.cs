using System;

namespace TnyFramework.DI.Attributes
{

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    public class ComponentAttribute : Attribute
    {
        public string Name { get; }

        public DIMode Mode { get; }

        public bool Lazy { get; }

        public ComponentAttribute(DIMode mode) : this()
        {
        }

        public ComponentAttribute(string name = "", bool lazy = false, DIMode mode = DIMode.Singleton)
        {
            Name = name;
            Lazy = lazy;
            Mode = mode;
        }

        public bool Named()
        {
            return !string.IsNullOrEmpty(Name);
        }
    }

}
