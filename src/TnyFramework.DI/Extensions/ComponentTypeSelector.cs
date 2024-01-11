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
using TnyFramework.Common.Scanner;
using TnyFramework.DI.Attributes;

namespace TnyFramework.DI.Extensions;

public class ComponentTypeSelector : TypeSelectorDefinition
{
    public static IList<Type> Types { get; private set; } = ImmutableList<Type>.Empty;

    public ComponentTypeSelector()
    {
        Selector(selector => selector
            .AddFilter(AttributeTypeFilter.OfInclude<ComponentAttribute>())
            .WithHandler(OnLoadComponent)
        );
    }

    private void OnLoadComponent(ICollection<Type> obj)
    {
        Types = obj.ToImmutableList();
    }
}
