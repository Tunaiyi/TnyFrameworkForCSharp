//
//  文件名称：RelayFramePackDTO.cs
//  简   述：
//  创建标识：lrg 2021/7/29
//

#region

using System;
using ProtoBuf;
using TnyFramework.Net.TypeProtobuf;

#endregion

namespace TnyFramework.Net.Demo.DTO
{

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
        public static Action RegisterSayContentDTO = () => { TypeProtobufObjectFactory.Factory.Register<SayContentDTO>(1000_01_01); };
    }

}
