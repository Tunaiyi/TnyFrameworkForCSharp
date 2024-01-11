// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using Microsoft.Extensions.Logging;
using TnyFramework.Common.Logger;
using TnyFramework.Net.Attributes;
using TnyFramework.Net.Command.Dispatcher;
using TnyFramework.Net.Transport;

namespace TnyFramework.Net.Rpc.Auth;

[RpcController]
public class RpcAuthController(IRpcAuthService rpcAuthService) : IController
{
    private static readonly ILogger LOGGER = LogFactory.Logger<RpcAuthController>();

    [RpcRequest(RpcProtocol.RPC_AUTH_4_AUTHENTICATE)]
    [AuthenticationRequired(typeof(RpcPasswordValidator))]
    public IRpcResult<string> Authenticate(ITunnel tunnel, [IdentifyToken] RpcAccessIdentify id)
    {
        var serviceType = RpcServiceType.ForService(tunnel.Service);
        var token = rpcAuthService.CreateToken(serviceType, id);
        LOGGER.LogInformation("Rpc执行 << [{id}] 认证成功", id);
        return RpcResults.Success(token);
    }

    [RpcResponse(RpcProtocol.RPC_AUTH_4_AUTHENTICATE)]
    [AuthenticationRequired(typeof(RpcTokenValidator))]
    public void Authenticated([IdentifyToken] RpcAccessIdentify id)
    {
        LOGGER.LogInformation("Rpc响应 >> [{id}] 认证完成", id);
    }

}
