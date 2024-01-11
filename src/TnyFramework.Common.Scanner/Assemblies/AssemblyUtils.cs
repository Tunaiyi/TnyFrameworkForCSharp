// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;
using Microsoft.Extensions.Logging;
using TnyFramework.Common.Extensions;
using TnyFramework.Common.Logger;

namespace TnyFramework.Common.Scanner.Assemblies;

public static class AssemblyUtils
{
    private static readonly ILogger LOGGER = LogFactory.Logger(nameof(AssemblyUtils));

    private static volatile IList<Assembly>? _ALL_ASSEMBLIES;

    public static IEnumerable<Assembly> AllAssemblies => _ALL_ASSEMBLIES ?? ImmutableList<Assembly>.Empty;

    public static IList<Assembly> LoadAllAssemblies(params string[] startWiths)
    {
        return LoadAllAssemblies(ab => {
            var name = ab.GetName();
            if (startWiths.IsNullOrEmpty())
            {
                // LOGGER.LogInformation("Load assembly {Assembly} with name {Name}", ab, name);
                return true;
            }
            foreach (var startWith in startWiths)
            {
                if (!name.Name!.StartsWith(startWith))
                {
                    continue;
                }
                LOGGER.LogInformation("Load assembly with name {Name} with start with {StartWith}", name, startWith);
                return true;
            }
            // LOGGER.LogInformation("Skip assembly {Assembly} with name {Name}", ab, name);
            return false;
        });
    }

    public static IList<Assembly> LoadAllAssemblies(Func<Assembly, bool>? filter = null)
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
            return _ALL_ASSEMBLIES = LoadAllAssemblies(AppDomain.CurrentDomain.GetAssemblies(), filter);
        }
    }

    public static IList<Assembly> LoadAllAssemblies(IEnumerable<Assembly> assemblies, Func<Assembly, bool>? filter = null)
    {
        var all = new HashSet<Assembly>();
        foreach (var root in assemblies)
        {
            DoLoadAllAssemblies(root, all, filter);
        }
        return all.ToImmutableList();
    }

    private static void DoLoadAllAssemblies(Assembly root, ICollection<Assembly> all, Func<Assembly, bool>? filter)
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

    private static bool Filter(Assembly assembly, Func<Assembly, bool>? filter)
    {
        return filter == null || filter.Invoke(assembly);
    }

    private static void DoAddAssembly(Assembly assembly, ICollection<Assembly> all, Func<Assembly, bool>? filter)
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
