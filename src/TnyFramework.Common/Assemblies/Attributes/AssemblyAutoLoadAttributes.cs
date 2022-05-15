using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace TnyFramework.Common.Assemblies.Attributes
{

    [AttributeUsage(AttributeTargets.Assembly)]
    public abstract class AssemblyAutoLoadAttributes : System.Attribute
    {
        public IList<string> LoadClasses { get; }

        protected AssemblyAutoLoadAttributes(string loadClass, params string[] loadClasses)
        {
            var classes = new List<string> {loadClass};
            classes.AddRange(loadClasses);
            LoadClasses = classes.ToImmutableList();
        }
    }

}
