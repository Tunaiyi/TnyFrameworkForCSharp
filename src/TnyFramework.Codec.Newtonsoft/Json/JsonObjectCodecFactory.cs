// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using Newtonsoft.Json;

namespace TnyFramework.Codec.Newtonsoft.Json
{

    /// <summary>
    /// ProtobufObjectCodec 工厂
    /// </summary>
    public class JsonObjectCodecFactory : ObjectCodecFactory
    {
        private readonly JsonSerializerSettings formatting;

        public JsonObjectCodecFactory() : base(JsonMimeType.JSON)
        {
            formatting = null;
        }

        public JsonObjectCodecFactory(JsonSerializerSettings formatting) : base(JsonMimeType.JSON)
        {
            this.formatting = formatting;
        }

        protected override IObjectCodec<T> Create<T>()
        {
            return new JsonObjectCodec<T>(formatting);
        }

        protected override IObjectCodec Create(Type type)
        {
            var makeGenericType = typeof(JsonObjectCodec<>).MakeGenericType(type);
            return (IObjectCodec) Activator.CreateInstance(makeGenericType, formatting);
        }
    }

}
