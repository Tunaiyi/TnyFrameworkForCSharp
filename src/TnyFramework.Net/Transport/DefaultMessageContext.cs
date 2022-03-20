using System.Threading;
using System.Threading.Tasks;
using TnyFramework.Common.Result;
using TnyFramework.Net.Message;

namespace TnyFramework.Net.Transport
{
    public class DefaultMessageContext : RequestContext, IMessageWritableContext
    {
        public override IResultCode ResultCode { get; }

        public override int ProtocolId { get; }

        public override int Line { get; }

        public override long ToMessage { get; }

        public override MessageMode Mode { get; }

        public override MessageType Type => Mode.GetMessageType();

        public override object Body { get; protected set; }

        public TaskResponseSource ResponseSource { get; private set; }

        public Task WrittenTask { get; private set; }


        public DefaultMessageContext(MessageMode mode, IProtocol protocol, IResultCode resultCode,
            long toMessage = MessageConstants.EMPTY_MESSAGE_ID)
        {
            ResultCode = resultCode;
            ToMessage = toMessage;
            Mode = mode;
            ProtocolId = protocol.ProtocolId;
            Line = protocol.Line;
        }


        public override bool ExistBody => Body != null;


        public override T BodyAs<T>()
        {
            if (Body == null)
                return default;
            return (T)Body;
        }


        public override MessageContext WithBody(object messageBody)
        {
            if (Body == null)
            {
                Body = messageBody;
            }
            return this;
        }


        public override void Cancel(bool mayInterruptIfRunning)
        {
            ResponseSource?.TrySetCanceled();
            if (WrittenTask == null)
            {
                WrittenTask = Task.FromCanceled(CancellationToken.None);
            }
        }


        public override void Cancel(System.Exception cause)
        {
            ResponseSource?.SetException(cause);
            if (WrittenTask == null)
            {
                WrittenTask = Task.FromException(cause);
            }
        }


        public override Task<IMessage> Respond()
        {
            return ResponseSource?.Task;
        }


        public override bool IsRespondAwaitable()
        {
            return ResponseSource != null;
        }


        public override Task Written()
        {
            return WrittenTask ?? Task.CompletedTask;
        }


        public override bool IsWriteAwaitable()
        {
            return WrittenTask != null;
        }


        public override RequestContext WillRespondAwaiter(long timeout)
        {
            ResponseSource = new TaskResponseSource(timeout);
            return this;
        }


        public void SetWrittenTask(Task task)
        {
            if (WrittenTask == null)
            {
                WrittenTask = task;
            }
        }
    }
}
