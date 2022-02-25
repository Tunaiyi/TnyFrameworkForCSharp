using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TnyFramework.Common.Exception;
namespace TnyFramework.DI.Container
{
    public class TypeServiceFactory : IServiceFactory
    {
        private readonly ConstructorInfo constructor;
        private readonly List<Type> parameterTypes;
        public Type Type { get; }


        public TypeServiceFactory(Type type)
        {
            Type = type;
            var constructors = type.GetConstructors();
            if (constructors.Length > 1)
            {
                throw new IllegalArgumentException($"{type} constructor size > 1");
            }
            constructor = constructors[0];
            var parameters = constructor.GetParameters();
            parameterTypes = parameters.Select(t => t.ParameterType).ToList();
        }


        public object Create(IServiceProvider provider)
        {
            var parameters = new List<object>();
            foreach (var parameterType in parameterTypes)
            {
                var parameter = provider.GetService(parameterType);
                parameters.Add(parameter);
            }
            return constructor.Invoke(parameters.ToArray());
        }
    }
}
