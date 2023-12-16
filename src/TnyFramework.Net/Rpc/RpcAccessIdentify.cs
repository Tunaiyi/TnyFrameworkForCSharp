// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using Newtonsoft.Json;
using TnyFramework.Common.Exceptions;
using TnyFramework.Net.Base;
using TnyFramework.Net.Message;

namespace TnyFramework.Net.Rpc
{

    [JsonObject(MemberSerialization.OptIn)]
    public class RpcAccessIdentify : IRpcServicerPoint, IContact
    {
        private const long RPC_SERVER_INDEX_SIZE = 10000;

        private const long RPC_SERVICE_ID_SIZE = 100000000000L;

        private const long RPC_SERVICE_TYPE_SIZE = RPC_SERVER_INDEX_SIZE * RPC_SERVICE_ID_SIZE;

        private long id;

        [JsonIgnore]
        public IRpcServiceType ServiceType { get; set; }

        [JsonIgnore]
        public int ServerId { get; set; }

        [JsonIgnore]
        public long ContactId => ServerId;

        [JsonIgnore]
        public IContactType ContactType => ServiceType;

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
            return serviceType.Id * RPC_SERVICE_TYPE_SIZE + serverId * RPC_SERVER_INDEX_SIZE + index;
        }

        private static int ParseIndex(long id)
        {
            return (int) (id % RPC_SERVER_INDEX_SIZE);
        }

        public static int ParseServerId(long id)
        {
            return (int) (id % RPC_SERVICE_TYPE_SIZE / RPC_SERVER_INDEX_SIZE);
        }

        private static RpcServiceType ParseServiceType(long id)
        {
            return RpcServiceType.ForId((int) (id / RPC_SERVICE_TYPE_SIZE));
        }

        private void CheckIndex(int index)
        {
            if (index >= RPC_SERVER_INDEX_SIZE)
            {
                throw new IllegalArgumentException($"index {index} 必须 <= {RPC_SERVER_INDEX_SIZE}");
            }
        }

        public RpcAccessIdentify()
        {
            ServiceType = null!;
        }

        public RpcAccessIdentify(long id)
        {
            Id = id;
            ServiceType = ParseServiceType(id);
            ServerId = ParseServerId(id);
        }

        public RpcAccessIdentify(RpcServiceType serviceType, int serverId, int index)
        {
            CheckIndex(index);
            ServiceType = serviceType;
            ServerId = serverId;
            Id = FormatId(serviceType, serverId, index);
        }

        public int CompareTo(IRpcServicerPoint? other)
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
            return $"{nameof(ServiceType)}:{ServiceType},{nameof(ServerId)}:{ServerId},{nameof(Id)}:{Id}";
        }
    }

}
