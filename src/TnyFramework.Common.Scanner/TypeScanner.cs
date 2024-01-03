// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TnyFramework.Common.Extensions;
using TnyFramework.Common.Logger;
using TnyFramework.Common.Scanner.Assemblies;
using TnyFramework.Common.Scanner.Attributes;
using TnyFramework.Common.Scanner.Exceptions;
using TnyFramework.Common.Tasks;

namespace TnyFramework.Common.Scanner
{

    /// <summary>
    /// Type 扫描器
    /// </summary>
    public class TypeScanner
    {
        private static readonly ILogger LOGGER = LogFactory.Logger<TypeScanner>();

        private readonly List<TypeSelector> selectors = new List<TypeSelector>();

        private readonly bool throwOnScanFail = true;

        public static TypeScanner Instance(bool throwOnScanFail)
        {
            return new TypeScanner(throwOnScanFail);
        }

        public static TypeScanner Instance(bool autoLoadSelector, bool throwOnScanFail)
        {
            return new TypeScanner(autoLoadSelector, throwOnScanFail);
        }

        public static TypeScanner Instance()
        {
            return new TypeScanner(true);
        }

        private TypeScanner(bool autoLoadSelector, bool throwOnScanFail = true)
            : this(autoLoadSelector)
        {
            this.throwOnScanFail = throwOnScanFail;
        }

        private TypeScanner(bool autoLoadSelector)
        {
            if (!autoLoadSelector)
                return;
            var types = AutoLoadTypes.GetTypes<AssemblyTypeSelectorAttributes>();
            foreach (var type in types)
            {
                if (typeof(ITypeSelectorDefinition).IsAssignableFrom(type))
                {
                    var value = Activator.CreateInstance(type);
                    if (value is ITypeSelectorDefinition definition)
                    {
                        selectors.AddRange(definition.Selectors);
                    }
                } else
                {
                    LOGGER.LogWarning("{Type} 未实现  {DefinitionType}", type, typeof(ITypeSelectorDefinition));
                }
            }
        }

        public TypeScanner AddSelector(IEnumerable<TypeSelector> typeSelectors)
        {
            selectors.AddRange(typeSelectors);
            return this;
        }

        public TypeScanner AddSelector(params TypeSelector[] typeSelectors)
        {
            selectors.AddRange(typeSelectors);
            return this;
        }

        private bool Filter(Assembly assembly, IEnumerable<string> assemblyNames)
        {
            var enumerable = assemblyNames as string[] ?? assemblyNames.ToArray();
            return enumerable.IsEmpty() || enumerable.Any(name => assembly.GetName().Name!.StartsWith(name));
        }

        private void DoSelect(Type type)
        {
            foreach (var selector in selectors)
            {
                selector.Select(type);
            }
        }

        /// <summary>
        /// DLL 扫描
        /// </summary>
        /// <param name="assemblyNames">扫描Assembly名</param>
        public async Task Scan(params string[] assemblyNames)
        {
            await DoScan(assemblyNames);
        }

        /// <summary>
        /// DLL 扫描
        /// </summary>
        /// <param name="assemblyNames">扫描Assembly名</param>
        public async Task Scan(IEnumerable<string> assemblyNames)
        {
            await DoScan(assemblyNames);
        }

        private void OnFail(Exception e, string message)
        {
            if (throwOnScanFail)
            {
                throw new TypeScanException(message, e);
            }
            LOGGER.LogError(e, message);
        }

        private async Task DoScan(IEnumerable<string> assemblyNames)
        {
            var names = assemblyNames as string[] ?? assemblyNames.ToArray();
            var tasks = new List<ITaskCompletionSource>();
            foreach (var assembly in AssemblyUtils.AllAssemblies)
            {
                if (!Filter(assembly, names))
                {
                    continue;
                }
                var source = new NoneTaskCompletionSource();
                ThreadPool.QueueUserWorkItem(_ => {
                    try
                    {
                        foreach (var type in assembly.GetTypes())
                        {
                            DoSelect(type);
                        }
                        source.SetResult();
                    } catch (Exception e)
                    {
                        LOGGER.LogError(e, "扫描 {assembly} Assembly 异常", assembly);
                        source.SetException(e);
                    }
                });
                tasks.Add(source);
            }
            foreach (var source in tasks)
            {
                try
                {
                    await source.Task;
                } catch (Exception e)
                {
                    OnFail(e, "Await scan exception");
                }
            }
            foreach (var selector in selectors)
            {
                selector.Selected();
            }
        }
    }

}
