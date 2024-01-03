// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.Linq;

namespace TnyFramework.Common.Scanner.Assemblies
{

    public static class TypeExtensions
    {
        /// <summary>
        /// 判断指定的类型 <paramref name="type"/> 是否是指定泛型类型的子类型，或实现了指定泛型接口。
        /// </summary>
        /// <param name="type">需要测试的类型。</param>
        /// <param name="generic">泛型接口类型，传入 typeof(IXxx&lt;&gt;)</param>
        /// <returns>如果是泛型接口的子类型，则返回 true，否则返回 false。</returns>
        public static bool IsAssignableFromGeneric(this Type type, Type generic)
        {
            return CheckAssignableFromGeneric(type, generic);
        }

        private static bool CheckAssignableFromGeneric(Type type, Type generic, Func<Type, bool>? tester = null)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            if (generic == null)
                throw new ArgumentNullException(nameof(generic));
            if (type == typeof(object))
                return false;
            if (tester == null)
            {
                tester = checkType => generic == (checkType.IsGenericType ? checkType.GetGenericTypeDefinition() : checkType);
            }
            if (tester(type))
            {
                return true;
            }
            if (type.GetInterfaces().Any(tester))
                return true;
            return type.BaseType != null && CheckAssignableFromGeneric(type.BaseType, generic, tester);
        }
    }

}
