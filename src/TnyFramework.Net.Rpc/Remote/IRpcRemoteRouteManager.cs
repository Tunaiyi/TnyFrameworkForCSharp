using System;

namespace TnyFramework.Net.Rpc.Remote
{

    public interface IRpcRemoteRouteManager
    {
        IRpcRemoteRouter GetRouter<T>() where T : IRpcRemoteRouter;

        IRpcRemoteRouter GetRouter(Type type);
    }

}
