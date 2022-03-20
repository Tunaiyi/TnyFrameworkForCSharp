namespace TnyFramework.Net.Message
{
    public abstract class AbstractNetMessageHead : INetMessageHead
    {
        protected AbstractNetMessageHead(MessageMode mode)
        {
            Mode = mode;
        }
        
        public MessageType Type => Mode.GetMessageType();

        public MessageMode Mode { get; }

        public abstract long ToMessage { get; }

        public abstract int ProtocolId { get; }

        public abstract int Line { get; }

        public abstract bool IsOwn(IProtocol protocol);

        public abstract long Id { get; }

        public abstract int Code { get; }

        public abstract long Time { get; }

        public abstract void AllotMessageId(long id);
    }
}
