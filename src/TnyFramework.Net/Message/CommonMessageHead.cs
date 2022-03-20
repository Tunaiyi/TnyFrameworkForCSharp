using System;
namespace TnyFramework.Net.Message
{
    public class CommonMessageHead : AbstractNetMessageHead
    {
        private long id;


        public CommonMessageHead(long id, MessageMode mode, int line, int protocolId, int code, long toMessage, long time) : base(mode)
        {
            this.id = id;
            ToMessage = toMessage;
            ProtocolId = protocolId;
            Line = line;
            Code = code;
            Time = time;
        }


        public CommonMessageHead(long id, IMessageContent subject) : base(subject.Mode)
        {
            this.id = id;
            ProtocolId = subject.ProtocolId;
            Code = subject.GetCode();
            ToMessage = subject.ToMessage;
            Time = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        }


        public override long ToMessage { get; }

        public override int ProtocolId { get; }

        public override int Line { get; }


        public override bool IsOwn(IProtocol protocol)
        {
            return ProtocolId == protocol.ProtocolId;
        }


        public override long Id => id;

        public override int Code { get; }

        public override long Time { get; }

        public override void AllotMessageId(long idValue) => id = idValue;


        private bool Equals(CommonMessageHead other)
        {
            return id == other.id && ToMessage == other.ToMessage && ProtocolId == other.ProtocolId && Line == other.Line && Code == other.Code &&
                   Time == other.Time;
        }


        public override string ToString()
        {
            return $"[{nameof(Id)}: {Id}, {nameof(Mode)}: {Mode}, {nameof(ProtocolId)}: {ProtocolId}]";
        }


        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((CommonMessageHead)obj);
        }


        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = id.GetHashCode();
                hashCode = (hashCode * 397) ^ ToMessage.GetHashCode();
                hashCode = (hashCode * 397) ^ ProtocolId;
                hashCode = (hashCode * 397) ^ Line;
                hashCode = (hashCode * 397) ^ Code;
                hashCode = (hashCode * 397) ^ Time.GetHashCode();
                return hashCode;
            }
        }
    }
}
