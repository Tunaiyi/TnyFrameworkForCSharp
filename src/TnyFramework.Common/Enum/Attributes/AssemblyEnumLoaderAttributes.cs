using System;
using TnyFramework.Common.Assemblies.Attributes;

namespace TnyFramework.Common.Enum.Attributes
{

    [AttributeUsage(AttributeTargets.Assembly)]
    public class AssemblyEnumLoaderAttributes : AssemblyAutoLoadAttributes
    {
        public AssemblyEnumLoaderAttributes(string loadClass, params string[] loadClasses) : base(loadClass, loadClasses)
        {
        }
    }

}
