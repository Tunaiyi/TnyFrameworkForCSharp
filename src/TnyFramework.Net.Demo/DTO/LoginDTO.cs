//
//  文件名称：RelayFramePackDTO.cs
//  简   述：
//  创建标识：lrg 2021/7/29
//

using System;
using ProtoBuf;
using TnyFramework.Net.TypeProtobuf;

namespace TnyFramework.Net.Demo.DTO
{
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
        public static Action RegisterLoginDTO = () => { TypeProtobufObjectFactory.Factory.Register<LoginDTO>(1000_01_00); };
    }
}
