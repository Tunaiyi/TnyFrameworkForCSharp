using System;
using TnyFramework.Net.Command;
using TnyFramework.Net.Exceptions;
using TnyFramework.Net.Message;
using TnyFramework.Net.Transport;

namespace TnyFramework.Net.Rpc.Auth
{

    public class RpcTokenValidator : AuthenticateValidator<RpcAccessIdentify>
    {
        private readonly IIdGenerator idCreator;

        private readonly IRpcAuthService rpcAuthService;

        public RpcTokenValidator(IIdGenerator idCreator, IRpcAuthService rpcAuthService)
        {
            this.idCreator = idCreator;
            this.rpcAuthService = rpcAuthService;
        }

        public override ICertificate<RpcAccessIdentify> Validate(ITunnel<RpcAccessIdentify> tunnel, IMessage message,
            ICertificateFactory<RpcAccessIdentify> factory)
        {
            var token = message.BodyAs<string>();
            try
            {
                var result = rpcAuthService.VerifyToken(token);
                if (result.IsSuccess())
                {
                    var rpcToken = result.Value;
                    return factory.Authenticate(idCreator.Generate(), RpcAccessIdentify.Parse(rpcToken.Id), rpcToken.Id,
                        rpcToken.ServiceType, DateTimeOffset.Now.ToUnixTimeMilliseconds());
                }
                var resultCode = result.Code;
                throw new ValidationException(resultCode, $"Rpc登录认证失败. {{resultCode}} : {result.Message}");
            } catch (Exception e)
            {
                throw new ValidationException(e, "Rpc登录认证失败");
            }
        }
    }

}
