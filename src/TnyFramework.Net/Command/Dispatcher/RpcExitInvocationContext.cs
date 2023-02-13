// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using TnyFramework.Net.Command.Dispatcher.Monitor;
using TnyFramework.Net.Endpoint;
using TnyFramework.Net.Message;
using TnyFramework.Net.Rpc;
using TnyFramework.Net.Transport;

namespace TnyFramework.Net.Command.Dispatcher
{

    public class RpcExitInvocationContext : RpcTransactionContext, IRpcConsumerContext
    {
        private readonly MessageContent content;

        private readonly RpcMonitor rpcMonitor;

        private readonly IEndpoint endpoint;

        public RpcExitInvocationContext(IEndpoint endpoint, MessageContent content, bool async, RpcMonitor rpcMonitor)
            : base(async)
        {
            this.endpoint = endpoint;
            this.content = content;
            this.rpcMonitor = rpcMonitor;
        }

        public override IMessageSubject MessageSubject => content;

        public override RpcTransactionMode Mode => RpcTransactionMode.Exit;

        public override INetMessager Messager => endpoint;

        public override bool Valid => true;

        public override IEndpoint GetEndpoint() => endpoint;

        public bool Invoke(string operationName)
        {
            return Prepare(operationName);
        }

        public override bool Complete()
        {
            if (!TryCompleted())
                return false;
            OnComplete();
            return true;
        }

        public bool Complete(IMessage message)
        {
            if (!TryCompleted())
                return false;
            OnComplete(message);
            return true;
        }

        protected override void OnPrepare()
        {
            rpcMonitor.OnBeforeInvoke(this);
        }

        protected override void OnComplete()
        {
            rpcMonitor.OnAfterInvoke(this, content, Cause);
        }

        private void OnComplete(IMessageSubject message)
        {
            rpcMonitor.OnAfterInvoke(this, message, Cause);
        }
    }

}
