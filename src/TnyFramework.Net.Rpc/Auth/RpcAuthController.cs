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
using TnyFramework.Net.Base;
using TnyFramework.Net.Command;
using TnyFramework.Net.Dispatcher;

namespace TnyFramework.Net.Rpc.Auth
{

    [RpcController]
    public class RpcAuthController : IController
    {
        private readonly IRpcAuthService rpcAuthService;

        private static readonly ILogger LOGGER = LogFactory.Logger<RpcAuthController>();

        public RpcAuthController(IRpcAuthService rpcAuthService)
        {
            this.rpcAuthService = rpcAuthService;
        }

        [RpcRequest(RpcProtocol.RPC_AUTH_4_AUTHENTICATE)]
        [AuthenticationRequired(typeof(RpcPasswordValidator))]
        public IRpcResult<string> Authenticate(IServerBootstrapSetting setting, [UserId] RpcAccessIdentify id)
        {
            var serviceType = RpcServiceType.ForService(setting.ServiceName());
            var token = rpcAuthService.CreateToken(serviceType, id);
            LOGGER.LogInformation("接受 << [{id}] 认证成功", id);
            return RpcResults.Success(token);
        }

        [RpcResponse(RpcProtocol.RPC_AUTH_4_AUTHENTICATE)]
        [AuthenticationRequired(typeof(RpcTokenValidator))]
        public void Authenticated([UserId] RpcAccessIdentify id)
        {
            LOGGER.LogInformation("连接 >> [{id}] 认证完成", id);
        }
    }

}
