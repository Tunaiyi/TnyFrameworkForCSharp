using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Logging;
using TnyFramework.Common.Exceptions;
using TnyFramework.Common.Logger;

namespace TnyFramework.DI.Container
{

    public class TypeServiceFactory : IServiceFactory
    {

        private static readonly ILogger LOGGER = LogFactory.Logger<TypeServiceFactory>();
        
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
            if (constructors.Length == 0)
            {
                throw new IllegalArgumentException($"{type} constructor is empty");
            }
            constructor = constructors[0];
            var parameters = constructor.GetParameters();
            parameterTypes = parameters.Select(t => t.ParameterType).ToList();
        }

        public object Create(IServiceProvider provider)
        {
            var parameters = new List<object>();
            var index = 0;
            foreach (var parameterType in parameterTypes)
            {
                var parameter = provider.GetService(parameterType);
                if (parameter == null)
                {
                    throw new NullReferenceException($"{Type} 第 {index} 个参数 {parameterType} 未找到该对象");
                }
                parameters.Add(parameter);
                index++;
            }
            return constructor.Invoke(parameters.ToArray());
        }
    }

}
