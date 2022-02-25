using TnyFramework.Net.Rpc;
namespace TnyFramework.Net.Command
{
    public interface ICertificateFactory
    {
        ICertificate Anonymous();

        ICertificate GeneralAuthenticate(long id, object userId, string userType, long authenticateAt);

        ICertificate RenewAuthenticate(long id, object userId, string userType, long authenticateAt);
    }


    public interface ICertificateFactory<TUserId> : ICertificateFactory
    {
        new ICertificate<TUserId> Anonymous();

        ICertificate<TUserId> Authenticate(long id, TUserId userId, string userType, long authenticateAt);

        ICertificate<TUserId> RenewAuthenticate(long id, TUserId userId, string userType, long authenticateAt);
    }
}
