// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using TnyFramework.Net.Application;

namespace TnyFramework.Net.Transport;

public static class Certificates
{
    public static ICertificate Anonymous { get; } = new Certificate(ICertificate.ANONYMITY_ID, ICertificate.ANONYMITY_IDENTIFY,
        ICertificate.ANONYMITY_CONTACT_ID, NetContactType.ANONYMITY, CertificateStatus.Unauthenticated, 0, null);

    public static ICertificate CreateAuthenticated(long id, long identify, long contactId, IContactType contactType, object? identifyToken = null)
    {
        return CreateAuthenticated(id, identify, contactId, contactType, CertificateStatus.Authenticated,
            DateTimeOffset.Now.ToUnixTimeMilliseconds(), identifyToken);
    }

    public static ICertificate RenewAuthenticated(long id, long identify, long contactId, IContactType contactType, long authenticateAt,
        object? identifyToken = null)
    {
        return CreateAuthenticated(id, identify, contactId, contactType, CertificateStatus.Renew, authenticateAt, identifyToken);
    }

    private static ICertificate CreateAuthenticated(long id, long identify, long contactId, IContactType contactType,
        CertificateStatus status, long authenticateAt, object? identifyToken = null)
    {
        return new Certificate(id, identify, contactId, contactType, status,
            authenticateAt, identifyToken);
    }

    public static ICertificate GetAnonymity() => Anonymous;

    private class Certificate : ICertificate
    {
        internal Certificate(long id, long identify, long contactId, IContactType contactType, CertificateStatus status,
            long authenticateAt, object? identifyToken)
        {
            Id = id;
            Identify = identify;
            Status = status;
            Identify = identify;
            IdentifyToken = identifyToken;
            ContactId = contactId;
            ContactType = contactType;
            AuthenticateAt = authenticateAt;
        }

        public long ContactId { get; }

        public IContactType ContactType { get; }

        public string ContactGroup => ContactType.Group;

        public long Id { get; }

        public long Identify { get; }

        public object? IdentifyToken { get; }

        public long AuthenticateAt { get; }

        public CertificateStatus Status { get; }

        public bool IsAuthenticated() => !Equals(ContactType, NetContactType.ANONYMITY);
    }
}
