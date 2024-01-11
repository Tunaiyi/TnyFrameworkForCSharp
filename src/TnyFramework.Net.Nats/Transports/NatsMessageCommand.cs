// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using TnyFramework.Net.Message;

namespace TnyFramework.Net.Nats.Transports
{

    public class NatsMessageCommand : NatsCommand<NatsMessageCommand>
    {
        private IMessage? message;

        private NatsAction action = null!;

        public static NatsCommand Connect(NatsTransport transport)
        {
            var command = Get();
            command.Init(transport, NatsAction.CONNECT);
            return command;
        }

        public static NatsCommand Connected(NatsTransport transport)
        {
            var command = Get();
            command.Init(transport, NatsAction.CONNECTED);
            return command;
        }

        public static NatsCommand Connected(string topic, string accessKey)
        {
            var command = Get();
            command.Init(topic, accessKey, NatsAction.CONNECTED);
            return command;
        }

        public static NatsCommand Close(NatsTransport transport)
        {
            var command = Get();
            command.Init(transport, NatsAction.CLOSE);
            return command;
        }

        public static NatsCommand Ping(NatsTransport transport)
        {
            var command = Get();
            command.Init(transport, NatsAction.PING);
            return command;
        }

        public static NatsCommand Pong(NatsTransport transport)
        {
            var command = Get();
            command.Init(transport, NatsAction.PONE);
            return command;
        }

        public static NatsCommand Close(string topic, string accessKey)
        {
            var command = Get();
            command.Init(topic, accessKey, NatsAction.CLOSE);
            return command;
        }

        public static NatsCommand MessageCommand(NatsTransport transport, IMessage? message)
        {
            var command = Get();
            command.Init(transport, NatsAction.MESSAGE, message);
            return command;
        }

        private void Init(string topic, string accessKey, NatsAction action, IMessage? message = null)
        {
            Init(topic, accessKey);
            this.message = message;
            this.action = action;
        }

        private void Init(NatsTransport transport, NatsAction action, IMessage? message = null)
        {
            Init(transport);
            this.message = message;
            this.action = action;
        }

        public override IMessage? Message => message;

        public override NatsAction Action => action;

        protected override void DoClear()
        {
            message = null!;
            action = null!;
        }
    }

}
