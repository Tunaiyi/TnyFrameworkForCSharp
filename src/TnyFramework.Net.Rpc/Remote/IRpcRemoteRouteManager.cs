using System;

namespace TnyFramework.Net.Rpc.Remote
{

    public interface IRpcRemoteRouteManager
    {
        IRpcRouter GetRouter<T>() where T : IRpcRouter;

        IRpcRouter GetRouter(Type type);
    }

}
