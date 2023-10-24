// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using TnyFramework.Net.Base;
using TnyFramework.Net.Rpc;

namespace TnyFramework.Net.Command
{

    public class Certificate
    {
        private const long ANONYMITY_MESSAGER_ID = -1L;

        public static Certificate<T> CreateAuthenticated<T>(long id, T userId, long messagerId, IMessagerType messagerType)
        {
            return CreateAuthenticated(id, userId, messagerId, messagerType, DateTimeOffset.Now.ToUnixTimeMilliseconds());
        }

        public static Certificate<T> CreateAuthenticated<T>(long id, T userId, long messagerId, IMessagerType messagerType,
            long authenticateAt, bool renew = false)
        {
            return new Certificate<T>(id, userId, messagerId, messagerType, renew ? CertificateStatus.Renew : CertificateStatus.Authenticated,
                authenticateAt);
        }

        public static Certificate<T> CreateUnauthenticated<T>(T? anonymityUserId = default)
        {
            return new Certificate<T>(-1, anonymityUserId!, ANONYMITY_MESSAGER_ID, NetMessagerType.ANONYMITY, CertificateStatus.Unauthenticated, 0);
        }
    }

    public class Certificate<TUserId> : Certificate, ICertificate<TUserId>
    {
        private readonly CertificateStatus status;
        //
        // private final Instant createAt;

        // private Instant authenticateAt;

        public long Id { get; }

        public long MessagerId { get; }

        public IMessagerType MessagerType { get; }

        public string UserGroup => MessagerType.Group;

        public long AuthenticateAt { get; }

        public CertificateStatus Status { get; }

        public TUserId UserId { get; }

        public Certificate(long id, TUserId userId, long messagerId, IMessagerType messagerType, CertificateStatus status, long authenticateAt)
        {
            this.status = status;
            Id = id;
            UserId = userId;
            MessagerId = messagerId;
            MessagerType = messagerType;
            AuthenticateAt = authenticateAt;
            Status = status;
            UserId = userId;
        }

        public bool IsAuthenticated() => status.IsAuthenticated();

        public object GetUserId() => UserId!;
    }

}
