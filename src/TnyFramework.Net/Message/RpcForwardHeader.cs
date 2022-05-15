using System.Collections.Generic;
using ProtoBuf;
using TnyFramework.Codec;
using TnyFramework.Codec.Attributes;
using TnyFramework.Codec.ProtobufNet.Attributes;
using TnyFramework.Net.Rpc;

namespace TnyFramework.Net.Message
{

    [Codable(MimeTypes.PROTOBUF_TYPE)]
    [TypeProtobuf(MessageHeaderConstants.RPC_FORWARD_HEADER_TYPE_PROTO)]
    [ProtoContract]
    public class RpcForwardHeader : MessageHeader<RpcForwardHeader>
    {
        [ProtoMember(2)]
        public ForwardRpcServicer From { get; set; }

        [ProtoMember(3)]
        public ForwardMessager Sender { get; set; }

        [ProtoMember(4)]
        public ForwardRpcServicer To { get; set; }

        [ProtoMember(5)]
        public ForwardMessager Receiver { get; set; }

        [ProtoMember(6, IsPacked = true)]
        public List<ForwardRpcServicer> Forwarders { get; set; } = new List<ForwardRpcServicer>();

        public override string Key => MessageHeaderConstants.RPC_FORWARD_HEADER_KEY;

        public RpcForwardHeader SetFrom(IRpcServicer fromService)
        {
            From = ToForwardRpcServicer(fromService);
            return this;
        }

        public RpcForwardHeader SetSender(IMessager sender)
        {
            Sender = ToForwardMessager(sender);
            return this;
        }

        public RpcForwardHeader SetTo(IRpcServicer toServicer)
        {
            To = ToForwardRpcServicer(toServicer);
            return this;
        }

        public RpcForwardHeader SetTo(IRpcServiceType serviceType)
        {
            To = ToForwardRpcServicer(new ForwardRpcServicer(serviceType));
            return this;
        }

        public RpcForwardHeader SetReceiver(IMessager receiver)
        {
            Receiver = ToForwardMessager(receiver);
            return this;
        }

        private static ForwardRpcServicer ToForwardRpcServicer(IRpcServicer rpcServicer)
        {
            switch (rpcServicer)
            {
                case null:
                    return null;
                case ForwardRpcServicer value:
                    return value;
                default:
                    return new ForwardRpcServicer(rpcServicer);
            }
        }

        private static ForwardMessager ToForwardMessager(IMessager messager)
        {
            switch (messager)
            {
                case null:
                    return null;
                case ForwardMessager value:
                    return value;
                default:
                    return new ForwardMessager(messager);
            }
        }
    }

}
