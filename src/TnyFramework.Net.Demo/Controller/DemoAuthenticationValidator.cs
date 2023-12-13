// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using TnyFramework.Net.Base;
using TnyFramework.Net.Command;
using TnyFramework.Net.Command.Auth;
using TnyFramework.Net.Exceptions;
using TnyFramework.Net.Message;
using TnyFramework.Net.Rpc;
using TnyFramework.Net.Transport;

namespace TnyFramework.Net.Demo.Controller
{

    public class DemoAuthenticationValidator : AuthenticationValidator<long>
    {
        public override ICertificate<long> Validate(ITunnel<long> tunnel, IMessage message, ICertificateFactory<long> factory)
        {
            var value = message.Body;
            if (!(value is MessageParamList paramList))
                throw new AuthFailedException("登录失败");
            var id = (long) paramList[0]!;
            var userId = (long) paramList[1]!;
            return factory.Authenticate(id, userId, userId, NetContactType.DEFAULT_USER, DateTimeOffset.Now.ToUnixTimeMilliseconds());
            // if (value instanceof LoginDTO) {
            //     LoginDTO dto = as(value);
            //     return factory.certificate(dto.getCertId(), dto.getUserId(), Certificates.DEFAULT_USER_TYPE, Instant.now());
            // }
            // if (value instanceof LoginResultDTO) {
            //     LoginResultDTO dto = as(value);
            //     return factory.certificate(System.currentTimeMillis(), dto.getUserId(), Certificates.DEFAULT_USER_TYPE, Instant.now());
            // }
            // System.out.println(value);
        }
    }

}
