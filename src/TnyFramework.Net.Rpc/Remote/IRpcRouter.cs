namespace TnyFramework.Net.Rpc.Remote
{

    public interface IRpcRouter
    {
        IRpcRemoteAccess Route(RpcRemoteServiceSet servicer, RpcRemoteMethod method, object routeValue, RpcRemoteInvokeParams parameters);
    }

}
