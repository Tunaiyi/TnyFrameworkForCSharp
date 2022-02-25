using System;
namespace TnyFramework.DI.Container
{
    public class ObjectServiceInstance<T> : IServiceInstance
    {
        private readonly T instance;


        public ObjectServiceInstance(T instance)
        {
            if (instance == null)
            {
                throw new NullReferenceException();
            }
            this.instance = instance;
        }


        public object Get(IServiceProvider provider)
        {
            return instance;
        }
    }
}
