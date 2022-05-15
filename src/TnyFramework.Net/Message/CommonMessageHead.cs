using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.IdentityModel.Tokens;

namespace TnyFramework.Net.Message
{

    public class CommonMessageHead : AbstractNetMessageHead
    {
        private long id;

        public override long Id => id;

        public override long ToMessage { get; }

        public override int ProtocolId { get; }

        public override int Line { get; }

        public override int Code { get; }

        public override long Time { get; }

        private IDictionary<string, MessageHeader> Headers { get; }

        public CommonMessageHead(long id, MessageMode mode, int line, int protocolId, int code, long toMessage, long time,
            IDictionary<string, MessageHeader> headers) : base(mode)
        {
            this.id = id;
            ToMessage = toMessage;
            ProtocolId = protocolId;
            Line = line;
            Code = code;
            Time = time;
            Headers = headers == null ? ImmutableDictionary<string, MessageHeader>.Empty : headers.ToImmutableDictionary();
        }

        public CommonMessageHead(long id, IMessageContent subject) : base(subject.Mode)
        {
            this.id = id;
            ProtocolId = subject.ProtocolId;
            Code = subject.GetCode();
            ToMessage = subject.ToMessage;
            Time = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            var headers = subject.Headers;
            Headers = headers == null ? ImmutableDictionary<string, MessageHeader>.Empty : headers.ToImmutableDictionary();
        }

        public override bool IsOwn(IProtocol protocol)
        {
            return ProtocolId == protocol.ProtocolId;
        }

        public override T GetHeader<T>(string key)
        {
            var value = GetHeader(key);
            if (value is T header)
                return header;
            return null;
        }

        public override MessageHeader GetHeader(string key, Type type)
        {
            var value = GetHeader(key);
            return type.IsInstanceOfType(value) ? value : null;
        }

        public override MessageHeader GetHeader(string key)
        {
            return !Headers.TryGetValue(key, out var header) ? null : header;
        }

        public override IList<T> GetHeaders<T>()
        {
            return Headers.Values.OfType<T>().ToList();
        }

        public override IList<MessageHeader> GetHeaders(Type type)
        {
            return Headers.Values.Where(type.IsInstanceOfType).ToList();
        }

        public override T GetHeader<T>(MessageHeaderKey<T> key)
        {
            return GetHeader<T>(key.Key);
        }

        public override bool IsHasHeaders => !Headers.IsNullOrEmpty();

        public override IList<MessageHeader> GetAllHeaders() => Headers.Values.ToImmutableList();

        public override IDictionary<string, MessageHeader> GetAllHeadersMap() => Headers.ToImmutableDictionary();

        public override bool ExistHeader(string key)
        {
            return Headers.ContainsKey(key);
        }

        public override bool ExistHeader<T>(string key)
        {
            return GetHeader<T>(key) != null;
        }

        public override bool ExistHeader(MessageHeaderKey key)
        {
            return ExistHeader(key.Key);
        }

        public override void AllotMessageId(long idValue) => id = idValue;

        private bool Equals(CommonMessageHead other)
        {
            return id == other.id && ToMessage == other.ToMessage && ProtocolId == other.ProtocolId && Line == other.Line && Code == other.Code &&
                   Time == other.Time;
        }

        public override string ToString()
        {
            return $"[{nameof(Id)}: {Id}, {nameof(Mode)}: {Mode}, {nameof(ProtocolId)}: {ProtocolId}]";
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((CommonMessageHead) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Id.GetHashCode();
                hashCode = (hashCode * 397) ^ ToMessage.GetHashCode();
                hashCode = (hashCode * 397) ^ ProtocolId;
                hashCode = (hashCode * 397) ^ Line;
                hashCode = (hashCode * 397) ^ Code;
                hashCode = (hashCode * 397) ^ Time.GetHashCode();
                return hashCode;
            }
        }
    }

}
