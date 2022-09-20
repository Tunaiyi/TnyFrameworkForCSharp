// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using TnyFramework.Common.Result;

namespace TnyFramework.Net.Message
{

    public class TickMessageHead : AbstractNetMessageHead
    {
        public static TickMessageHead Ping()
        {
            return new TickMessageHead(MessageMode.Ping);
        }

        public static TickMessageHead Pong()
        {
            return new TickMessageHead(MessageMode.Pong);
        }

        public TickMessageHead(MessageMode mode) : base(mode)
        {
            ProtocolId = Protocols.PING_PONG_PROTOCOL_NUM;
            Time = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        }

        public override long ToMessage => MessageConstants.EMPTY_MESSAGE_ID;

        public override int ProtocolId { get; }

        public override int Line => 0;

        public override bool IsOwn(IProtocol protocol)
        {
            return ProtocolId == protocol.ProtocolId;
        }

        public override long Id => 0;

        public override int Code => ResultConstants.SUCCESS_CODE;

        public override long Time { get; }

        public override T GetHeader<T>(string key) => null;

        public override MessageHeader GetHeader(string key, Type type) => null;

        public override MessageHeader GetHeader(string key) => null;

        public override IList<T> GetHeaders<T>() => ImmutableList<T>.Empty;

        public override IList<MessageHeader> GetHeaders(Type type) => ImmutableList<MessageHeader>.Empty;

        public override T GetHeader<T>(MessageHeaderKey<T> key) => null;

        public override bool IsHasHeaders => false;

        public override bool IsForward() => false;

        public override RpcForwardHeader ForwardHeader => null;

        public override IList<MessageHeader> GetAllHeaders() => ImmutableList<MessageHeader>.Empty;

        public override IDictionary<string, MessageHeader> GetAllHeadersMap() => ImmutableDictionary<string, MessageHeader>.Empty;

        public override bool ExistHeader(string key) => false;

        public override bool ExistHeader<T>(string key) => false;

        public override bool ExistHeader(MessageHeaderKey key) => false;

        public override void AllotMessageId(long id)
        {
        }
    }

}
