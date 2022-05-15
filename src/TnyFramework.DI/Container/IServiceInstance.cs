using System;

namespace TnyFramework.DI.Container
{

    public interface IServiceInstance
    {
        object Get(IServiceProvider provider);
    }

}
