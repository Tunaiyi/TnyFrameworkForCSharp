using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TnyFramework.Common.Scanner
{

    public static class TypeFilterExtensions
    {
        public static bool MatchAttributes(this Type type, params Type[] types)
        {

            return DoMatchAttributes(type, types);
        }

        public static bool MatchAttributes(this Type type, IEnumerable<Type> types)
        {

            return DoMatchAttributes(type, types);
        }

        private static bool DoMatchAttributes(MemberInfo type, IEnumerable<Type> types)
        {
            return types.Any(testType => type.GetCustomAttribute(testType) != null);
        }

        public static bool MatchSuper(this Type type, params Type[] types)
        {
            return DoMatchSuper(type, types);
        }

        public static bool MatchSuper(this Type type, IEnumerable<Type> types)
        {
            return DoMatchSuper(type, types);
        }

        private static bool DoMatchSuper(Type type, IEnumerable<Type> types)
        {
            return types.Any(testType => testType.IsAssignableFrom(type));
        }
    }

}
