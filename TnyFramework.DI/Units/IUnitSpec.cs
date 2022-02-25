namespace TnyFramework.DI.Units
{
    public interface IUnitSpec<in TUnit, out TContext>
    {
        IUnitSpec<TUnit, TContext> Unit(TUnit value);

        IUnitSpec<TUnit, TContext> Creator<TImplement>() where TImplement : TUnit, new();

        IUnitSpec<TUnit, TContext> Creator(UnitCreator<TUnit, TContext> value);
    }
}
