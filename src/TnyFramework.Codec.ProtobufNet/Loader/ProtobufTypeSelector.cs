// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.Collections.Generic;
using System.Reflection;
using ProtoBuf;
using TnyFramework.Codec.ProtobufNet.Attributes;
using TnyFramework.Codec.ProtobufNet.TypeProtobuf;
using TnyFramework.Common.Scanner;

namespace TnyFramework.Codec.ProtobufNet.Loader;

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
