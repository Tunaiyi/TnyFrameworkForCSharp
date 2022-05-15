using System;

namespace TnyFramework.Common.Scanner
{

    public class TypeFilter : ITypeFilter
    {
        private readonly Func<Type, bool> include;
        private readonly Func<Type, bool> exclude;

        public static ITypeFilter Exclude(Func<Type, bool> tester)
        {
            return new TypeFilter(exclude: tester);
        }

        public static ITypeFilter Include(Func<Type, bool> tester)
        {
            return new TypeFilter(include: tester);
        }

        internal TypeFilter(Func<Type, bool> include = null, Func<Type, bool> exclude = null)
        {
            this.include = include;
            this.exclude = exclude;
        }

        public bool Include(Type type)
        {
            return include == null || include(type);
        }

        public bool Exclude(Type type)
        {
            return exclude != null && exclude(type);
        }
    }

    public interface ITypeFilter
    {
        bool Include(Type type);

        bool Exclude(Type type);
    }

}
