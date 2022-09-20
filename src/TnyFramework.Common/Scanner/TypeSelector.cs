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
using System.Threading;
using Microsoft.Extensions.Logging;
using TnyFramework.Common.Logger;

namespace TnyFramework.Common.Scanner
{

    public class TypeSelector
    {
        private static readonly ILogger LOGGER = LogFactory.Logger<TypeSelector>();

        private readonly List<ITypeFilter> filters = new List<ITypeFilter>();

        public IList<Type> Types { get; private set; } = ImmutableList<Type>.Empty;

        private ConcurrentDictionary<Thread, ISet<Type>> mapClass = new ConcurrentDictionary<Thread, ISet<Type>>();

        private ITypeSelectedHandler handler;

        public static TypeSelector Create()
        {
            return new TypeSelector();
        }

        private TypeSelector()
        {
        }

        public TypeSelector WithHandler(Action<ICollection<Type>> action)
        {
            handler = new TypeSelectedHandler(action);
            return this;
        }

        public TypeSelector WithHandler(ITypeSelectedHandler value)
        {
            handler = value;
            return this;
        }

        public TypeSelector AddFilter(params ITypeFilter[] value)
        {
            filters.AddRange(value);
            return this;
        }

        public TypeSelector AddFilter(IEnumerable<ITypeFilter> value)
        {
            filters.AddRange(value);
            return this;
        }

        internal bool Select(Type type)
        {
            if (!Filter(type))
                return false;
            try
            {
                var current = Thread.CurrentThread;
                mapClass.GetOrAdd(current, thread => new HashSet<Type>())
                    .Add(type);
                return true;
            } catch (Exception e)
            {
                LOGGER.LogError(e, "添加用户自定义视图类错误 找不到此类的{type}文件", type);
            }
            return false;
        }

        private bool Filter(Type type)
        {
            return filters.All(fileFilter => fileFilter.Include(type) && !fileFilter.Exclude(type));
        }

        internal void Selected()
        {
            ISet<Type> classes = new HashSet<Type>();
            foreach (var mapClasses in mapClass.Values)
            {
                foreach (var value in mapClasses)
                {
                    classes.Add(value);
                }
            }
            Types = classes.Distinct().ToImmutableList();
            mapClass.Clear();
            mapClass = null;
            handler?.Handle(Types);
        }

        internal void Clear()
        {
            Types = ImmutableList<Type>.Empty;
        }
    }

}
