namespace TnyFramework.Net.DotNetty.Configuration.Guide
{

    public interface IServerUnitContext<TUserId>
    {
        INetUnitContext UnitContext { get; }

        INetServerGuideUnitContext<TUserId> ServerGuideUnitContext { get; }
    }

}
