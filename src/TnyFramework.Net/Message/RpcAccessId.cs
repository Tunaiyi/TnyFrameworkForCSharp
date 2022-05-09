using ProtoBuf;
using TnyFramework.Net.Rpc;

namespace TnyFramework.Net.Message
{

    [ProtoContract]
    public class RpcAccessId
    {
        [ProtoMember(1)]
        public long Id { get; set; }

        public int ServiceId => RpcAccessIdentify.ParseServerId(Id);


        public RpcAccessId()
        {
        }


        public RpcAccessId(long id)
        {
            Id = id;
        }
    }

}
