#region

using System;
using System.Collections.Generic;
using System.Reflection;

#endregion

namespace TnyFramework.Common.Assemblies
{

    public static class AssemblyUtils
    {
        private static volatile IList<Assembly> _ALL_ASSEMBLIES;


        public static IList<Assembly> LoadAllAssemblies(Func<Assembly, bool> filter = null)
        {
            if (_ALL_ASSEMBLIES != null)
            {
                return _ALL_ASSEMBLIES;
            }
            lock (typeof(AssemblyUtils))
            {
                if (_ALL_ASSEMBLIES != null)
                {
                    return _ALL_ASSEMBLIES;
                }
                _ALL_ASSEMBLIES = LoadAllAssemblies(AppDomain.CurrentDomain.GetAssemblies(), filter);
            }
            return _ALL_ASSEMBLIES;
        }


        public static IList<Assembly> LoadAllAssemblies(ICollection<Assembly> assemblies, Func<Assembly, bool> filter = null)
        {
            var all = new List<Assembly>();
            foreach (var root in assemblies)
            {
                DoLoadAllAssemblies(root, all, filter);
            }
            return all;
        }


        private static void DoLoadAllAssemblies(Assembly root, ICollection<Assembly> all, Func<Assembly, bool> filter)
        {
            if (!DoAddAssembly(root, all, filter))
                return;
            foreach (var referencedAssembly in root.GetReferencedAssemblies())
            {
                var ass = Assembly.Load(referencedAssembly);
                if (DoAddAssembly(ass, all, filter))
                {
                    DoLoadAllAssemblies(ass, all, filter);
                }
            }
        }


        private static bool DoAddAssembly(Assembly assembly, ICollection<Assembly> all, Func<Assembly, bool> filter)
        {
            if (filter != null && !filter.Invoke(assembly))
            {
                return false;
            }
            if (all.Contains(assembly))
                return false;
            all.Add(assembly);
            return true;
        }
    }

}
