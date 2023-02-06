// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using TnyFramework.Common.Attribute;
using TnyFramework.Common.Exceptions;
using TnyFramework.Common.Result;
using TnyFramework.Net.Base;
using TnyFramework.Net.Command.Dispatcher.Monitor;
using TnyFramework.Net.Common;
using TnyFramework.Net.Endpoint;
using TnyFramework.Net.Exceptions;
using TnyFramework.Net.Message;
using TnyFramework.Net.Rpc;
using TnyFramework.Net.Transport;

namespace TnyFramework.Net.Command.Dispatcher
{

    public class RpcProviderInvocationContext : RpcInvocationContext, IRpcProviderContext
    {
        private readonly INetMessage message;

        private readonly INetTunnel tunnel;

        private readonly RpcMonitor rpcMonitor;

        private bool silently;

        public override IMessageSubject MessageSubject => message;

        public override RpcInvocationMode InvocationMode => RpcInvocationMode.Enter;

        public RpcProviderInvocationContext(INetTunnel tunnel, INetMessage message, IAttributes attributes = null)
            : base(attributes)
        {
            this.tunnel = tunnel;
            this.message = message;
            rpcMonitor = tunnel?.Context.RpcMonitor;
        }

        public override IEndpoint GetEndpoint() => tunnel.GetEndpoint();

        public override bool IsEmpty() => tunnel == null || message == null;

        public INetMessage NetMessage => message;

        public INetTunnel NetTunnel => tunnel;

        public RpcMonitor RpcMonitor => rpcMonitor;

        public INetworkContext NetworkContext => tunnel.Context;

        public bool CompleteSilently(Exception error = null)
        {
            if (!TryCompleted(error))
                return false;
            silently = true;
            OnComplete();
            return true;
        }

        public bool CompleteSilently(IResultCode code, object body = null)
        {
            if (!TryCompleted())
                return false;
            silently = true;
            OnComplete(code, body);
            return true;
        }

        public override bool Complete()
        {
            return Complete(ResultCode.SUCCESS);
        }

        public bool Complete(IResultCode code)
        {
            if (!TryCompleted())
                return false;
            OnComplete(code, null);
            return true;
        }

        public bool Complete(IResultCode code, object body, Exception error = null)
        {
            if (!TryCompleted(error))
                return false;
            OnComplete(code, body);
            return true;
        }

        public bool Complete(MessageContent content, Exception error)
        {
            if (!TryCompleted(error))
                return false;
            OnComplete(content);
            return true;
        }

        protected override void OnPrepare()
        {
            rpcMonitor.OnBeforeInvoke(this);
        }

        protected override void OnComplete()
        {
            if (silently)
            {
                OnComplete(null);
                return;
            }
            object body = null;
            var code = NetResultCode.SERVER_ERROR;
            var cause = Cause;
            switch (cause)
            {
                case null:
                    return;
                case NetException ne:
                    body = ne.Body;
                    code = ne.Code;
                    break;
                case ResultCodeException rce:
                    code = rce.Code;
                    break;
            }
            OnComplete(code, body);
        }

        private void OnComplete(IResultCode code, object body)
        {
            if (silently)
            {
                OnComplete(null);
                return;
            }
            MessageContent content = null;
            switch (message.Mode)
            {
                case MessageMode.Response:
                case MessageMode.Push:
                    if (body != null)
                    {
                        content = RpcMessageAide.ToMessage(message, code, body);
                    }
                    break;
                case MessageMode.Request:
                    content = RpcMessageAide.ToMessage(message, code, body);
                    break;
            }
            OnComplete(content);
        }

        private void OnComplete(MessageContent content)
        {
            var cause = Cause;
            if (silently)
            {
                content = null;
            }
            rpcMonitor.OnAfterInvoke(this, content, cause);
            if (content != null)
            {
                RpcMessageAide.Send(tunnel, content);
            }
        }
    }

}
