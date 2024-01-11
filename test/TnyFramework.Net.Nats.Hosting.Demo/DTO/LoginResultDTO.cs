using ProtoBuf;
using TnyFramework.Codec.Attributes;
using TnyFramework.Codec.ProtobufNet.Attributes;
using TnyFramework.Codec.ProtobufNet.TypeProtobuf;

namespace TnyFramework.Net.Nats.Hosting.Demo.DTO;

[Codable(ProtobufMimeType.PROTOBUF_TYPE)]
[TypeProtobuf(1000_00_00)]
[ProtoContract]
public class LoginResultDTO
{
    [ProtoMember(1)]
    public long userId;

    [ProtoMember(2)]
    public string message = "";
}