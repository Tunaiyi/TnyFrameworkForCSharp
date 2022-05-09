using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using TnyFramework.Common.Result;
using TnyFramework.Net.Command;
using TnyFramework.Net.Exceptions;
using TnyFramework.Net.Message;
using TnyFramework.Net.Transport;

namespace TnyFramework.Net.Rpc.Auth
{

    public class RpcPasswordValidator : IAuthenticateValidator<RpcAccessIdentify>
    {
        private readonly IRpcAuthService rpcAuthService;

        private readonly IIdGenerator idGenerator;

        public IList<int> AuthProtocolLimit { get; } = ImmutableList.Create<int>();


        public RpcPasswordValidator(IIdGenerator idGenerator, IRpcAuthService rpcAuthService)
        {
            this.idGenerator = idGenerator;
            this.rpcAuthService = rpcAuthService;
        }


        public ICertificate Validate(ITunnel tunnel, IMessage message, ICertificateFactory certificateFactory)
        {
            if (tunnel is ITunnel<RpcAccessIdentify> rpcTunnel && certificateFactory is ICertificateFactory<RpcAccessIdentify> rpcFactory)
                return Validate(rpcTunnel, message, rpcFactory);
            throw new ValidationException("服务器错误");
        }


        public ICertificate<RpcAccessIdentify> Validate(ITunnel<RpcAccessIdentify> tunnel, IMessage message,
            ICertificateFactory<RpcAccessIdentify> factory)
        {
            var paramList = message.BodyAs<MessageParamList>();
            if (paramList == null)
            {
                throw new ValidationException("Rpc登录参数错误");
            }
            var id = RpcAuthMessageContexts.GetIdParam(paramList);
            var password = RpcAuthMessageContexts.GetPasswordParam(paramList);
            var result = rpcAuthService.Authenticate(id, password);
            if (result.IsFailure())
                throw new ValidationException($"Rpc登录认证失败, Code : {result.Value} ; Message : {result.Message}");
            var identify = result.Value;
            return factory.Authenticate(idGenerator.Generate(), identify, identify.Id, identify.ServiceType,
                DateTimeOffset.Now.ToUnixTimeMilliseconds());
        }
    }

}
