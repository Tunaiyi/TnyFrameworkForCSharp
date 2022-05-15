using Castle.Core.Internal;

namespace TnyFramework.Net.Rpc.Remote
{

    public class FirstRpcRemoteRouter : IRpcRemoteRouter
    {
        public IRpcRemoteAccessPoint Route(RpcRemoteServiceSet servicer, RpcRemoteMethod method, object routeValue, object[] parameters)
        {
            var nodes = servicer.OrderRemoteNodes;
            if (nodes.IsNullOrEmpty())
            {
                return null;
            }
            var node = nodes[0];
            var accessPoints = node.OrderAccessPoints;
            return accessPoints.IsNullOrEmpty() ? null : accessPoints[0];
        }
    }

}
