namespace TnyFramework.Net.Endpoint
{

    public interface INetSession : INetEndpoint, ISession
    {
    }

    public interface INetSession<out TUserId> : INetEndpoint<TUserId>, INetSession, ISession<TUserId>
    {
    }

}
