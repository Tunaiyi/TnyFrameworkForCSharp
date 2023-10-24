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

    public abstract class RpcTransactionContext : AttributesContext, IRpcTransactionContext
    {
        private const int INIT = 0;
        private const int OPEN = 1;
        private const int CLOSE = 2;

        public static IRpcEnterContext CreateEnter(INetTunnel tunnel, INetMessage message, bool async = true)
        {
            return new RpcEnterInvocationContext(tunnel, message, async);
        }

        public static IRpcConsumerContext CreateExit(IEndpoint endpoint, MessageContent content, RpcMonitor rpcMonitor, bool async = true)
        {
            return new RpcExitInvocationContext(endpoint, content, async, rpcMonitor);
        }

        // public static string ForwardOperation(IMessageSubject message)
        // {
        //     return $"forward[{message.ProtocolId}]{message.Mode.Mark()}";
        // }
        //
        // public static string RelayOperation(IMessageSubject message)
        // {
        //     return $"relay[{message.ProtocolId}]{message.Mode.Mark()}";
        // }
        //
        // public static string RpcOperation(Type operation, string method, IMessageSubject message)
        // {
        //     return $"[{message.ProtocolId}@{message.Mode.Mark()}]{operation.Name}.{method}";
        // }

        public static string RpcOperation(string method, IMessageSubject message)
        {
            return $"[{message.ProtocolId}@{message.Mode.Mark()}]{method}";
        }

        public static string ReturnOperation(IMessageSubject message)
        {
            return $"return[{message.ProtocolId}@{message.Mode.Mark()}]";
        }

        public static string ErrorOperation(IMessageSchema message)
        {
            return $"error[{message.ProtocolId}@{message.Mode.Mark()}]";
        }

        private int status = INIT;

        protected RpcTransactionContext(bool async,  IAttributes? attributes = null) : base(attributes)
        {
            Async = async;
            OperationName = null!;
            Cause = null!;
        }

        public string? OperationName { get; private set; }

        public Exception? Cause { get; private set; }

        public bool Async { get; }

        public IAsyncExecutor Executor => GetEndpoint();

        public bool Completed => status == CLOSE;

        public abstract IMessageSubject MessageSubject { get; }

        public abstract RpcTransactionMode Mode { get; }

        public abstract INetMessager Messager { get; }

        public abstract bool Valid { get; }

        public abstract bool Complete();

        public abstract IEndpoint GetEndpoint();

        public bool Prepare(string? operationName, Action? action = null)
        {
            if (!Valid)
                return false;
            if (status != INIT)
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
                    OperationName = ErrorOperation(MessageSubject);
                }
            }
            if (Interlocked.CompareExchange(ref status, OPEN, INIT) != INIT)
                return false;
            action?.Invoke();
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

        protected bool TryCompleted(Exception? error = null)
        {
            if (!Valid)
                return false;
            if (status == INIT)
            {
                Prepare(null);
            }
            if (Interlocked.CompareExchange(ref status, CLOSE, OPEN) != OPEN)
                return false;
            Cause = error;
            return true;
        }

        public bool IsError()
        {
            return Cause != null;
        }

        protected abstract void OnPrepare();

        protected abstract void OnComplete();
    }

}
