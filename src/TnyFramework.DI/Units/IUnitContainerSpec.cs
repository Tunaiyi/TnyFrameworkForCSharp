using System;

namespace TnyFramework.DI.Units
{

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

}
