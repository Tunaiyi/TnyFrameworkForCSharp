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

namespace TnyFramework.Common.Scanner;

public abstract class TypeSelectorDefinition : ITypeSelectorDefinition
{
    private readonly List<TypeSelector> selectors = new List<TypeSelector>();

    protected TypeSelectorDefinition()
    {
    }

    public IEnumerable<TypeSelector> Selectors => selectors.ToImmutableList();

    protected TypeSelectorDefinition Selector(Action<TypeSelector> action)
    {
        var selector = TypeSelector.Create();
        action(selector);
        selectors.Add(selector);
        return this;
    }
}

public interface ITypeSelectorDefinition
{
    IEnumerable<TypeSelector> Selectors { get; }
}
