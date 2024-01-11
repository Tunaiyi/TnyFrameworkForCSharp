// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using Microsoft.Extensions.ObjectPool;
using TnyFramework.Net.Message;
using TnyFramework.Net.Session;

namespace TnyFramework.Net.Nats.Transports
{

    internal class NatsCommandObjectPolicy<T> : IPooledObjectPolicy<T> where T : class, INatsCommand, new()
    {
        public T Create()
        {
            return new T();
        }

        public bool Return(T obj)
        {
            obj.Clear();
            return true;
        }
    }

    public class NatsContentCommand : NatsCommand<NatsContentCommand>
    {
        private MessageAllocator allocator = null!;

        private MessageContent content = null!;

        private NatsTransport transport = null!;

        private IMessage? message = null!;

        public static NatsCommand ContentCommand(NatsTransport transport, MessageAllocator allocator,
            MessageContent content)
        {
            var command = Get();
            command.Init(transport, allocator, content);
            return command;
        }

        private void Init(NatsTransport transport, MessageAllocator allocator, MessageContent content)
        {
            Init(transport);
            this.transport = transport;
            this.allocator = allocator;
            this.content = content;
            message = null;
        }

        public override IMessage Message => message ??= allocator(transport.MessageFactory, content);

        public override NatsAction Action => NatsAction.MESSAGE;

        protected override void DoClear()
        {
            transport = null!;
            message = null!;
            content = null!;
            allocator = null!;
        }
    }

}
