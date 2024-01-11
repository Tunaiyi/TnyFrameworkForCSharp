using System;
using TnyFramework.Net.Application;
using TnyFramework.Net.Attributes;
using TnyFramework.Net.Command.Dispatcher;
using TnyFramework.Net.Nats.Hosting.Demo.DTO;
using TnyFramework.Net.Session;
using TnyFramework.Net.Transport;

namespace TnyFramework.Net.Nats.Hosting.Demo.Controller;

[RpcController]
[AuthenticationRequired(ContactType.DEFAULT_USER_TYPE)]
public class LoginController : IController
{
    [RpcRequest(CtrlerIds.LOGIN_4_LOGIN)]
    [AuthenticationRequired(typeof(DemoAuthenticationValidator), ContactType.DEFAULT_USER_TYPE)]
    public LoginDTO Login(ITunnel tunnel, ISession endpoint, long sessionId, long userId)
    {
        var certificate = endpoint.Certificate;
        //        EXECUTOR_SERVICE.scheduleAtFixedRate(() -> endpoint.send(MessageContexts
        //                .push(ProtocolAide.protocol(CtrlerIDs.LOGIN$PING), "ping tunnel id " + tunnel.getId())), 0, 3, TimeUnit.SECONDS);
        return new LoginDTO(certificate.Id, userId,
            $"{userId} - {sessionId} 登录成功 at {DateTimeOffset.Now.ToUnixTimeMilliseconds()}");
    }
}