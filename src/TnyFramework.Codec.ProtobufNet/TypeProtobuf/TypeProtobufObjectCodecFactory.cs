// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;

namespace TnyFramework.Codec.ProtobufNet.TypeProtobuf
{

    /// <summary>
    /// ProtobufObjectCodec 工厂
    /// </summary>
    public class TypeProtobufObjectCodecFactory : ObjectCodecFactory
    {
        public TypeProtobufObjectCodecFactory() : base(TypeProtobufMimeType.TYPE_PROTOBUF)
        {
        }

        protected override IObjectCodec<T> Create<T>()
        {
            return new TypeProtobufObjectCodec<T>(TypeProtobufSchemeFactory.Factory);
        }

        protected override IObjectCodec Create(Type type)
        {
            var makeGenericType = typeof(TypeProtobufObjectCodec<>).MakeGenericType(type);
            return (IObjectCodec) Activator.CreateInstance(makeGenericType)!;
        }
    }

}
