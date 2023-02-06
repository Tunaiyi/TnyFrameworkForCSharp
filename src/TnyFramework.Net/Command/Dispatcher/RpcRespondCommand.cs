// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.Threading.Tasks;
using TnyFramework.Common.Result;

namespace TnyFramework.Net.Command.Dispatcher
{

    public class RpcRespondCommand : BaseCommand, ICommand
    {
        private IRpcProviderContext rpcContext;

        private readonly IResultCode code;

        private readonly object body;

        public RpcRespondCommand(IRpcProviderContext rpcContext, IResultCode code, object body)
        {
            this.rpcContext = rpcContext;
            this.code = code;
            this.body = body;

        }

        protected override Task Action()
        {
            RpcContexts.SetCurrent(rpcContext);
            try
            {
                var message = rpcContext.NetMessage;
                rpcContext.Prepare(RpcInvocationContexts.ErrorOperation(message));
                rpcContext.Complete(code, body);
                
            } catch (Exception cause)
            {
                rpcContext.Complete(cause);
            } finally
            {
                RpcContexts.Clear();
            }
            return Task.CompletedTask;
        }
    }

}
