﻿using System;
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
