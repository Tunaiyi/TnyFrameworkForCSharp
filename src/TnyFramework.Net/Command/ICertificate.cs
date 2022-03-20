using TnyFramework.Net.Rpc;
namespace TnyFramework.Net.Command
{
    public interface ICertificate
    {
        long Id { get; }

        object GetUserId();

        string UserType { get; }

        long AuthenticateAt { get; }

        CertificateStatus Status { get; }

        bool IsAuthenticated();

    }

    public interface ICertificate<out TUserId> : ICertificate
    {
        TUserId UserId { get; }
    }

    public static class CertificateExtensions
    {
        public static bool IsNewerThan<TUserId>(this ICertificate one, ICertificate other)
        {
            if (one.IsAuthenticated() && !other.IsAuthenticated())
            {
                return true;
            }
            if (!one.IsAuthenticated() && other.IsAuthenticated())
            {
                return true;
            }
            var oneAt = one.AuthenticateAt;
            var otherAt = one.AuthenticateAt;
            if (oneAt > 0 && otherAt > 0)
            {
                return oneAt > otherAt;
            }
            return true;
        }


        public static bool IsOlderThan(this ICertificate one, ICertificate other)
        {
            if (one.IsAuthenticated() && !other.IsAuthenticated())
            {
                return true;
            }
            if (!one.IsAuthenticated() && other.IsAuthenticated())
            {
                return true;
            }
            var oneAt = one.AuthenticateAt;
            var otherAt = one.AuthenticateAt;
            if (oneAt > 0 && otherAt > 0)
            {
                return oneAt < otherAt;
            }
            return true;
        }


        public static bool IsSameUser(this ICertificate one, ICertificate other)
        {
            if (ReferenceEquals(one, other))
            {
                return true;
            }
            return Equals(one.GetUserId(), other.GetUserId()) && Equals(one.UserType, other.UserType);
        }


        public static bool IsSameCertificate(this ICertificate one, ICertificate other)
        {

            if (ReferenceEquals(one, other))
            {
                return true;
            }
            return one.Id == other.Id && Equals(one.GetUserId(), other.GetUserId()) && Equals(one.UserType, other.UserType);
        }
    }
}
