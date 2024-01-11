// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;

namespace TnyFramework.DI.Units;

public interface IUnitContainerSpec<in TUnit, out TSpec, out TContext> where TSpec : IUnitSpec<TUnit, TContext>
{
    IUnitContainerSpec<TUnit, TSpec, TContext> Add(TUnit unit);

    IUnitContainerSpec<TUnit, TSpec, TContext> Add(string name, TUnit unit);

    IUnitContainerSpec<TUnit, TSpec, TContext> Add(UnitCreator<TUnit, TContext> value);

    IUnitContainerSpec<TUnit, TSpec, TContext> Add(string name, UnitCreator<TUnit, TContext> value);

    IUnitContainerSpec<TUnit, TSpec, TContext> Add<TImplement>() where TImplement : TUnit, new();

    IUnitContainerSpec<TUnit, TSpec, TContext> Add<TImplement>(string name) where TImplement : TUnit, new();

    IUnitContainerSpec<TUnit, TSpec, TContext> AddSpec(Action<TSpec> action);

    IUnitContainerSpec<TUnit, TSpec, TContext> Clear();
}
