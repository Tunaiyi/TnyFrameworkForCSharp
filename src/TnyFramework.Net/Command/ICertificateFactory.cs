using TnyFramework.Net.Base;

namespace TnyFramework.Net.Command
{

    public interface ICertificateFactory
    {
        ICertificate Anonymous();

        ICertificate GeneralAuthenticate(long id, object userId, long messagerId, IMessagerType messagerType, long authenticateAt);

        ICertificate RenewAuthenticate(long id, object userId, long messagerId, IMessagerType messagerType, long authenticateAt);
    }

    public interface ICertificateFactory<TUserId> : ICertificateFactory
    {
        new ICertificate<TUserId> Anonymous();

        ICertificate<TUserId> Authenticate(long id, TUserId userId, long messagerId, IMessagerType messagerType, long authenticateAt);

        ICertificate<TUserId> RenewAuthenticate(long id, TUserId userId, long messagerId, IMessagerType messagerType, long authenticateAt);
    }

}
