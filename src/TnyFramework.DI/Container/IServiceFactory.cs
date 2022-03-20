using System;
namespace TnyFramework.DI.Container
{
    public interface IServiceFactory
    {
        object Create(IServiceProvider provider);
    }
}
