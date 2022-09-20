// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System.Threading.Tasks;
using TnyFramework.Net.Message;
using TnyFramework.Net.Rpc;
using TnyFramework.Net.Transport;

namespace TnyFramework.Net.Dispatcher
{

    public class MessageForwardCommand : BaseCommand
    {
        private IMessage message;

        private IRpcContext rpcContext;

        private INetTunnel<RpcAccessIdentify> tunnel;

        public MessageForwardCommand(INetTunnel<RpcAccessIdentify> tunnel, IMessage message)
        {
            this.tunnel = tunnel;
            this.message = message;
            rpcContext = tunnel.Context;
        }

        protected override Task Action()
        {
            //TODO 实现转发逻辑
            return Task.CompletedTask;
        }
    }

}
