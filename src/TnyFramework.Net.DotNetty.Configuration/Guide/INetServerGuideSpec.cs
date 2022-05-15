using TnyFramework.Net.Base;
using TnyFramework.Net.DotNetty.Bootstrap;

namespace TnyFramework.Net.DotNetty.Configuration.Guide
{

    public interface INetServerGuideSpec<TUserId>
        : INetGuideSpec<INettyServerGuide, TUserId, INetServerGuideUnitContext<TUserId>, INetServerGuideSpec<TUserId>>
    {
        INetServerGuideSpec<TUserId> Server(IServerSetting setting);

        INetServerGuideSpec<TUserId> Server(int port);

        INetServerGuideSpec<TUserId> Server(string host, int port);

        INetServerGuideSpec<TUserId> Server(string host, int port, bool libuv);

        INetServerGuideSpec<TUserId> Server(string serveName, string host, int port);

        INetServerGuideSpec<TUserId> Server(string serveName, string host, int port, bool libuv);
    }

}
