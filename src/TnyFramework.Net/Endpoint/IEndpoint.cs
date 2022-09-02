using TnyFramework.Coroutines.Async;
using TnyFramework.Net.Transport;

namespace TnyFramework.Net.Endpoint
{

    public interface IEndpoint : ICommunicator, IConnection, ISender, IAsyncExecutor
    {
        long Id { get; }

        MessageHandleFilter SendFilter { get; set; }

        MessageHandleFilter ReceiveFilter { get; set; }

        void Heartbeat();

        EndpointStatus Status { get; }

        long OfflineTime { get; }

        void Offline();

        bool IsOnline();

        bool IsOffline();
    }

    public interface IEndpoint<out TUserId> : IEndpoint, ICommunicator<TUserId>
    {
    }

}
