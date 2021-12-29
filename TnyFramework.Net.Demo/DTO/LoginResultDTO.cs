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
    public class LoginResultDTO
    {
        [ProtoMember(1)]
        public long userId;

        [ProtoMember(2)]
        public string message;
    }

    public static partial class DTOOutline
    {
        public static Action RegisterLoginResultDTO = () => { ObjRegister.Register<LoginResultDTO>(1000_00_00); };
    }
}
