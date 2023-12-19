// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TnyFramework.Common.Logger;

namespace TnyFramework.Net.Command.Dispatcher
{

    public class RpcNoopCommand : Command
    {
        private static readonly ILogger LOGGER = LogFactory.Logger<RpcNoopCommand>();

        private readonly IRpcMessageEnterContext rpcMessageContext;

        public RpcNoopCommand(IRpcMessageEnterContext rpcMessageContext)
        {
            this.rpcMessageContext = rpcMessageContext;
        }

        protected override Task Action()
        {
            RpcContexts.SetCurrent(rpcMessageContext);
            try
            {
                var message = rpcMessageContext.Message;
                rpcMessageContext.Invoke(RpcMessageTransactionContext.ReturnOperation(message));
                rpcMessageContext.CompleteSilently();
            } catch (Exception cause)
            {
                LOGGER.LogError(cause, "");
                rpcMessageContext.Complete(cause);
            } finally
            {
                RpcContexts.Clear();
            }
            return Task.CompletedTask;
        }
    }

}
