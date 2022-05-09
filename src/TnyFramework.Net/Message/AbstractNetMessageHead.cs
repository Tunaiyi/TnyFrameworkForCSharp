using System.Collections.Generic;

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

        public abstract T GetHeader<T>(string key) where T : MessageHeader<T>;

        public abstract MessageHeader GetHeader(string key);

        public abstract IList<T> GetHeaders<T>() where T : MessageHeader<T>;

        public abstract T GetHeader<T>(MessageHeaderKey<T> key) where T : MessageHeader<T>;

        public abstract bool IsHasHeaders { get; }

        public abstract IList<MessageHeader> GetAllHeaders();

        public abstract IDictionary<string, MessageHeader> GetAllHeadersMap();

        public abstract bool ExistHeader(string key);

        public abstract bool ExistHeader<T>(string key) where T : MessageHeader<T>;

        public abstract bool ExistHeader(MessageHeaderKey key);

        public abstract void AllotMessageId(long id);
    }

}
