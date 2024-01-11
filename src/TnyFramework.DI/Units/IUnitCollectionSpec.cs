// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;

namespace TnyFramework.DI.Units;

public interface IUnitCollectionSpec<in TUnit, out TContext>
{
    int Size { get; }

    IUnitCollectionSpec<TUnit, TContext> Add(TUnit unit);

    IUnitCollectionSpec<TUnit, TContext> AddSpec(Action<IUnitSpec<TUnit, TContext>> action);

    IUnitCollectionSpec<TUnit, TContext> Add(string name, TUnit unit);

    IUnitCollectionSpec<TUnit, TContext> Add(UnitCreator<TUnit, TContext> value);

    IUnitCollectionSpec<TUnit, TContext> Add(string name, UnitCreator<TUnit, TContext> value);

    IUnitCollectionSpec<TUnit, TContext> Add<TImplement>() where TImplement : TUnit, new();

    IUnitCollectionSpec<TUnit, TContext> Add<TImplement>(string name) where TImplement : TUnit, new();

    IUnitCollectionSpec<TUnit, TContext> Clear();
}
