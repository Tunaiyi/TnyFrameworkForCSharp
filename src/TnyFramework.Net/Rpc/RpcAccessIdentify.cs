using Newtonsoft.Json;
using TnyFramework.Common.Exceptions;
using TnyFramework.Net.Base;
using TnyFramework.Net.Message;

namespace TnyFramework.Net.Rpc
{

    [JsonObject(MemberSerialization.OptIn)]
    public class RpcAccessIdentify : IRpcServicerPoint, IMessager
    {
        private const long RPC_MAX_INDEX = 0xFF;

        private const long RPC_MAX_INDEX_MASK = 0xFF;

        private const long RPC_SERVER_ID_INDEX = 0x7FFFFFFF;

        private const int RPC_SERVER_ID_INDEX_SHIFT_SIZE = 8;

        private static readonly long RPC_SERVER_ID_INDEX_MASK = RPC_SERVER_ID_INDEX << RPC_SERVER_ID_INDEX_SHIFT_SIZE;

        private const long RPC_MAX_SERVER_TYPE = 0x7FFFF;

        private const int RPC_SERVICE_TYPE_MASK_SIZE = RPC_SERVER_ID_INDEX_SHIFT_SIZE + 32;

        private static readonly long RPC_SERVICE_TYPE_MASK = RPC_MAX_SERVER_TYPE << RPC_SERVICE_TYPE_MASK_SIZE;

        private long id;

        [JsonIgnore]
        public IRpcServiceType ServiceType { get; set; }

        [JsonIgnore]
        public int ServerId { get; set; }

        [JsonIgnore]
        public long MessagerId => ServerId;

        [JsonIgnore]
        public IMessagerType MessagerType => ServiceType;

        [JsonProperty("id")]
        public long Id {
            get => id;
            set {
                id = value;
                ServiceType = ParseServiceType(Id);
                ServerId = ParseServerId(Id);
            }
        }

        public int Index => ParseIndex(id);

        public static RpcAccessIdentify Parse(long id)
        {
            return new RpcAccessIdentify(id);
        }

        public static long FormatId(RpcServiceType serviceType, int serverId, int index)
        {
            return ((long) serviceType.Id << RPC_SERVICE_TYPE_MASK_SIZE) | ((long) serverId << RPC_SERVER_ID_INDEX_SHIFT_SIZE) | (uint) index;
        }

        private static int ParseIndex(long id)
        {
            return (int) (id & RPC_MAX_INDEX_MASK);
        }

        public static int ParseServerId(long id)
        {
            return (int) ((id & RPC_SERVER_ID_INDEX_MASK) >> RPC_SERVER_ID_INDEX_SHIFT_SIZE);
        }

        private static RpcServiceType ParseServiceType(long id)
        {
            return RpcServiceType.ForId((int) ((id & RPC_SERVICE_TYPE_MASK) >> RPC_SERVICE_TYPE_MASK_SIZE));
        }

        private void CheckIndex(int index)
        {
            if (index > RPC_MAX_INDEX)
            {
                throw new IllegalArgumentException($"index {index} 必须 <= {RPC_MAX_INDEX}");
            }
        }

        public RpcAccessIdentify()
        {
        }

        public RpcAccessIdentify(long id)
        {
            Id = id;
        }

        public RpcAccessIdentify(RpcServiceType serviceType, int serverId, int index)
        {
            CheckIndex(index);
            ServiceType = serviceType;
            ServerId = serverId;
            Id = FormatId(serviceType, serverId, index);
        }

        public int CompareTo(IRpcServicerPoint other)
        {
            if (ReferenceEquals(this, other)) return 0;
            return ReferenceEquals(null, other) ? 1 : Id.CompareTo(other.Id);
        }

        public int CompareTo(IRpcServicer other)
        {
            if (ReferenceEquals(this, other)) return 0;
            return ReferenceEquals(null, other) ? 1 : ServerId.CompareTo(other.ServerId);
        }

        public override string ToString()
        {
            return $"{nameof(ServiceType)}: {ServiceType}, {nameof(ServerId)}: {ServerId}, {nameof(Id)}: {Id}";
        }
    }

}
