using ProtoBuf;
using TnyFramework.Net.Rpc;

namespace TnyFramework.Net.Message
{

    [ProtoContract]
    public class ForwardPoint : IRpcServicerPoint
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

        public ForwardPoint()
        {
        }

        public ForwardPoint(IRpcServiceType serviceType)
        {
            ServiceType = serviceType;
            ServiceTypeId = ServiceType.Id;
        }

        public ForwardPoint(IRpcServicer servicer)
        {
            ServiceType = servicer.ServiceType;
            ServiceTypeId = ServiceType.Id;
            if (servicer is IRpcServicerPoint point)
            {
                AccessId = new RpcAccessId(point.Id);
            }
        }

        public bool IsHasAccessId()
        {
            return AccessId != null;
        }

        public int CompareTo(IRpcServicerPoint other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            var serverId = ServerId.CompareTo(other.ServerId);
            return serverId != 0 ? serverId : Id.CompareTo(other.Id);
        }
    }

}
