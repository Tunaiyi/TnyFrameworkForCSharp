using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using TnyFramework.Net.Command;
using TnyFramework.Net.Exceptions;
using TnyFramework.Net.Message;
using TnyFramework.Net.Transport;
namespace TnyFramework.Net.Rpc.Auth
{
    public class RpcPasswordValidator : IAuthenticateValidator<IRpcLinkerId>
    {
        private readonly IRpcAuthService rpcAuthService;

        private readonly IIdGenerator idGenerator;

        public IList<int> AuthProtocolLimit { get; } = ImmutableList.Create<int>();


        public RpcPasswordValidator(IIdGenerator idGenerator,  IRpcAuthService rpcAuthService)
        {
            this.idGenerator = idGenerator;
            this.rpcAuthService = rpcAuthService;
        }




        public ICertificate Validate(ITunnel tunnel, IMessage message, ICertificateFactory certificateFactory)
        {
            return Validate((ITunnel<IRpcLinkerId>)tunnel, message, (ICertificateFactory<IRpcLinkerId>)certificateFactory);
        }


        public ICertificate<IRpcLinkerId> Validate(ITunnel<IRpcLinkerId> tunnel, IMessage message, ICertificateFactory<IRpcLinkerId> factory)
        {
            var paramList = message.BodyAs<MessageParamList>();
            if (paramList == null)
            {
                throw new ValidationException("Rpc登录参数错误");
            }
            var service = RpcAuthMessageContexts.GetServiceParam(paramList);
            var serverId = RpcAuthMessageContexts.GetServerIdParam(paramList);
            var instanceId = RpcAuthMessageContexts.GetInstanceIdParam(paramList);
            var password = RpcAuthMessageContexts.GetPasswordParam(paramList);
            var result = rpcAuthService.Authenticate(service, serverId, instanceId, password);
            if (result.IsSuccess())
            {
                return factory.Authenticate(idGenerator.Generate(), result.Value, service, DateTimeOffset.Now.ToUnixTimeMilliseconds());
            }
            throw new ValidationException($"Rpc登录认证失败, Code : {result.Value} ; Message : {result.Message}");
        }
    }
}
