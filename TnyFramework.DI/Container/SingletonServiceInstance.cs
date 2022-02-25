using System;
namespace TnyFramework.DI.Container
{
    public class SingletonServiceInstance : BaseServiceInstance
    {
        private volatile object instance;
        
        public SingletonServiceInstance(IServiceFactory factory) : base(factory)
        {
        }
        
        public override object Get(IServiceProvider provider)
        {
            if (instance != null)
            {
                return instance;
            }
            lock (this)
            {
                if (instance != null)
                {
                    return instance;
                }
                return instance = Create(provider);
            }
        }
    }
}
