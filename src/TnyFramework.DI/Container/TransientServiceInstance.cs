using System;

namespace TnyFramework.DI.Container
{

    public class TransientServiceInstance : BaseServiceInstance
    {
        public TransientServiceInstance(IServiceFactory factory) : base(factory)
        {
        }

        public override object Get(IServiceProvider provider)
        {
            return Create(provider);
        }
    }

}
