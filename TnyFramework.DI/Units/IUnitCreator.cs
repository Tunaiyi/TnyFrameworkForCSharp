namespace TnyFramework.DI.Units
{
    public interface IUnitCreator<out TUnit, in TContext>
    {
        TUnit Load(TContext context);
    }

    public delegate TUnit UnitCreator<out TUnit, in TContext>(TContext context);
}
