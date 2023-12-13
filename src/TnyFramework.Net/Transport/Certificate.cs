// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using TnyFramework.Net.Base;

namespace TnyFramework.Net.Transport
{

    public class Certificate
    {
        private const long ANONYMITY_CONTACT_ID = -1L;

        public static Certificate<T> CreateAuthenticated<T>(long id, T userId, long contactId, IContactType contactType)
        {
            return CreateAuthenticated(id, userId, contactId, contactType, DateTimeOffset.Now.ToUnixTimeMilliseconds());
        }

        public static Certificate<T> CreateAuthenticated<T>(long id, T userId, long contactId, IContactType contactType,
            long authenticateAt, bool renew = false)
        {
            return new Certificate<T>(id, userId, contactId, contactType, renew ? CertificateStatus.Renew : CertificateStatus.Authenticated,
                authenticateAt);
        }

        public static Certificate<T> CreateUnauthenticated<T>(T? anonymityUserId = default)
        {
            return new Certificate<T>(-1, anonymityUserId!, ANONYMITY_CONTACT_ID, NetContactType.ANONYMITY, CertificateStatus.Unauthenticated, 0);
        }
    }

    public class Certificate<TUserId> : Certificate, ICertificate<TUserId>
    {
        private readonly CertificateStatus status;
        //
        // private final Instant createAt;

        // private Instant authenticateAt;

        public long Id { get; }

        public long ContactId { get; }

        public IContactType ContactType { get; }

        public string UserGroup => ContactType.Group;

        public long AuthenticateAt { get; }

        public CertificateStatus Status { get; }

        public TUserId UserId { get; }

        public Certificate(long id, TUserId userId, long contactId, IContactType contactType, CertificateStatus status, long authenticateAt)
        {
            this.status = status;
            Id = id;
            UserId = userId;
            ContactId = contactId;
            ContactType = contactType;
            AuthenticateAt = authenticateAt;
            Status = status;
            UserId = userId;
        }

        public bool IsAuthenticated() => status.IsAuthenticated();

        public object GetUserId() => UserId!;
    }

}
