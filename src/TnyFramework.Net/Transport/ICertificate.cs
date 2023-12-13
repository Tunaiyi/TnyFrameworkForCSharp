// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using TnyFramework.Net.Message;

namespace TnyFramework.Net.Transport
{

    public interface ICertificate : IContact
    {
        long Id { get; }

        object GetUserId();

        string UserGroup { get; }

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
        public static bool IsNewerThan(this ICertificate one, ICertificate other)
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
            return Equals(one.GetUserId(), other.GetUserId()) && Equals(one.UserGroup, other.UserGroup);
        }

        public static bool IsSameCertificate(this ICertificate one, ICertificate other)
        {

            if (ReferenceEquals(one, other))
            {
                return true;
            }
            return one.Id == other.Id && Equals(one.GetUserId(), other.GetUserId()) && Equals(one.UserGroup, other.UserGroup);
        }
    }

}
