using System;

namespace TnyFramework.DI.Units
{

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

}
