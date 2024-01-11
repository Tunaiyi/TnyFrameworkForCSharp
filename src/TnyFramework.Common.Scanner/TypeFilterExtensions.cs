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

namespace TnyFramework.Common.Scanner;

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
