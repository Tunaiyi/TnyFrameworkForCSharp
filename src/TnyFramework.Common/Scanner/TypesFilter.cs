using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Castle.Core.Internal;

namespace TnyFramework.Common.Scanner
{

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
                includes = includes?.ToImmutableHashSet(),
                excludes = excludes?.ToImmutableHashSet()
            };
        }

        protected TypesFilter(Func<Type, ISet<Type>, bool> tester)
        {
            this.tester = tester;
        }

        public bool Include(Type type)
        {
            return includes.IsNullOrEmpty() || tester(type, includes);
        }

        public bool Exclude(Type type)
        {
            return !excludes.IsNullOrEmpty() && tester(type, excludes);
        }
    }

}
