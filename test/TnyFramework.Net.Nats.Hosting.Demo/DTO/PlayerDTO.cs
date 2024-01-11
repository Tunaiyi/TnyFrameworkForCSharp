using ProtoBuf;
using TnyFramework.Codec.Attributes;
using TnyFramework.Codec.ProtobufNet.Attributes;
using TnyFramework.Codec.ProtobufNet.TypeProtobuf;

namespace TnyFramework.Net.Nats.Hosting.Demo.DTO;

[Codable(ProtobufMimeType.PROTOBUF_TYPE)]
[TypeProtobuf(1000_03_00)]
[ProtoContract]
public class PlayerDTO
{
    [ProtoMember(1)]
    public long id;

    [ProtoMember(2)]
    public string name = "";

    [ProtoMember(3)]
    public int age;
}