using System;
using TnyFramework.Net.Base;
using TnyFramework.Net.Command;
using TnyFramework.Net.Demo.DTO;
using TnyFramework.Net.Dispatcher;
using TnyFramework.Net.Endpoint;
using TnyFramework.Net.Rpc.Attributes;
using TnyFramework.Net.Transport;
namespace TnyFramework.Net.Demo.Controller
{
    [RpcController]
    [AuthenticationRequired(MessagerType.DEFAULT_USER_TYPE)]
    public class LoginController : IController
    {
        [RpcRequest(CtrlerIds.LOGIN_4_LOGIN)]
        [AuthenticationRequired(typeof(DemoAuthenticateValidator), MessagerType.DEFAULT_USER_TYPE)]
        public LoginDTO Login(ITunnel<long> tunnel, IEndpoint<long> endpoint, [MsgParam] long sessionId, [MsgParam] long userId)
        {
            var certificate = endpoint.Certificate;
            //        EXECUTOR_SERVICE.scheduleAtFixedRate(() -> endpoint.send(MessageContexts
            //                .push(ProtocolAide.protocol(CtrlerIDs.LOGIN$PING), "ping tunnel id " + tunnel.getId())), 0, 3, TimeUnit.SECONDS);
            return new LoginDTO(certificate.Id, userId, $"{userId} - {sessionId} 登录成功 at {DateTimeOffset.Now.ToUnixTimeMilliseconds()}");
        }
    }
}
