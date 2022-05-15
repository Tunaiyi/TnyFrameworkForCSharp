using System;

namespace TnyFramework.DI.Container
{

    public interface IUnitFactory
    {
    }

    public interface IUnitFactory<out TInstance> : IUnitFactory
    {
        TInstance Create(IServiceProvider provider);
    }

    public interface IObjectUnitFactory : IUnitFactory
    {
        object Create(IServiceProvider provider);
    }

}
