// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using Newtonsoft.Json;
using TnyFramework.Common.Result;
using TnyFramework.Net.Base;
using TnyFramework.Net.Common;

namespace TnyFramework.Net.Rpc.Auth
{

    public class RpcAuthService : IRpcAuthService
    {
        private readonly INetAppContext netAppContext;

        private readonly IRpcUserPasswordManager rpcUserPasswordManager;

        public RpcAuthService(INetAppContext netAppContext, IRpcUserPasswordManager rpcUserPasswordManager)
        {
            this.netAppContext = netAppContext;
            this.rpcUserPasswordManager = rpcUserPasswordManager;
        }

        public IDoneResult<RpcAccessIdentify> Authenticate(long id, string password)
        {
            var identify = RpcAccessIdentify.Parse(id);
            return rpcUserPasswordManager.Auth(identify, password)
                ? DoneResults.Success(identify)
                : DoneResults.Failure<RpcAccessIdentify>(NetResultCode.AUTH_FAIL_ERROR);
        }

        public string CreateToken(RpcServiceType serviceType, RpcAccessIdentify user)
        {
            var token = new RpcAccessToken(serviceType, netAppContext.ServerId, user);
            return JsonConvert.SerializeObject(token);
        }

        public IDoneResult<RpcAccessToken> VerifyToken(string token)
        {
            var rpcToken = JsonConvert.DeserializeObject<RpcAccessToken>(token);
            return DoneResults.Success(rpcToken);
        }
    }

}
