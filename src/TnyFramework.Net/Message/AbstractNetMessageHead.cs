// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System.Collections.Generic;

namespace TnyFramework.Net.Message
{

    public abstract class AbstractNetMessageHead : MessageHeaderContainer, INetMessageHead
    {
        protected AbstractNetMessageHead()
        {
        }

        protected AbstractNetMessageHead(IDictionary<string, MessageHeader> headers) : base(headers)
        {
        }

        public MessageType Type => Mode.GetMessageType();

        public abstract MessageMode Mode { get; }

        public abstract long ToMessage { get; }

        public abstract int ProtocolId { get; }

        public abstract int Line { get; }

        public abstract bool IsOwn(IProtocol protocol);

        public abstract long Id { get; }

        public abstract int Code { get; }

        public abstract long Time { get; }

        public abstract void AllotMessageId(long id);
    }

}
