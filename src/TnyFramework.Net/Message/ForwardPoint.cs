// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using ProtoBuf;
using TnyFramework.Common.Extensions;
using TnyFramework.Net.Application;
using TnyFramework.Net.Rpc;

namespace TnyFramework.Net.Message;

[ProtoContract]
public class ForwardPoint : IRpcAccessPoint
{
    private int serviceTypeId;

    private IRpcServiceType? serviceType;

    [ProtoMember(1)]
    public int ServiceTypeId {
        get => serviceTypeId;
        set {
            serviceTypeId = value;
            serviceType = RpcServiceType.ForId(serviceTypeId);
        }
    }

    [ProtoIgnore]
    public IRpcServiceType ServiceType => GetServiceType();

    [ProtoMember(2)]
    public RpcAccessId? AccessId { get; }

    public int ServerId => AccessId?.ServiceId ?? -1;

    public long Id => AccessId?.Id ?? -1;

    public long ContactId => AccessId?.Id ?? -1;

    public IContactType ContactType => ServiceType;

    public ForwardPoint()
    {
        serviceType = null!;
    }

    public ForwardPoint(IRpcServiceType serviceType)
    {
        this.serviceType = serviceType;
        ServiceTypeId = ServiceType.Id;
    }

    public ForwardPoint(IRpcServicer servicer)
    {
        serviceType = servicer.ServiceType;
        ServiceTypeId = ServiceType.Id;
        if (servicer is IRpcServicerPoint point)
        {
            AccessId = new RpcAccessId(point.Id);
        }
    }

    public ForwardPoint(RpcServiceType serviceType, long accessId)
    {
        this.serviceType = serviceType;
        serviceTypeId = serviceType.Id;
        AccessId = new RpcAccessId(accessId);
    }

    private IRpcServiceType GetServiceType()
    {
        if (serviceType == null)
        {
            return serviceType = RpcServiceType.ForId(serviceTypeId);
        }
        return serviceType;
    }

    public bool IsAppointed()
    {
        return AccessId.IsNotNull();
    }

    public int CompareTo(IRpcAccessPoint? other)
    {
        if (ReferenceEquals(this, other)) return 0;
        if (ReferenceEquals(null, other)) return 1;
        return ContactId.CompareTo(other.ContactId);
    }
}
