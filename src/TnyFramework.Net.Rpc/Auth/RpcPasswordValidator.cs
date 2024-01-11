// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System.Collections.Generic;
using System.Collections.Immutable;
using TnyFramework.Net.Command.Auth;
using TnyFramework.Net.Exceptions;
using TnyFramework.Net.Message;
using TnyFramework.Net.Transport;

namespace TnyFramework.Net.Rpc.Auth;

public class RpcPasswordValidator : IAuthenticationValidator
{
    private readonly IRpcAuthService rpcAuthService;

    private readonly IIdGenerator idGenerator;

    public IList<int> AuthProtocolLimit { get; } = ImmutableList.Create<int>();

    public RpcPasswordValidator(IIdGenerator idGenerator, IRpcAuthService rpcAuthService)
    {
        this.idGenerator = idGenerator;
        this.rpcAuthService = rpcAuthService;
    }

    public ICertificate Validate(ITunnel tunnel, IMessage message)
    {
        var paramList = message.BodyAs<MessageParamList>();
        if (paramList == null)
        {
            throw new AuthFailedException("Rpc登录参数错误");
        }
        var id = RpcAuthMessageContexts.GetIdParam(paramList);
        var password = RpcAuthMessageContexts.GetPasswordParam(paramList);
        var result = rpcAuthService.Authenticate(id, password);
        if (result.IsFailure())
            throw new AuthFailedException($"Rpc登录认证失败, Code : {result.Value} ; Message : {result.Message}");
        var identify = result.Value;
        return Certificates.CreateAuthenticated(idGenerator.Generate(), identify.Id, identify.ServerId, identify.ServiceType, identify);
    }
}
