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
using TnyFramework.Net.Endpoint;
using TnyFramework.Net.Exceptions;

namespace TnyFramework.Net.Command.Auth
{

    public class MessagerAuthenticateService : IMessagerAuthenticator
    {
        private readonly IEndpointKeeperManager endpointKeeperManager;

        public MessagerAuthenticateService(IEndpointKeeperManager endpointKeeperManager)
        {
            this.endpointKeeperManager = endpointKeeperManager;
        }

        public void Authenticate(MessageDispatcherContext dispatcherContext, IRpcEnterContext context, Type type)
        {
            var tunnel = context.NetTunnel;
            var message = context.NetMessage;
            var networkContext = context.NetworkContext;
            if (tunnel.IsAuthenticated())
                return;
            var certificateFactory = networkContext.GetCertificateFactory();
            var validator = GetValidator(dispatcherContext, type);
            if (validator == null)
            {
                throw new AuthFailedException(NetResultCode.SERVER_ERROR, null, $"{nameof(type)} is null");
            }
            var certificate = validator.Validate(tunnel, message, certificateFactory);
            // 是否需要做登录校验,判断是否已经登录
            if (certificate == null || !certificate.IsAuthenticated())
                return;
            var endpointKeeper = endpointKeeperManager
                .LoadKeeper(certificate.MessagerType, tunnel.AccessMode);
            endpointKeeper?.Online(certificate, tunnel);
        }

        private IAuthenticationValidator? GetValidator(MessageDispatcherContext dispatcherContext, Type validatorClass)
        {
            return dispatcherContext.Validator(validatorClass);
        }
    }

}
