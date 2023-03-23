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
        private readonly IList<IRpcMonitorResumeExecuteHandler> resumeExecuteHandlers = new List<IRpcMonitorResumeExecuteHandler>();
        private readonly IList<IRpcMonitorSuspendExecuteHandler> suspendExecuteHandlers = new List<IRpcMonitorSuspendExecuteHandler>();
        private readonly IList<IRpcMonitorSendHandler> sendHandlers = new List<IRpcMonitorSendHandler>();
        private readonly IList<IRpcMonitorTransferHandler> transferHandlers = new List<IRpcMonitorTransferHandler>();

        public RpcMonitor()
        {
        }

        public RpcMonitor(IList<IRpcMonitorHandler> handlers)
        {
            AddHandlers(handlers);
        }

        public RpcMonitor AddHandler(IRpcMonitorHandler handler)
        {
            if (handler is IRpcMonitorBeforeInvokeHandler bih)
                beforeInvokeHandlers.Add(bih);
            if (handler is IRpcMonitorAfterInvokeHandler aih)
                afterInvokeHandlers.Add(aih);
            if (handler is IRpcMonitorReceiveHandler rh)
                receiveHandlers.Add(rh);
            if (handler is IRpcMonitorSendHandler sh)
                sendHandlers.Add(sh);
            if (handler is IRpcMonitorTransferHandler th)
                transferHandlers.Add(th);
            if (handler is IRpcMonitorResumeExecuteHandler reh)
                resumeExecuteHandlers.Add(reh);
            if (handler is IRpcMonitorSuspendExecuteHandler seh)
                suspendExecuteHandlers.Add(seh);
            return this;
        }

        public RpcMonitor AddHandlers(IEnumerable<IRpcMonitorHandler> handlers)
        {
            foreach (var handler in handlers)
            {
                AddHandler(handler);
            }
            return this;

        }

        public void OnReceive(IRpcEnterContext rpcContext)
        {
            var tunnel = rpcContext.NetTunnel;
            var message = rpcContext.NetMessage;
            if (receiveHandlers.Count > 0)
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

        public void OnBeforeInvoke(IRpcTransactionContext rpcContext)
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

        public void OnAfterInvoke(IRpcTransactionContext rpcContext, IMessageSubject result, Exception exception)
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

        public void OnResume(IRpcEnterContext rpcContext)
        {
            if (resumeExecuteHandlers.Count <= 0)
                return;
            foreach (var handler in resumeExecuteHandlers)
            {
                try
                {
                    handler.OnResume(rpcContext);
                } catch (Exception e)
                {
                    LOGGER.LogError(e, "RpcMonitor.OnResume");
                }
            }
        }

        public void OnSuspend(IRpcEnterContext rpcContext)
        {
            if (suspendExecuteHandlers.Count <= 0)
                return;
            foreach (var handler in suspendExecuteHandlers)
            {
                try
                {
                    handler.OnSuspend(rpcContext);
                } catch (Exception e)
                {
                    LOGGER.LogError(e, "RpcMonitor.OnSuspend");
                }
            }
        }

        public void OnTransfer(IRpcTransferContext rpcContext)
        {
            if (transferHandlers.Count <= 0)
                return;
            foreach (var handler in transferHandlers)
            {
                try
                {
                    handler.OnTransfer(rpcContext);
                } catch (Exception e)
                {
                    LOGGER.LogError(e, "RpcMonitor.OnTransfer");
                }
            }
        }

        public void OnTransfered(IRpcTransferContext rpcContext, IMessageSubject result, Exception exception)
        {
            if (transferHandlers.Count <= 0)
                return;
            foreach (var handler in transferHandlers)
            {
                try
                {
                    handler.OnTransfered(rpcContext, result, exception);
                } catch (Exception e)
                {
                    LOGGER.LogError(e, "RpcMonitor.OnTransfered");
                }
            }
        }
    }

}
