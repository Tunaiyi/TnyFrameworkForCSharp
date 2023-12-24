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
using System.Linq;
using System.Reflection;
using TnyFramework.DI.Attributes;

namespace TnyFramework.DI.Extensions
{

    public static class ServiceCollectionUtils
    {
        public static IList<Type> FindRegisterTypes(Type? checkType, bool force = true)
        {
            if (checkType == null || checkType == typeof(object))
            {
                return ImmutableList<Type>.Empty;
            }
            var registeredTypes = new List<Type>();
            if (force || checkType.GetCustomAttribute<ServiceInterfaceAttribute>() != null)
            {
                registeredTypes.Add(checkType);
            }
            var baseType = checkType.BaseType;
            if (baseType != null)
            {
                registeredTypes.AddRange(FindRegisterTypes(baseType, force));
            }
            var interfaces = checkType.GetInterfaces();
            foreach (var type in interfaces)
            {
                registeredTypes.AddRange(FindRegisterTypes(type, force));
            }
            return registeredTypes.Distinct().ToImmutableList();
        }
    }

}
