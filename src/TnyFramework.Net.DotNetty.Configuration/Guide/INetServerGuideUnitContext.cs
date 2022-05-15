using TnyFramework.Net.Base;

namespace TnyFramework.Net.DotNetty.Configuration.Guide
{

    public interface INetServerGuideUnitContext : INetGuideUnitContext
    {
        IServerSetting LoadServerSetting();
    }

    public interface INetServerGuideUnitContext<TUserId> : INetGuideUnitContext<TUserId>, INetServerGuideUnitContext
    {
    }

}
