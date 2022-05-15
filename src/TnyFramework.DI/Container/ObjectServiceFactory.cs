using System;

namespace TnyFramework.DI.Container
{

    public class ObjectServiceFactory : IServiceFactory
    {
        private readonly object instance;

        public ObjectServiceFactory(object instance)
        {
            this.instance = instance;
        }

        public object Create(IServiceProvider provider)
        {
            return instance;
        }
    }

}
