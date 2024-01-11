// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using TnyFramework.Net.Application;
using TnyFramework.Net.Attributes;
using TnyFramework.Net.Command.Dispatcher;
using TnyFramework.Net.DotNetty.Demo.DTO;
using TnyFramework.Net.Session;
using TnyFramework.Net.Transport;

namespace TnyFramework.Net.DotNetty.Demo.Controller;

[RpcController]
[AuthenticationRequired(ContactType.DEFAULT_USER_TYPE)]
public class LoginController : IController
{
    [RpcRequest(CtrlerIds.LOGIN_4_LOGIN)]
    [AuthenticationRequired(typeof(DemoAuthenticationValidator), ContactType.DEFAULT_USER_TYPE)]
    public LoginDTO Login(ITunnel tunnel, ISession session, long sessionId, long userId)
    {
            var certificate = session.Certificate;
            //        EXECUTOR_SERVICE.scheduleAtFixedRate(() -> session.send(MessageContexts
            //                .push(ProtocolAide.protocol(CtrlerIDs.LOGIN$PING), "ping tunnel id " + tunnel.getId())), 0, 3, TimeUnit.SECONDS);
            return new LoginDTO(certificate.Id, userId, $"{userId} - {sessionId} 登录成功 at {DateTimeOffset.Now.ToUnixTimeMilliseconds()}");
        }
}