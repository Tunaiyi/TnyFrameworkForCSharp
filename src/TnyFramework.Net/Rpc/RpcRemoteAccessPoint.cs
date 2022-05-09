using TnyFramework.Net.Message;
using TnyFramework.Net.Transport;

namespace TnyFramework.Net.Rpc
{

    public interface IRpcRemoteAccessPoint : ISender
    {
        RpcAccessIdentify AccessId { get; }

        ForwardRpcServicer ForwardRpcServicer { get; }
    }

}
