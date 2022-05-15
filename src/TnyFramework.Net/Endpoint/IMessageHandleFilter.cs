using TnyFramework.Net.Message;

namespace TnyFramework.Net.Endpoint
{

    public delegate MessageHandleStrategy MessageHandleFilter(IEndpoint endpoint, IMessageContent message);

    public class MessageHandleFilters
    {
        public static readonly MessageHandleFilter ALL_IGNORE_FILTER = (e, m) => MessageHandleStrategy.Ignore;

        public static readonly MessageHandleFilter ALL_HANDLE_FILTER = (e, m) => MessageHandleStrategy.Handle;

        public static readonly MessageHandleFilter ALL_THROW_FILTER = (e, m) => MessageHandleStrategy.Throw;
    }

    public interface IMessageHandleFilter
    {
        MessageHandleStrategy Filter(IEndpoint endpoint, IMessageContent message);
    }

}
