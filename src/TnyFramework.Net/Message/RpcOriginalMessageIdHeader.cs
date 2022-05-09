using ProtoBuf;
using TnyFramework.Codec;
using TnyFramework.Codec.Attributes;
using TnyFramework.Codec.ProtobufNet.Attributes;

namespace TnyFramework.Net.Message
{

    [Codable(MimeTypes.PROTOBUF_TYPE)]
    [TypeProtobuf(MessageHeaderConstants.RPC_ORIGINAL_MESSAGE_ID_TYPE_PROTO)]
    [ProtoContract]
    public class RpcOriginalMessageIdHeader : MessageHeader<RpcOriginalMessageIdHeader>
    {
        [ProtoMember(1)]
        public long MessageId { get; set; }

        public override string Key => MessageHeaderConstants.RPC_ORIGINAL_MESSAGE_ID_KEY;
    }

}
