using System;
using TnyFramework.Common.Assemblies.Attributes;

namespace TnyFramework.Common.Scanner.Attributes
{

    [AttributeUsage(AttributeTargets.Assembly)]
    public class AssemblyTypeSelectorAttributes : AssemblyAutoLoadAttributes
    {
        public AssemblyTypeSelectorAttributes(string loadClass, params string[] loadClasses) : base(loadClass, loadClasses)
        {
        }
    }

}
