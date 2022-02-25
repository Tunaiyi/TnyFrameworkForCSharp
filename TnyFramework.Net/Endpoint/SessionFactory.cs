using TnyFramework.Net.Command;
namespace TnyFramework.Net.Endpoint
{
    public class SessionFactory : ISessionFactory
    {
        public INetSession Create<TUserId>(ISessionSetting setting, IEndpointContext context, ICertificateFactory<TUserId> certificateFactory)
        {
            return new Session<TUserId>(certificateFactory.Anonymous(), context);
        }
    }
}
