using ProtoBuf;
namespace puertstest.World.Data
{
    [ProtoContract]
    public class RelayFrameData
    {
        [field: ProtoMember(1)]
        public int FrameId { get; set; }

        [field: ProtoMember(2)]
        public int PrevLoadedFrameId { get; set; }

        [field: ProtoMember(3)]
        public int Count { get; set; }

        [field: ProtoMember(4)]
        public byte[] FramesData { get; set; }
    }
}
