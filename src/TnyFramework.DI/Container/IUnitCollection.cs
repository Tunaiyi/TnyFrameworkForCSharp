using System.Collections.Generic;
namespace TnyFramework.DI.Container
{
    public interface IUnitCollection
    {
    }

    public interface IUnitCollection<TUnit> : IUnitCollection, IDictionary<string, TUnit>
    {
        IDictionary<string, TUnit> UnitMap { get; }
    }
}
