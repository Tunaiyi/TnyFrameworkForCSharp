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
using System.Linq;
using TnyFramework.Common.Extensions;

namespace TnyFramework.Net.Message
{

    public class MessageHeaderContainer : IMessageHeaderContainer
    {
        private IDictionary<string, MessageHeader> Headers { get; }

        protected MessageHeaderContainer()
        {
            Headers = new Dictionary<string, MessageHeader>();
        }

        protected MessageHeaderContainer(IDictionary<string, MessageHeader> headers)
        {
            Headers = !headers.IsNullOrEmpty()
                ? new Dictionary<string, MessageHeader>(headers)
                : new Dictionary<string, MessageHeader>();
        }

        public MessageHeader? GetHeader(string key)
        {
            return GetHeader<MessageHeader>(key);
        }

        public T? GetHeader<T>(string key) where T : MessageHeader
        {
            var value = Headers.Get(key);
            if (value is T header)
                return header;
            return null;
        }

        public MessageHeader? GetHeader(string key, Type type)
        {
            var value = GetHeader(key);
            return type.IsInstanceOfType(value) ? value : null;
        }

        public IList<T> GetHeaders<T>() where T : MessageHeader
        {
            return Headers.Values.OfType<T>().ToList();
        }

        public IList<MessageHeader> GetHeaders(Type type)
        {
            return Headers.Values.Where(type.IsInstanceOfType).ToList();
        }

        public T? GetHeader<T>(MessageHeaderKey<T> key) where T : MessageHeader<T>
        {
            return GetHeader<T>(key.Key);
        }

        public bool IsHasHeaders()
        {
            return !Headers.IsNullOrEmpty();
        }

        public IList<MessageHeader> GetAllHeaders() => Headers.Values.ToImmutableList();

        public IDictionary<string, MessageHeader> GetAllHeaderMap() => Headers.ToImmutableDictionary();

        public bool IsForward() => ExistHeader(MessageHeaderConstants.RPC_FORWARD_HEADER);

        public RpcForwardHeader? ForwardHeader => GetHeader(MessageHeaderConstants.RPC_FORWARD_HEADER);

        public bool ExistHeader(string key)
        {
            return Headers.ContainsKey(key);
        }

        public bool ExistHeader<T>(string key) where T : MessageHeader<T>
        {
            return GetHeader<T>(key) != null;
        }

        public bool ExistHeader(MessageHeaderKey key)
        {
            return ExistHeader(key.Key);
        }

        public bool ExistHeader<T>(MessageHeaderKey<T> key) where T : MessageHeader<T>
        {
            return GetHeader(key) != null;
        }

        public MessageHeader? PutHeader(MessageHeader header)
        {
            return Headers.Put(header.Key, header);
        }

        public MessageHeader? PutHeaderIfAbsent(MessageHeader header)
        {
            return Headers.PutIfAbsent(header.Key, header);
        }

        public T? PutHeader<T>(MessageHeader<T> header) where T : MessageHeader<T>
        {
            return (T?) Headers.Put(header.Key, header);
        }

        public T? PutHeaderIfAbsent<T>(MessageHeader<T> header) where T : MessageHeader<T>
        {
            return (T?) Headers.PutIfAbsent(header.Key, header);
        }

        public bool RemoveHeader<T>(string key)
        {
            return Headers.Remove(key);
        }

        public bool RemoveHeader<T>(MessageHeaderKey<T> key) where T : MessageHeader<T>
        {
            return Headers.Remove(key.Key);
        }

        public void RemoveHeaders(IEnumerable<string> keys)
        {
            Headers.RemoveRange(keys);
        }

        public void RemoveAllHeaders()
        {
            Headers.Clear();
        }
    }

}
