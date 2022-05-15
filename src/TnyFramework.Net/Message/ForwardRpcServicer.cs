using ProtoBuf;
using TnyFramework.Net.Rpc;

namespace TnyFramework.Net.Message
{

    [ProtoContract]
    public class ForwardRpcServicer : IRpcServicer
    {
        private int serviceTypeId;

        [ProtoMember(1)]
        public int ServiceTypeId {
            get => serviceTypeId;
            set {
                serviceTypeId = value;
                ServiceType = RpcServiceType.ForId(serviceTypeId);
            }
        }

        [ProtoIgnore]
        public IRpcServiceType ServiceType { get; private set; }

        [ProtoMember(2)]
        public RpcAccessId AccessId { get; }

        public int ServerId => AccessId.ServiceId;

        public long Id => AccessId.Id;

        public ForwardRpcServicer()
        {
        }

        public ForwardRpcServicer(IRpcServiceType serviceType)
        {
            ServiceType = serviceType;
            ServiceTypeId = ServiceType.Id;
        }

        public ForwardRpcServicer(IRpcServicer servicer)
        {
            ServiceType = servicer.ServiceType;
            ServiceTypeId = ServiceType.Id;
            AccessId = new RpcAccessId(servicer.Id);
        }

        public bool IsHasAccessId()
        {
            return AccessId != null;
        }

        public int CompareTo(IRpcServicer other)
        {
            if (ReferenceEquals(this, other)) return 0;
            return ReferenceEquals(null, other) ? 1 : Id.CompareTo(other.Id);
        }
    }

}
