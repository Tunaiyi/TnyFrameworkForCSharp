using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using Castle.Core.Internal;

namespace TnyFramework.Common.Assemblies
{

    public static class AssemblyUtils
    {
        private static volatile IList<Assembly> _ALL_ASSEMBLIES;

        public static IEnumerable<Assembly> AllAssemblies => _ALL_ASSEMBLIES ?? ImmutableList<Assembly>.Empty;

        public static IList<Assembly> LoadAllAssemblies(params string[] startWiths)
        {
            return LoadAllAssemblies(ab => startWiths.IsNullOrEmpty() || (
                from assemblyFilter in startWiths
                let name = ab.GetName()
                where name.Name.StartsWith(assemblyFilter)
                select assemblyFilter).Any());
        }

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

        public static IList<Assembly> LoadAllAssemblies(IEnumerable<Assembly> assemblies, Func<Assembly, bool> filter = null)
        {
            var all = new HashSet<Assembly>();
            foreach (var root in assemblies)
            {
                DoLoadAllAssemblies(root, all, filter);
            }
            return all.ToImmutableList();
        }

        private static void DoLoadAllAssemblies(Assembly root, ICollection<Assembly> all, Func<Assembly, bool> filter)
        {
            foreach (var referencedAssembly in root.GetReferencedAssemblies())
            {
                var ass = Assembly.Load(referencedAssembly);
                if (!Filter(ass, filter))
                {
                    continue;
                }
                if (all.Contains(ass))
                {
                    continue;
                }
                DoLoadAllAssemblies(ass, all, filter);
            }
            DoAddAssembly(root, all, filter);
        }

        private static bool Filter(Assembly assembly, Func<Assembly, bool> filter)
        {
            return filter == null || filter.Invoke(assembly);
        }

        private static void DoAddAssembly(Assembly assembly, ICollection<Assembly> all, Func<Assembly, bool> filter)
        {
            if (!Filter(assembly, filter))
            {
                return;
            }
            if (all.Contains(assembly))
                return;
            all.Add(assembly);
        }
    }

}
