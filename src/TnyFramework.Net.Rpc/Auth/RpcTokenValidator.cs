// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using TnyFramework.Net.Command.Auth;
using TnyFramework.Net.Exceptions;
using TnyFramework.Net.Message;
using TnyFramework.Net.Transport;

namespace TnyFramework.Net.Rpc.Auth
{

    public class RpcTokenValidator : AuthenticationValidator
    {
        private readonly IIdGenerator idCreator;

        private readonly IRpcAuthService rpcAuthService;

        public RpcTokenValidator(IIdGenerator idCreator, IRpcAuthService rpcAuthService)
        {
            this.idCreator = idCreator;
            this.rpcAuthService = rpcAuthService;
        }

        public override ICertificate Validate(ITunnel tunnel, IMessage message)
        {
            var token = message.BodyAs<string>();
            if (token == null)
            {
                throw new AuthFailedException("Token is null");
            }
            try
            {
                var result = rpcAuthService.VerifyToken(token);
                if (result.IsSuccess())
                {
                    var rpcToken = result.Value;
                    var rpcIdentify = RpcAccessIdentify.Parse(rpcToken.Id);
                    return Certificates.CreateAuthenticated(idCreator.Generate(), rpcIdentify.Id, rpcToken.Id, rpcToken.ServiceType, rpcIdentify);
                }
                var resultCode = result.Code;
                throw new AuthFailedException(resultCode, null, $"Rpc登录认证失败. {{resultCode}} : {result.Message}");
            } catch (Exception e)
            {
                throw new AuthFailedException(e, null, null, "Rpc登录认证失败");
            }
        }
    }

}
