// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;

namespace TnyFramework.DI.Units;

public static class UnitCollectionSpec
{
    public static UnitCollectionSpec<TUnit, TContext> Units<TUnit, TContext>()
    {
        return new UnitCollectionSpec<TUnit, TContext>();
    }
}

public class UnitCollectionSpec<TUnit, TContext> : UnitContainerSpec<TUnit, IUnitSpec<TUnit, TContext>, UnitSpec<TUnit, TContext>, TContext>,
    IUnitCollectionSpec<TUnit, TContext>
{
    public UnitCollectionSpec() : base(UnitSpec.Unit<TUnit, TContext>)
    {
    }

    public new UnitCollectionSpec<TUnit, TContext> AddDefault(TUnit unit)
    {
        base.AddDefault(unit);
        return this;
    }

    public new IUnitCollectionSpec<TUnit, TContext> AddDefaultSpec(Action<IUnitSpec<TUnit, TContext>> action)
    {
        base.AddDefaultSpec(action);
        return this;
    }

    public new UnitCollectionSpec<TUnit, TContext> AddDefault(string name, TUnit unit)
    {
        base.AddDefault(name, unit);
        return this;
    }

    public new UnitCollectionSpec<TUnit, TContext> AddDefault(UnitCreator<TUnit, TContext> value)
    {
        base.AddDefault(value);
        return this;
    }

    public new UnitCollectionSpec<TUnit, TContext> AddDefault(string name, UnitCreator<TUnit, TContext> value)
    {
        base.AddDefault(name, value);
        return this;
    }

    public new UnitCollectionSpec<TUnit, TContext> AddDefault<TImplement>() where TImplement : TUnit, new()
    {
        base.AddDefault<TImplement>();
        return this;
    }

    public new UnitCollectionSpec<TUnit, TContext> AddDefault<TImplement>(string name) where TImplement : TUnit, new()
    {
        base.AddDefault<TImplement>(name);
        return this;
    }

    public new IUnitCollectionSpec<TUnit, TContext> Add(TUnit unit)
    {
        base.Add(unit);
        return this;
    }

    public new IUnitCollectionSpec<TUnit, TContext> AddSpec(Action<IUnitSpec<TUnit, TContext>> action)
    {
        base.AddSpec(action);
        return this;
    }

    public new IUnitCollectionSpec<TUnit, TContext> Add(string name, TUnit unit)
    {
        base.Add(name, unit);
        return this;
    }

    public new IUnitCollectionSpec<TUnit, TContext> Add(UnitCreator<TUnit, TContext> value)
    {
        base.Add(value);
        return this;
    }

    public new IUnitCollectionSpec<TUnit, TContext> Add(string name, UnitCreator<TUnit, TContext> value)
    {
        base.Add(name, value);
        return this;
    }

    public new IUnitCollectionSpec<TUnit, TContext> Add<TImplement>() where TImplement : TUnit, new()
    {
        base.Add<TImplement>();
        return this;
    }

    public new IUnitCollectionSpec<TUnit, TContext> Add<TImplement>(string name) where TImplement : TUnit, new()
    {
        base.Add<TImplement>(name);
        return this;
    }

    public new IUnitCollectionSpec<TUnit, TContext> Clear()
    {
        base.Clear();
        return this;
    }
}
