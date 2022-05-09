//
//  文件名称：RelayFramePackDTO.cs
//  简   述：
//  创建标识：lrg 2021/7/29
//

using System;
using ProtoBuf;
using TnyFramework.Codec;
using TnyFramework.Codec.Attributes;
using TnyFramework.Codec.ProtobufNet.Attributes;
using TnyFramework.Codec.ProtobufNet.TypeProtobuf;

namespace TnyFramework.Net.Demo.DTO
{

    [Codable(MimeTypes.PROTOBUF_TYPE)]
    [TypeProtobuf(1000_01_00)]
    [ProtoContract]
    public class LoginDTO
    {
        [ProtoMember(1)]
        public long userId;

        [ProtoMember(2)]
        public string message;

        [ProtoMember(3)]
        public long certId;


        public LoginDTO()
        {
        }


        public LoginDTO(long certId, long userId, string message)
        {
            this.userId = userId;
            this.message = message;
            this.certId = certId;
        }
    }

    public static partial class DTOOutline
    {
        public static Action RegisterLoginDTO = () => { TypeProtobufSchemeFactory.Factory.Load<LoginDTO>(); };
    }

}
