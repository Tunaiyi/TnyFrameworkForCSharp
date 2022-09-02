using Castle.Core.Internal;

namespace TnyFramework.Net.Rpc.Remote
{

    public class FirstRpcRouter : IRpcRouter
    {
        public IRpcRemoteAccess Route(RpcRemoteServiceSet servicer, RpcRemoteMethod method, object routeValue, RpcRemoteInvokeParams parameters)
        {
            var nodes = servicer.OrderRemoteNodes;
            if (nodes.IsNullOrEmpty())
            {
                return null;
            }
            var node = nodes[0];
            var accessPoints = node.GetOrderRemoteAccesses();
            return accessPoints.IsNullOrEmpty() ? null : accessPoints[0];
        }
    }

}
