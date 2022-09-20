﻿// Copyright (c) 2020 Tunaiyi
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
    [TypeProtobuf(1000_01_01)]
    [ProtoContract]
    public class SayContentDTO
    {
        [ProtoMember(1)]
        public long userId;

        [ProtoMember(2)]
        public string message;

        public SayContentDTO()
        {
        }

        public SayContentDTO(long userId, string message)
        {
            this.userId = userId;
            this.message = message;
        }
    }

    public static partial class DTOOutline
    {
        public static Action RegisterSayContentDTO = () => { TypeProtobufSchemeFactory.Factory.Load<SayContentDTO>(); };
    }

}
