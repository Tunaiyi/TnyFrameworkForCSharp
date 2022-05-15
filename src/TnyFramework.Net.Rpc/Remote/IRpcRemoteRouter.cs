namespace TnyFramework.Net.Rpc.Remote
{

    public interface IRpcRemoteRouter
    {
        IRpcRemoteAccessPoint Route(RpcRemoteServiceSet servicer, RpcRemoteMethod method, object routeValue, object[] parameters);
    }

    public interface IRpcRemoteRouter<T> : IRpcRemoteRouter
    {
        IRpcRemoteAccessPoint Route(RpcRemoteServiceSet servicer, RpcRemoteMethod method, T routeValue, object[] parameters);
    }

    public abstract class RpcRemoteRouter<T> : IRpcRemoteRouter<T>
    {
        public abstract IRpcRemoteAccessPoint Route(RpcRemoteServiceSet servicer, RpcRemoteMethod method, T routeValue, object[] parameters);

        public IRpcRemoteAccessPoint Route(RpcRemoteServiceSet servicer, RpcRemoteMethod method, object routeValue, object[] parameters)
        {
            if (routeValue is T value)
            {
                return Route(servicer, method, value, parameters);
            }
            return null;
        }
    }

}
