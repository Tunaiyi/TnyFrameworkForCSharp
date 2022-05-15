using System;

namespace TnyFramework.DI.Container
{

    public class FuncServiceFactory<TInstance> : IServiceFactory
    {
        private readonly Func<IServiceProvider, TInstance> factory;

        public FuncServiceFactory(Func<IServiceProvider, TInstance> factory)
        {
            this.factory = factory;
        }

        public object Create(IServiceProvider provider)
        {
            return factory(provider);
        }
    }

}
