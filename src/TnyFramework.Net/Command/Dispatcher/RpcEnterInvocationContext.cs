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
using TnyFramework.Net.Base;
using TnyFramework.Net.Command.Dispatcher.Monitor;
using TnyFramework.Net.Endpoint;
using TnyFramework.Net.Message;
using TnyFramework.Net.Rpc;
using TnyFramework.Net.Transport;

namespace TnyFramework.Net.Command.Dispatcher
{

    public class RpcEnterInvocationContext : CompletableRpcTransactionContext, IRpcEnterContext
    {
        private const int OPEN = 0;
        private const int CLOSE = 1;

        private readonly INetTunnel tunnel;

        private readonly RpcMonitor rpcMonitor;

        private volatile int running = CLOSE;

        private INetContact to;

        private bool forward;

        public override IMessageSubject MessageSubject => message;

        public override RpcTransactionMode Mode => RpcTransactionMode.Enter;

        public RpcMonitor RpcMonitor => rpcMonitor;

        public INetworkContext NetworkContext => tunnel.Context;

        public INetTunnel NetTunnel => tunnel;

        public override INetContact Contact => tunnel;

        public IMessage Message => message;

        public INetMessage NetMessage => message;

        public override bool Valid => tunnel.IsNull() && Message.IsNull();

        public INetContact From => tunnel;

        public INetContact To => to;

        public RpcEnterInvocationContext(INetTunnel tunnel, INetMessage message, bool async, IAttributes? attributes = null)
            : base(message, async, attributes)
        {
            this.tunnel = tunnel;
            to = null!;
            if (tunnel.IsNotNull())
            {
                rpcMonitor = tunnel.Context.RpcMonitor;
            } else
            {
                rpcMonitor = null!;
            }
        }

        public bool Invoke(string operationName)
        {
            return Prepare(operationName);
        }

        public override IEndpoint GetEndpoint() => tunnel.GetEndpoint();

        public bool Resume()
        {
            if (!Valid || Interlocked.CompareExchange(ref running, CLOSE, OPEN) != CLOSE)
            {
                return false;
            }
            rpcMonitor.OnResume(this);
            return true;
        }

        public bool Suspend()
        {
            if (!Valid || Interlocked.CompareExchange(ref running, OPEN, CLOSE) != OPEN)
            {
                return false;
            }
            rpcMonitor.OnSuspend(this);
            return true;
        }

        public bool Running() => running == OPEN;

        bool IRpcTransferContext.Transfer(INetContact toContact, string operationName)
        {
            return Prepare(operationName, () => {
                to = toContact;
                forward = true;
            });
        }

        protected override void OnPrepare()
        {
            if (forward)
            {
                rpcMonitor.OnTransfer(this);
            } else
            {
                rpcMonitor.OnBeforeInvoke(this);
            }
        }

        protected override void OnComplete(MessageContent? result, Exception? exception)
        {
            if (forward)
            {
                rpcMonitor.OnTransferred(this, result, exception);
            } else
            {
                rpcMonitor.OnAfterInvoke(this, result, exception);
            }
        }

        protected override void OnReturn(MessageContent content)
        {
            RpcMessageAide.Send(tunnel, content);
        }
    }

}
