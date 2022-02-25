namespace TnyFramework.Net.Endpoint
{
    public interface ISessionKeeper : IEndpointKeeper
    {
    }

    public interface ISessionKeeper<TUserId> : ISessionKeeper, IEndpointKeeper<TUserId, ISession<TUserId>>
    {
    }
}
