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

    public class TickMessageHead : INetMessageHead
    {
        public static TickMessageHead Ping()
        {
            return new TickMessageHead(MessageMode.Ping);
        }

        public static TickMessageHead Pong()
        {
            return new TickMessageHead(MessageMode.Pong);
        }

        public long ToMessage => MessageConstants.EMPTY_MESSAGE_ID;

        public MessageType Type { get; }

        public MessageMode Mode { get; }

        public int ProtocolId { get; }

        public int Line => 0;

        private TickMessageHead(MessageMode mode)
        {
            Mode = mode;
            ProtocolId = Protocols.PING_PONG_PROTOCOL_NUM;
            Time = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        }

        public bool IsOwn(IProtocol protocol)
        {
            return ProtocolId == protocol.ProtocolId;
        }

        public long Id => 0;

        public int Code => ResultConstants.SUCCESS_CODE;

        public long Time { get; }

        public T GetHeader<T>(string key) where T : MessageHeader => default;

        public MessageHeader GetHeader(string key, Type type) => null;

        public IList<T> GetHeaders<T>() where T : MessageHeader => ImmutableList<T>.Empty;

        public MessageHeader GetHeader(string key) => null;

        public IList<MessageHeader> GetHeaders(Type type) => ImmutableList<MessageHeader>.Empty;

        public T GetHeader<T>(MessageHeaderKey<T> key) where T : MessageHeader<T> => default;

        public bool IsHasHeaders() => false;

        public IDictionary<string, MessageHeader> GetAllHeaderMap() => ImmutableDictionary<string, MessageHeader>.Empty;

        public bool IsForward() => false;

        public RpcForwardHeader ForwardHeader => null;

        public IList<MessageHeader> GetAllHeaders() => ImmutableList<MessageHeader>.Empty;

        public bool ExistHeader(string key) => false;

        public bool ExistHeader<T>(string key) where T : MessageHeader<T> => false;

        public bool ExistHeader(MessageHeaderKey key) => false;

        public bool ExistHeader<T>(MessageHeaderKey<T> key) where T : MessageHeader<T> => false;

        public T PutHeader<T>(MessageHeader<T> header) where T : MessageHeader<T> => null;

        public T PutHeaderIfAbsent<T>(MessageHeader<T> header) where T : MessageHeader<T> => null;

        public bool RemoveHeader<T>(string key) => false;

        public bool RemoveHeader<T>(MessageHeaderKey<T> key) where T : MessageHeader<T> => false;

        public void RemoveHeaders(IEnumerable<string> keys)
        {
        }

        public void RemoveAllHeaders()
        {
        }

        public void AllotMessageId(long id)
        {
        }
    }

}
