using System;
using TnyFramework.Net.Rpc;
namespace TnyFramework.Net.Command
{
    public class Certificate
    {
        public const string DEFAULT_USER_TYPE = "user";
        public const string ANONYMITY_USER_TYPE = "#anonymity";


        public static Certificate<T> CreateAuthenticated<T>(long id, T userId, string userType = DEFAULT_USER_TYPE)
        {
            return CreateAuthenticated(id, userId, userType, DateTimeOffset.Now.ToUnixTimeMilliseconds());
        }


        public static Certificate<T> CreateAuthenticated<T>(long id, T userId, long authenticateAt)
        {
            return CreateAuthenticated(id, userId, DEFAULT_USER_TYPE, authenticateAt);
        }


        public static Certificate<T> CreateAuthenticated<T>(long id, T userId, string userType, long authenticateAt, bool renew = false)
        {
            return new Certificate<T>(id, userType, userId, renew ? CertificateStatus.Renew : CertificateStatus.Authenticated, authenticateAt);
        }


        public static Certificate<T> CreateUnauthenticated<T>(T anonymityUserId = default)
        {
            return new Certificate<T>(-1, ANONYMITY_USER_TYPE, anonymityUserId, CertificateStatus.Unauthenticated, 0);
        }
    }

    public class Certificate<TUserId> : Certificate, ICertificate<TUserId>
    {
        private readonly CertificateStatus status;
        //
        // private final Instant createAt;

        // private Instant authenticateAt;

        public long Id { get; }

        public string UserType { get; }
        public long AuthenticateAt { get; }

        public CertificateStatus Status { get; }

        public TUserId UserId { get; }


        public Certificate(long id, string userType, TUserId userId, CertificateStatus status, long authenticateAt)
        {
            this.status = status;
            Id = id;
            UserType = userType;
            AuthenticateAt = authenticateAt;
            Status = status;
            UserId = userId;
        }


        public bool IsAuthenticated() => status.IsAuthenticated();

        public object GetUserId() => UserId;
    }
}
