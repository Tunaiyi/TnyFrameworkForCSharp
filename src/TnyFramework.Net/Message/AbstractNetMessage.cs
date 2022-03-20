using System;
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
            this.Body = body;
        }


        public long ToMessage => head.ToMessage;

        public MessageType Type => head.Type;

        public MessageMode Mode => head.Mode;

        public long Id => head.Id;

        public int ProtocolId => head.ProtocolId;

        public int Line => head.Line;

        public int Code => head.Code;

        public long Time => head.Time;


        public bool IsOwn(IProtocol protocol)
        {
            return head.IsOwn(protocol);
        }


        public void AllotMessageId(long id)
        {
            head.AllotMessageId(id);
        }


        public IMessageHead Head => head;

        public IAttributes Attribute => attributes.Attributes;

        public int GetCode() => Code;

        public bool ExistBody => Body != null;

        public object Body { get; }


        public T BodyAs<T>()
        {
            var type = typeof(T);
            var body = Body;
            if (body == null)
                return default;
            if (type.IsInstanceOfType(Body))
            {
                return (T)body;
            }
            throw new InvalidCastException($"{body.GetType()} can not convert to {type}");
        }
    }
}
