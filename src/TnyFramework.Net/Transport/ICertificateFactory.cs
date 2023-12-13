// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using TnyFramework.Net.Base;

namespace TnyFramework.Net.Transport
{

    public interface ICertificateFactory
    {
        ICertificate Anonymous();

        ICertificate GeneralAuthenticate(long id, object userId, long contactId, IContactType contactType, long authenticateAt);

        ICertificate RenewAuthenticate(long id, object userId, long contactId, IContactType contactType, long authenticateAt);
    }

    public interface ICertificateFactory<TUserId> : ICertificateFactory
    {
        new ICertificate<TUserId> Anonymous();

        ICertificate<TUserId> Authenticate(long id, TUserId userId, long contactId, IContactType contactType, long authenticateAt);

        ICertificate<TUserId> RenewAuthenticate(long id, TUserId userId, long contactId, IContactType contactType, long authenticateAt);
    }

}
