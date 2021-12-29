//
//  文件名称：RelayFramePackDTO.cs
//  简   述：
//  创建标识：lrg 2021/7/29
//

using System;
using GF.Base;
using ProtoBuf;
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
    }

    public static partial class DTOOutline
    {
        public static Action RegisterLoginDTO = () => { ObjRegister.Register<LoginDTO>(1000_01_00); };
    }
}
