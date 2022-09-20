// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using TnyFramework.Net.Dispatcher;
using TnyFramework.Net.Message;
using TnyFramework.Net.Transport;

namespace TnyFramework.Net.Command.Tasks
{

    public class MessageCommandTask : ICommandTask
    {
        private readonly IMessage message;

        private readonly INetTunnel tunnel;

        private readonly IMessageDispatcher messageDispatcher;

        public MessageCommandTask(IMessage message, INetTunnel tunnel, IMessageDispatcher messageDispatcher)
        {
            this.message = message;
            this.tunnel = tunnel;
            this.messageDispatcher = messageDispatcher;
        }

        public ICommand Command {
            get {
                // if (message.ExistHeader(MessageHeaderConstants.RPC_FORWARD_HEADER))
                // {
                //     if (tunnel is INetTunnel<RpcAccessIdentify> netTunnel)
                //     {
                //         return new MessageForwardCommand(netTunnel, message);
                //     }
                // }
                switch (message.Mode)
                {
                    case MessageMode.Push:
                    case MessageMode.Request:
                    case MessageMode.Response:
                        return messageDispatcher.Dispatch(tunnel, message);
                    case MessageMode.Ping:
                        return new RunnableCommand(() => tunnel.Pong());
                    case MessageMode.Pong:
                    default: return null;
                }
            }
        }
    }

}
