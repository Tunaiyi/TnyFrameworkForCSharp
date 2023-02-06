// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.Threading;
using TnyFramework.Common.Attribute;
using TnyFramework.Common.Extensions;
using TnyFramework.Coroutines.Async;
using TnyFramework.Net.Command.Dispatcher.Monitor;
using TnyFramework.Net.Endpoint;
using TnyFramework.Net.Message;
using TnyFramework.Net.Rpc;
using TnyFramework.Net.Transport;

namespace TnyFramework.Net.Command.Dispatcher
{

    public abstract class RpcInvocationContext : AttributesContext, IRpcInvocationContext
    {
        public static IRpcConsumerContext CreateConsumer(IEndpoint endpoint, MessageContent content,
            RpcMonitor rpcMonitor)
        {
            return new RpcConsumerInvocationContext(endpoint, content, rpcMonitor);
        }

        public static IRpcProviderContext CreateProvider(INetTunnel tunnel, INetMessage message)
        {
            return new RpcProviderInvocationContext(tunnel, message);
        }

        private const int CLOSE = 0;
        private const int OPEN = 1;

        private int completed = CLOSE;

        private int prepared = CLOSE;

        protected RpcInvocationContext(IAttributes attributes = null) : base(attributes)
        {
        }

        public string OperationName { get; private set; }

        public Exception Cause { get; private set; }

        public IAsyncExecutor Executor => GetEndpoint();

        public abstract IMessageSubject MessageSubject { get; }

        public abstract RpcInvocationMode InvocationMode { get; }

        public abstract bool Complete();

        public abstract IEndpoint GetEndpoint();

        public abstract bool IsEmpty();

        public bool Prepare(string operationName = null)
        {
            if (IsEmpty())
                return false;
            if (prepared == OPEN)
            {
                return false;
            }
            if (OperationName.IsBlank())
            {
                if (operationName.IsNotBlank())
                {
                    OperationName = operationName;
                } else if (IsError())
                {
                    OperationName = RpcInvocationContexts.ErrorOperation(MessageSubject);
                }
            }
            if (Interlocked.CompareExchange(ref prepared, CLOSE, OPEN) != CLOSE)
                return false;
            OnPrepare();
            return true;
        }

        public bool Complete(Exception error)
        {
            if (!TryCompleted(error))
                return false;
            OnComplete();
            return true;
        }

        public bool IsError()
        {
            return Cause != null;
        }

        protected bool TryCompleted(Exception error = null)
        {
            if (IsEmpty())
                return false;
            if (Interlocked.CompareExchange(ref completed, CLOSE, OPEN) != CLOSE)
                return false;
            Cause = error;
            Prepare();
            return true;
        }

        protected abstract void OnPrepare();

        protected abstract void OnComplete();
    }

}
