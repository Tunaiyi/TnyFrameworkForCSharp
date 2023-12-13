// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using TnyFramework.Common.Result;
using TnyFramework.Net.Base;
using TnyFramework.Net.Message;
using TnyFramework.Net.Rpc;
using TnyFramework.Net.Transport;

namespace TnyFramework.Net.Command.Dispatcher
{

    public class RpcInvokeContext : IRpcInvokeContext
    {
        private readonly RpcForwardHeader? forward;

        private readonly INetAppContext appContext;

        public RpcInvokeContext(MethodControllerHolder controller, IRpcEnterContext rpcContext, INetAppContext appContext)
        {
            Controller = controller;
            RpcContext = rpcContext;
            this.appContext = appContext;
            var message = rpcContext.NetMessage;
            forward = message.GetHeader(MessageHeaderKeys.RPC_FORWARD_HEADER);
        }

        public MethodControllerHolder Controller { get; }

        public IRpcEnterContext RpcContext { get; }

        public string Name => Controller.SimpleName;

        public string AppType => appContext.AppType;

        public string ScopeType => appContext.AppType;

        public IMessage Message => RpcContext.NetMessage;

        public ITunnel Tunnel => RpcContext.NetTunnel;

        public IContactType ContactType {
            get {
                var servicer = forward?.From;
                return servicer != null ? servicer.ServiceType : Tunnel.ContactType;
            }
        }

        public object? Result { get; private set; }

        public Exception? Cause { get; private set; }

        public bool Done { get; private set; }

        public void Complete(object? result = null)
        {
            if (Done)
            {
                return;
            }
            if (result is Exception cause)
            {
                Cause = cause;
                Done = true;
            } else
            {
                Result = result;
                Done = true;
            }
        }

        public void Intercept(IRpcResult result)
        {

            Complete(result);
        }

        public void Intercept(IResultCode code)
        {
            Complete(code);
        }

        public void Intercept(IResultCode code, object body)
        {
            Complete(RpcResults.Result(code, body));
        }
    }

}
