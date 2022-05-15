using TnyFramework.Net.Base;

namespace TnyFramework.Net.Command
{

    public class CertificateFactory<TUserId> : ICertificateFactory<TUserId>
    {
        private readonly TUserId anonymousUserId;

        public CertificateFactory()
        {
            anonymousUserId = default;
        }

        public CertificateFactory(TUserId anonymousUserId)
        {
            this.anonymousUserId = anonymousUserId;
        }

        public ICertificate<TUserId> Anonymous()
        {
            return Certificate.CreateUnauthenticated(anonymousUserId);
        }

        public ICertificate<TUserId> Authenticate(long id, TUserId userId, long messagerId, IMessagerType messagerType, long authenticateAt)
        {
            return Certificate.CreateAuthenticated(id, userId, messagerId, messagerType, authenticateAt);
        }

        public ICertificate<TUserId> RenewAuthenticate(long id, TUserId userId, long messagerId, IMessagerType messagerType, long authenticateAt)
        {
            return Certificate.CreateAuthenticated(id, userId, messagerId, messagerType, authenticateAt, true);
        }

        ICertificate ICertificateFactory.Anonymous()
        {
            return Anonymous();
        }

        public ICertificate GeneralAuthenticate(long id, object userId, long messagerId, IMessagerType messagerType, long authenticateAt)
        {
            return Authenticate(id, (TUserId) userId, messagerId, messagerType, authenticateAt);
        }

        public ICertificate RenewAuthenticate(long id, object userId, long messagerId, IMessagerType messagerType, long authenticateAt)
        {
            return RenewAuthenticate(id, (TUserId) userId, messagerId, messagerType, authenticateAt);
        }
    }

}
