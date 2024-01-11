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
using TnyFramework.Net.Attributes;
using TnyFramework.Net.Rpc.Attributes;

namespace TnyFramework.Net.Hosting.Selectors;

public class RpcTypeSelector : TypeSelectorDefinition
{
    internal static IList<Type> Controllers { get; private set; } = ImmutableList<Type>.Empty;

    internal static IList<Type> RemoteService { get; private set; } = ImmutableList<Type>.Empty;

    public RpcTypeSelector()
    {
        Selector(selector => selector
            .AddFilter(AttributeTypeFilter.OfInclude<RpcControllerAttribute>())
            .WithHandler(OnLoadController)
        );

        Selector(selector => selector
            .AddFilter(AttributeTypeFilter.OfInclude<RpcRemoteServiceAttribute>())
            .AddFilter(TypeFilter.Include(type => type.IsInterface))
            .WithHandler(OnLoadService)
        );
    }

    private static void OnLoadController(ICollection<Type> types)
    {
        Controllers = types.ToImmutableList();
    }

    private static void OnLoadService(ICollection<Type> types)
    {
        RemoteService = types.ToImmutableList();
    }
}
