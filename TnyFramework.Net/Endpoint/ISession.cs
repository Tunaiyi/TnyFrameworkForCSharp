namespace TnyFramework.Net.Endpoint
{
    public interface ISession : IEndpoint
    {
    }

    public interface ISession<out TUserId> : ISession, IEndpoint<TUserId>
    {
    }
}
