// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using ProtoBuf;
using TnyFramework.Codec.Attributes;
using TnyFramework.Codec.ProtobufNet.Attributes;
using TnyFramework.Codec.ProtobufNet.TypeProtobuf;
using TnyFramework.Net.Rpc;

namespace TnyFramework.Net.Message;

[Codable(ProtobufMimeType.PROTOBUF_TYPE)]
[TypeProtobuf(MessageHeaderKeys.RPC_FORWARD_TYPE_PROTO)]
[ProtoContract]
public class RpcForwardHeader : SelfMessageHeader<RpcForwardHeader>
{
    /// <summary>
    /// 请求服务
    /// </summary>
    [ProtoMember(2)]
    public ForwardPoint? From { get; set; }

    /// <summary>
    /// 请求发送者
    /// </summary>
    [ProtoMember(3)]
    public ForwardContact? Sender { get; set; }

    /// <summary>
    /// 目标服务
    /// </summary>
    [ProtoMember(4)]
    public ForwardPoint? To { get; set; }

    /// <summary>
    /// 目标接受者
    /// </summary>
    [ProtoMember(5)]
    public ForwardContact? Receiver { get; set; }

    /// <summary>
    /// 发生转发者
    /// </summary>
    [ProtoMember(6, IsPacked = true)]
    public ForwardPoint? FromForwarder { get; set; }

    /// <summary>
    /// 目标转发者
    /// </summary>
    [ProtoMember(7, IsPacked = true)]
    public ForwardPoint? ToForwarder { get; set; }

    public RpcForwardHeader() : base(MessageHeaderKeys.RPC_FORWARD_HEADER)
    {
    }

    public RpcForwardHeader SetFrom(IRpcServicer fromService)
    {
        From = ToForwardRpcServicer(fromService);
        return this;
    }

    public RpcForwardHeader SetSender(IContact sender)
    {
        Sender = ToForwardContact(sender);
        return this;
    }

    public RpcForwardHeader SetTo(IRpcServicer toServicer)
    {
        To = ToForwardRpcServicer(toServicer);
        return this;
    }

    public RpcForwardHeader SetTo(IRpcServiceType serviceType)
    {
        To = ToForwardRpcServicer(new ForwardPoint(serviceType));
        return this;
    }

    public RpcForwardHeader SetReceiver(IContact receiver)
    {
        Receiver = ToForwardContact(receiver);
        return this;
    }

    public RpcForwardHeader SetFromForwarder(IRpcServicer fromService)
    {
        FromForwarder = ToForwardRpcServicer(fromService);
        return this;
    }

    public RpcForwardHeader SetToForwarder(IRpcServicer toService)
    {
        ToForwarder = ToForwardRpcServicer(toService);
        return this;
    }

    public RpcForwardHeader SetFromForwarder(IRpcServicerPoint fromPoint)
    {
        FromForwarder = ToForwardRpcServicer(fromPoint);
        return this;
    }

    public RpcForwardHeader SetToForwarder(IRpcServicerPoint toPoint)
    {
        ToForwarder = ToForwardRpcServicer(toPoint);
        return this;
    }

    private static ForwardPoint ToForwardRpcServicer(IRpcServicer rpcServicer)
    {
        switch (rpcServicer)
        {
            case null:
                return null!;
            case ForwardPoint value:
                return value;
            default:
                return new ForwardPoint(rpcServicer);
        }
    }

    private static ForwardContact? ToForwardContact(IContact contact)
    {
        switch (contact)
        {
            case null:
                return null;
            case ForwardContact value:
                return value;
            default:
                return new ForwardContact(contact);
        }
    }
}
