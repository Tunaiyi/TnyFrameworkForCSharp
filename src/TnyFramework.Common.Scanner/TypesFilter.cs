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
using CollectionExtensions = TnyFramework.Common.Extensions.CollectionExtensions;

namespace TnyFramework.Common.Scanner;

/// <summary>
/// 通用筛选器
/// </summary>
public abstract class TypesFilter<TFilter> : ITypeFilter
    where TFilter : TypesFilter<TFilter>, new()

{
    private ISet<Type> includes = ImmutableHashSet<Type>.Empty;
    private ISet<Type> excludes = ImmutableHashSet<Type>.Empty;
    private readonly Func<Type, ISet<Type>, bool> tester;

    public static ITypeFilter OfInclude<T>()
    {
        return OfInclude(typeof(T));
    }

    public static ITypeFilter OfInclude(params Type[] includes)
    {
        return new TFilter {
            includes = includes.ToImmutableHashSet()
        };
    }

    public static ITypeFilter OfInclude(IEnumerable<Type> includes)
    {
        return new TFilter {
            includes = includes.ToImmutableHashSet()
        };
    }

    public static ITypeFilter OfExclude<T>()
    {
        return OfExclude(typeof(T));
    }

    public static ITypeFilter OfExclude(params Type[] excludes)
    {
        return new TFilter {
            excludes = excludes.ToImmutableHashSet()
        };
    }

    public static ITypeFilter OfExclude(IEnumerable<Type> excludes)
    {
        return new TFilter {
            excludes = excludes.ToImmutableHashSet()
        };
    }

    public static ITypeFilter Of(IEnumerable<Type> includes, IEnumerable<Type> excludes)
    {
        return new TFilter {
            includes = includes.ToImmutableHashSet(),
            excludes = excludes.ToImmutableHashSet()
        };
    }

    protected TypesFilter(Func<Type, ISet<Type>, bool> tester)
    {
        this.tester = tester;
    }

    public bool Include(Type type)
    {
        return CollectionExtensions.IsNullOrEmpty(includes) || tester(type, includes);
    }

    public bool Exclude(Type type)
    {
        return !CollectionExtensions.IsNullOrEmpty(excludes) && tester(type, excludes);
    }
}
