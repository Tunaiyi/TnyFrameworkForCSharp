namespace TnyFramework.Net.Rpc
{

    public interface IRpcRemoteServiceManager
    {
        RpcRemoteServiceSet LoadOrCreate(IRpcServiceType serviceType);

        RpcRemoteServiceSet Find(IRpcServiceType serviceType);
    }

}
