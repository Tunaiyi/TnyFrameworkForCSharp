using System;
namespace TnyFramework.DI.Container
{
    public abstract class BaseServiceInstance : IServiceInstance
    {
        private volatile IServiceFactory factory;


        protected BaseServiceInstance(IServiceFactory factory)
        {
            this.factory = factory;
        }


        public abstract object Get(IServiceProvider provider);


        protected object Create(IServiceProvider provider)
        {
            return factory.Create(provider);
        }
    }
}
