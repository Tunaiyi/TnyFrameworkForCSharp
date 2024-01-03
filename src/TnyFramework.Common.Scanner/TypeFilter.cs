// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;

namespace TnyFramework.Common.Scanner
{

    public class TypeFilter : ITypeFilter
    {
        private readonly Func<Type, bool>? include;
        private readonly Func<Type, bool>? exclude;

        public static ITypeFilter Exclude(Func<Type, bool> tester)
        {
            return new TypeFilter(exclude: tester);
        }

        public static ITypeFilter Include(Func<Type, bool> tester)
        {
            return new TypeFilter(include: tester);
        }

        internal TypeFilter(Func<Type, bool>? include = null, Func<Type, bool>? exclude = null)
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
