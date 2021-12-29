using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using TnyFramework.Common.Result;
using TnyFramework.Net.DotNetty.Message;

namespace TnyFramework.Net.DotNetty.Transport
{
    internal class DefaultMessageContext : RequestContext
    {
        private object body;

        public Task WrittenTask { get; set; }


        public DefaultMessageContext(MessageMode mode, IProtocol protocol, IResultCode resultCode,
            long toMessage = MessageConstants.EMPTY_MESSAGE_ID)
        {
            ResultCode = resultCode;
            ToMessage = toMessage;
            Mode = mode;
            ProtocolId = protocol.ProtocolId;
            Line = protocol.Line;
        }


        public override IResultCode ResultCode { get; }

        public override object Body => body;

        public override int ProtocolId { get; }

        public override int Line { get; }

        public override long ToMessage { get; }
        public override MessageType Type => Mode.GetMessageType();
        public override MessageMode Mode { get; }

        public override bool ExistBody => body != null;


        public override T BodyAs<T>()
        {
            if (body == null)
                return default;
            return (T)body;
        }


        public override MessageContext WithBody(object messageBody)
        {
            if (body == null)
            {
                body = messageBody;
            }
            return this;
        }


        public override void Cancel(bool mayInterruptIfRunning)
        {
            throw new System.NotImplementedException();
        }


        public override void Cancel(System.Exception cause)
        {
            throw new System.NotImplementedException();
        }


        public override Task<IMessage> Respond()
        {
            throw new System.NotImplementedException();
        }


        public override bool IsRespondAwaitable()
        {
            throw new System.NotImplementedException();
        }


        public override Task Written()
        {
            throw new System.NotImplementedException();
        }


        public override bool IsWriteAwaitable()
        {
            throw new System.NotImplementedException();
        }


        public override RequestContext WillRespondAwaiter(long timeout)
        {
            throw new System.NotImplementedException();
        }
    }
}
