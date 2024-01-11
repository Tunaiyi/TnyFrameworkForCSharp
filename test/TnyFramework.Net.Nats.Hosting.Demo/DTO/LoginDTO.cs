using ProtoBuf;
using TnyFramework.Codec.Attributes;
using TnyFramework.Codec.ProtobufNet.Attributes;
using TnyFramework.Codec.ProtobufNet.TypeProtobuf;

namespace TnyFramework.Net.Nats.Hosting.Demo.DTO;

[Codable(ProtobufMimeType.PROTOBUF_TYPE)]
[TypeProtobuf(1000_01_00)]
[ProtoContract]
public class LoginDTO
{
    [ProtoMember(1)]
    public long userId;

    [ProtoMember(2)]
    public string message = "";

    [ProtoMember(3)]
    public long certId;

    public LoginDTO()
    {
    }

    public LoginDTO(long certId, long userId, string message)
    {
        this.userId = userId;
        this.message = message;
        this.certId = certId;
    }
}