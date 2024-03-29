// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using TnyFramework.Net.Application;
using TnyFramework.Net.Command.Auth;
using TnyFramework.Net.Exceptions;
using TnyFramework.Net.Message;
using TnyFramework.Net.Transport;

namespace TnyFramework.Net.DotNetty.Demo.Controller;

public class DemoAuthenticationValidator : AuthenticationValidator
{
    public override ICertificate Validate(ITunnel tunnel, IMessage message)
    {
            var value = message.Body;
            if (!(value is MessageParamList paramList))
                throw new AuthFailedException("登录失败");
            var id = (long) paramList[0]!;
            var userId = (long) paramList[1]!;
            return Certificates.CreateAuthenticated(id, userId, userId, NetContactType.DEFAULT_USER, userId);
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