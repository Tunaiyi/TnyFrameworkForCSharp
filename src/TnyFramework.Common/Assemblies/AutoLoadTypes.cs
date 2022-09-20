// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using Castle.Core.Internal;
using Microsoft.Extensions.Logging;
using TnyFramework.Common.Assemblies.Attributes;
using TnyFramework.Common.Logger;

namespace TnyFramework.Common.Assemblies
{

    public static class AutoLoadTypes
    {
        private static readonly ILogger LOGGER = LogFactory.Logger(typeof(AutoLoadTypes));

        private static bool _INIT;

        private static IDictionary<Type, ISet<Type>> _TYPES_MAP = ImmutableDictionary<Type, ISet<Type>>.Empty;

        private static readonly IList<string> ASSEMBLY_FILTERS = new List<string>();

        private static IList<string> AssemblyFilters { get; set; }

        public static void NameFilter(params string[] assemblyFilters)
        {
            foreach (var assemblyFilter in assemblyFilters)
            {
                ASSEMBLY_FILTERS.Add(assemblyFilter);
            }
            AssemblyFilters = ASSEMBLY_FILTERS.ToImmutableList();
        }

        private static bool Filter(Assembly ab)
        {
            return AssemblyFilters.IsNullOrEmpty() || (
                from assemblyFilter in AssemblyFilters
                let name = ab.GetName()
                where name.Name.StartsWith(assemblyFilter)
                select assemblyFilter).Any();
        }

        private static void Init()
        {
            if (_INIT)
            {
                return;
            }
            lock (typeof(AutoLoadTypes))
            {
                if (_INIT)
                {
                    return;
                }
                var assemblies = AssemblyUtils.AllAssemblies;
                var dictionary = new ConcurrentDictionary<Type, ISet<Type>>();
                foreach (var assembly in assemblies)
                {
                    if (!Filter(assembly))
                        continue;
                    var autoLoadAttributes = assembly.GetCustomAttributes<AssemblyAutoLoadAttributes>();
                    var name = assembly.GetName();
                    LOGGER.LogDebug("Auto Load Assembly : {Assembly} [{Version}] ", name.Name, name.Version);
                    foreach (var attribute in autoLoadAttributes)
                    {
                        LOGGER.LogDebug("Auto Load Assembly : {Assembly} [{Version}] with {AttributeType} [{Types}]", name.Name, name.Version,
                            attribute.GetType(), string.Join(",", attribute.LoadClasses));
                        var types = dictionary.GetOrAdd(attribute.GetType(), _ => new HashSet<Type>());
                        foreach (var attributeLoadClass in attribute.LoadClasses)
                        {
                            types.Add(assembly.GetType(attributeLoadClass));
                        }
                    }
                }
                _TYPES_MAP = dictionary.ToImmutableDictionary();
                _INIT = true;
            }
        }

        public static ISet<Type> GetTypes<T>() where T : AssemblyAutoLoadAttributes
        {
            return GetTypes(typeof(T));
        }

        public static ISet<Type> GetTypes(Type type)
        {
            if (!_INIT)
            {
                Init();
            }
            return _TYPES_MAP.TryGetValue(type, out var types) ? types : ImmutableHashSet<Type>.Empty;
        }
    }

}
