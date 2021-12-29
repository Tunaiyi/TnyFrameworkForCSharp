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
        public static Action RegisterPlayerDTO = () => { ObjRegister.Register<PlayerDTO>(1000_03_00); };
    }
}
