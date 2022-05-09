using TnyFramework.Net.Dispatcher;

namespace TnyFramework.Net.Rpc
{

    public interface IRpcContext
    {
        IRpcForwarder RpcForwarder { get; }
    }

}
