using System;

namespace TnyFramework.DI.Attributes
{

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    public class ComponentAttribute : Attribute
    {
        public string Name { get; }

        public DIMode Mode { get; }


        public ComponentAttribute(DIMode mode) : this("", mode)
        {
        }


        public ComponentAttribute(string name = "", DIMode mode = DIMode.Singleton)
        {
            Name = name;
            Mode = mode;
        }


        public bool Named()
        {
            return !string.IsNullOrEmpty(Name);
        }
    }

}
