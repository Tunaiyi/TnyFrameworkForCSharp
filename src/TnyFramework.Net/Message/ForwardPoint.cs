// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

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
