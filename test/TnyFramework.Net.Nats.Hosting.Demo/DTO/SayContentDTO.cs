using ProtoBuf;
using TnyFramework.Codec.Attributes;
using TnyFramework.Codec.ProtobufNet.Attributes;
using TnyFramework.Codec.ProtobufNet.TypeProtobuf;

namespace TnyFramework.Net.Nats.Hosting.Demo.DTO;

[Codable(ProtobufMimeType.PROTOBUF_TYPE)]
[TypeProtobuf(1000_01_01)]
[ProtoContract]
public class SayContentDTO
{
    [ProtoMember(1)]
    public long userId;

    [ProtoMember(2)]
    public string message = "";

    public SayContentDTO()
    {
    }

    public SayContentDTO(long userId, string message)
    {
        this.userId = userId;
        this.message = message;
    }
}