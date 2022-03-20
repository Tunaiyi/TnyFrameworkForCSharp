using System;
namespace TnyFramework.DI.Container
{
    public class ScopedServiceInstance : BaseServiceInstance
    {
        public ScopedServiceInstance(IServiceFactory factory) : base(factory)
        {
        }


        public override object Get(IServiceProvider provider)
        {
            return Create(provider);
        }
    }
}
