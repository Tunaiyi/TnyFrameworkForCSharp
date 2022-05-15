using System;
using System.Collections.Generic;
using System.Reflection;
using ProtoBuf;
using TnyFramework.Codec.ProtobufNet.Attributes;
using TnyFramework.Codec.ProtobufNet.TypeProtobuf;
using TnyFramework.Common.Scanner;

namespace TnyFramework.Codec.ProtobufNet.Loader
{

    public class ProtobufTypeSelector : TypeSelectorDefinition
    {
        public ProtobufTypeSelector()
        {
            Selector(selector => {
                selector
                    .AddFilter(AttributeTypeFilter.OfInclude<TypeProtobufAttribute>())
                    .WithHandler(Handle);
            });
        }

        private static void Handle(ICollection<Type> types)
        {
            foreach (var type in types)
            {
                if (type.GetCustomAttribute<ProtoContractAttribute>() == null)
                {
                    throw new NullReferenceException($"{type} 未找到 {typeof(ProtoContractAttribute)}");
                }
                TypeProtobufSchemeFactory.Factory.Load(type);
            }
        }
    }

}
