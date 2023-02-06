// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System.Collections.Generic;
using System.Collections.Immutable;
using TnyFramework.Net.Message;
using TnyFramework.Net.Transport;

namespace TnyFramework.Net.Command.Auth
{

    public interface IAuthenticationValidator
    {
        ICertificate Validate(ITunnel tunnel, IMessage message, ICertificateFactory certificateFactory);

        IList<int> AuthProtocolLimit { get; }
    }

    public interface IAuthenticationValidator<TUserId> : IAuthenticationValidator
    {
        ICertificate<TUserId> Validate(ITunnel<TUserId> tunnel, IMessage message, ICertificateFactory<TUserId> factory);
    }

    public abstract class AuthenticationValidator<TUserId> : IAuthenticationValidator<TUserId>
    {
        protected AuthenticationValidator()
        {
            AuthProtocolLimit = ImmutableList.Create<int>();
        }

        protected AuthenticationValidator(IList<int> authProtocolLimit)
        {
            AuthProtocolLimit = authProtocolLimit == null ? ImmutableList.Create<int>() : ImmutableList.CreateRange(authProtocolLimit);

        }

        public abstract ICertificate<TUserId> Validate(ITunnel<TUserId> tunnel, IMessage message, ICertificateFactory<TUserId> factory);

        public ICertificate Validate(ITunnel tunnel, IMessage message, ICertificateFactory certificateFactory)
        {
            return Validate((ITunnel<TUserId>) tunnel, message, (ICertificateFactory<TUserId>) certificateFactory);
        }

        public IList<int> AuthProtocolLimit { get; }
    }

}
