// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using TnyFramework.Net.Command.Dispatcher;
using TnyFramework.Net.Common;
using TnyFramework.Net.Exceptions;
using TnyFramework.Net.Session;

namespace TnyFramework.Net.Command.Auth;

public class ContactAuthenticateService : IContactAuthenticator
{
    private readonly ISessionKeeperManager sessionKeeperManager;

    public ContactAuthenticateService(ISessionKeeperManager sessionKeeperManager)
    {
        this.sessionKeeperManager = sessionKeeperManager;
    }

    public void Authenticate(MessageDispatcherContext dispatcherContext, IRpcMessageEnterContext context, Type type)
    {
        var tunnel = context.NetTunnel;
        var message = context.NetMessage;
        if (tunnel.IsAuthenticated())
            return;
        var validator = GetValidator(dispatcherContext, type);
        if (validator == null)
        {
            throw new AuthFailedException(NetResultCode.SERVER_ERROR, null, $"{nameof(type)} is null");
        }
        var certificate = validator.Validate(tunnel, message);
        // 是否需要做登录校验,判断是否已经登录
        if (!certificate.IsAuthenticated())
            return;
        var sessionKeeper = sessionKeeperManager
            .LoadKeeper(certificate.ContactType, tunnel.AccessMode);
        sessionKeeper?.Online(certificate, tunnel);
    }

    private IAuthenticationValidator? GetValidator(MessageDispatcherContext dispatcherContext, Type validatorClass)
    {
        return dispatcherContext.Validator(validatorClass);
    }
}
