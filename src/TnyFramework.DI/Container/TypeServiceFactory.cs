// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

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
                    throw new NullReferenceException($"{Type} ??? {index} ????????? {parameterType} ??????????????????");
                }
                parameters.Add(parameter);
                index++;
            }
            return constructor.Invoke(parameters.ToArray());
        }
    }

}
