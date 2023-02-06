// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using TnyFramework.Common.Logger;
using TnyFramework.Net.Base;
using TnyFramework.Net.Message;
using TnyFramework.Net.Transport;

namespace TnyFramework.Net.Command.Dispatcher.Monitor
{

    public class RpcMonitor
    {
        private static readonly ILogger LOGGER = LogFactory.Logger<RpcMonitor>();

        private readonly IList<IRpcMonitorBeforeInvokeHandler> beforeInvokeHandlers = new List<IRpcMonitorBeforeInvokeHandler>();
        private readonly IList<IRpcMonitorAfterInvokeHandler> afterInvokeHandlers = new List<IRpcMonitorAfterInvokeHandler>();
        private readonly IList<IRpcMonitorReceiveHandler> receiveHandlers = new List<IRpcMonitorReceiveHandler>();
        private readonly IList<IRpcMonitorSendHandler> sendHandlers = new List<IRpcMonitorSendHandler>();

        public RpcMonitor(IList<IRpcMonitorHandler> listeners)
        {
            foreach (var handler in listeners)
            {
                switch (handler)
                {
                    case IRpcMonitorBeforeInvokeHandler bih:
                        beforeInvokeHandlers.Add(bih);
                        break;
                    case IRpcMonitorAfterInvokeHandler aih:
                        afterInvokeHandlers.Add(aih);
                        break;
                    case IRpcMonitorReceiveHandler rh:
                        receiveHandlers.Add(rh);
                        break;
                    case IRpcMonitorSendHandler sh:
                        sendHandlers.Add(sh);
                        break;
                }
            }
        }

        public void OnReceive(IRpcProviderContext rpcContext)
        {
            var tunnel = rpcContext.NetTunnel;
            var message = rpcContext.NetMessage;
            if (sendHandlers.Count > 0)
            {
                foreach (var handler in receiveHandlers)
                {
                    try
                    {
                        handler.OnReceive(rpcContext);
                    } catch (Exception e)
                    {
                        LOGGER.LogError(e, "RpcMonitor.OnReceive");
                    }
                }
            }
            NetLogger.LogReceive(tunnel, message);
        }

        public void OnSend(INetTunnel tunnel, IMessage message)
        {
            NetLogger.LogSend(tunnel, message);
            if (sendHandlers.Count <= 0)
                return;
            foreach (var handler in sendHandlers)
            {
                try
                {
                    handler.OnSend(tunnel, message);
                } catch (Exception e)
                {
                    LOGGER.LogError(e, "RpcMonitor.OnSend");
                }
            }
        }

        public void OnBeforeInvoke(IRpcContext rpcContext)
        {
            if (beforeInvokeHandlers.Count <= 0)
                return;
            foreach (var handler in beforeInvokeHandlers)
            {
                try
                {
                    handler.OnBeforeInvoke(rpcContext);
                } catch (Exception e)
                {
                    LOGGER.LogError(e, "RpcMonitor.OnBeforeInvoke");
                }
            }
        }

        public void OnAfterInvoke(IRpcContext rpcContext, IMessageSubject result, Exception exception)
        {
            if (afterInvokeHandlers.Count <= 0)
                return;
            foreach (var handler in afterInvokeHandlers)
            {
                try
                {
                    handler.OnAfterInvoke(rpcContext, result, exception);
                } catch (Exception e)
                {
                    LOGGER.LogError(e, "RpcMonitor.OnBeforeInvoke");
                }
            }
        }
    }

}
