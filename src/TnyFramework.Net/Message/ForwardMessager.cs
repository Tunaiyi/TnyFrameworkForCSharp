using ProtoBuf;
using TnyFramework.Net.Base;
using MessagerTypes = TnyFramework.Net.Base.MessagerType;

namespace TnyFramework.Net.Message
{

    [ProtoContract]
    public class ForwardMessager : IMessager
    {
        private int messagerTypeId;

        [ProtoMember(1)]
        public long MessagerId { get; set; }

        [ProtoMember(2)]
        public int MessagerTypeId {
            get => messagerTypeId;
            set {
                messagerTypeId = value;
                MessagerType = MessagerTypes.ForId(messagerTypeId);
            }
        }

        public IMessagerType MessagerType { get; private set; }


        public ForwardMessager()
        {
        }


        public ForwardMessager(IMessager messager)
        {
            MessagerId = messager.MessagerId;
            MessagerType = messager.MessagerType;
            messagerTypeId = MessagerType.Id;
        }
    }

}
