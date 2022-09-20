// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using ProtoBuf;
using TnyFramework.Codec;
using TnyFramework.Codec.Attributes;
using TnyFramework.Codec.ProtobufNet.Attributes;
using TnyFramework.Codec.ProtobufNet.TypeProtobuf;

namespace TnyFramework.Net.Demo.DTO
{

    [Codable(MimeTypes.PROTOBUF_TYPE)]
    [TypeProtobuf(1000_03_00)]
    [ProtoContract]
    public class PlayerDTO
    {
        [ProtoMember(1)]
        public long id;

        [ProtoMember(2)]
        public string name;

        [ProtoMember(3)]
        public int age;
    }

    public static partial class DTOOutline
    {
        public static Action RegisterPlayerDTO = () => { TypeProtobufSchemeFactory.Factory.Load<PlayerDTO>(); };
    }

}
