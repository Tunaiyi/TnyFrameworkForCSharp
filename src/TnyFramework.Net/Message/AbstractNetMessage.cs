using System;
using System.Collections.Generic;
using TnyFramework.Common.Attribute;

namespace TnyFramework.Net.Message
{

    public abstract class AbstractNetMessage : INetMessage
    {
        private readonly INetMessageHead head;

        private readonly AttributesContext attributes = new AttributesContext();

        protected AbstractNetMessage(INetMessageHead head)
        {
            this.head = head;
        }

        protected AbstractNetMessage(INetMessageHead head, object body)
        {
            this.head = head;
            Body = body;
        }

        public long Id => head.Id;

        public int ProtocolId => head.ProtocolId;

        public int Line => head.Line;

        public int Code => head.Code;

        public long Time => head.Time;

        public long ToMessage => head.ToMessage;

        public MessageType Type => head.Type;

        public MessageMode Mode => head.Mode;

        public IMessageHead Head => head;

        public object Body { get; }

        public IDictionary<string, MessageHeader> Headers => Head.GetAllHeadersMap();

        public IAttributes Attribute => attributes.Attributes;

        public bool ExistBody => Body != null;

        public T GetHeader<T>(string key) where T : MessageHeader<T> => head.GetHeader<T>(key);

        public MessageHeader GetHeader(string key, Type type) => head.GetHeader(key, type);

        public MessageHeader GetHeader(string key) => head.GetHeader(key);

        public IList<T> GetHeaders<T>() where T : MessageHeader<T> => head.GetHeaders<T>();

        public IList<MessageHeader> GetHeaders(Type type) => head.GetHeaders(type);

        public T GetHeader<T>(MessageHeaderKey<T> key) where T : MessageHeader<T> => head.GetHeader(key);

        public bool IsHasHeaders => head.IsHasHeaders;

        public IList<MessageHeader> GetAllHeaders() => head.GetAllHeaders();

        public IDictionary<string, MessageHeader> GetAllHeadersMap() => head.GetAllHeadersMap();

        public bool ExistHeader(string key) => head.ExistHeader(key);

        public bool ExistHeader<T>(string key) where T : MessageHeader<T> => head.ExistHeader<T>(key);

        public bool ExistHeader(MessageHeaderKey key) => head.ExistHeader(key);

        public bool IsOwn(IProtocol protocol) => head.IsOwn(protocol);

        public void AllotMessageId(long id) => head.AllotMessageId(id);

        public int GetCode() => Code;

        public T BodyAs<T>()
        {
            var type = typeof(T);
            var body = Body;
            if (body == null)
                return default;
            if (type.IsInstanceOfType(Body))
            {
                return (T) body;
            }
            throw new InvalidCastException($"{body.GetType()} can not convert to {type}");
        }
    }

}
