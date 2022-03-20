using System;
using TnyFramework.Net.Command;
using TnyFramework.Net.Exceptions;
using TnyFramework.Net.Message;
using TnyFramework.Net.Transport;
namespace TnyFramework.Net.Rpc.Auth
{
    public class RpcTokenValidator : AuthenticateValidator<RpcLinkerId>
    {
        private readonly IIdGenerator idCreator;

        private readonly IRpcAuthService rpcAuthService;


        public RpcTokenValidator(IIdGenerator idCreator, IRpcAuthService rpcAuthService)
        {
            this.idCreator = idCreator;
            this.rpcAuthService = rpcAuthService;
        }


        public override ICertificate<RpcLinkerId> Validate(ITunnel<RpcLinkerId> tunnel, IMessage message, ICertificateFactory<RpcLinkerId> factory)
        {
            var token = message.BodyAs<string>();
            try
            {
                var result = rpcAuthService.VerifyToken(token);
                if (result.IsSuccess())
                {
                    var rpcToken = result.Value;
                    return factory.Authenticate(idCreator.Generate(), new RpcLinkerId(rpcToken.Service, rpcToken.ServerId, rpcToken.Id),
                        rpcToken.Service, DateTimeOffset.Now.ToUnixTimeMilliseconds());
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
