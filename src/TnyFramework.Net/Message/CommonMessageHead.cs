// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.Collections.Generic;

namespace TnyFramework.Net.Message;

public class CommonMessageHead : AbstractNetMessageHead
{
    private long id;

    public override long Id {
        get => id;
        init => id = value;
    }

    public sealed override long ToMessage { get; init; }

    public sealed override int ProtocolId { get; init; }

    public sealed override int Line { get; init; }

    public sealed override int Code { get; init; }

    public sealed override long Time { get; init; }

    public sealed override MessageMode Mode { get; init; }

    public CommonMessageHead()
    {
    }

    public CommonMessageHead(long id, MessageMode mode, int line, int protocolId, int code, long toMessage,
        long time, IDictionary<string, MessageHeader> headers) : base(headers)
    {
        this.id = id;
        Line = line;
        Mode = mode;
        ProtocolId = protocolId;
        Time = time;
        ToMessage = toMessage;
        Code = code;
    }

    public CommonMessageHead(long id, IMessageSubject subject) : base(subject.GetAllHeaderMap())
    {
        this.id = id;
        Line = subject.Line;
        Mode = subject.Mode;
        ProtocolId = subject.ProtocolId;
        Code = subject.Code;
        ToMessage = subject.ToMessage;
        Time = DateTimeOffset.Now.ToUnixTimeMilliseconds();
    }

    public override bool IsOwn(IProtocol protocol)
    {
        return ProtocolId == protocol.ProtocolId;
    }

    public override void AllotMessageId(long idValue) => id = idValue;

    private bool Equals(CommonMessageHead other)
    {
        return id == other.id && ToMessage == other.ToMessage && ProtocolId == other.ProtocolId &&
               Line == other.Line && Code == other.Code &&
               Time == other.Time;
    }

    public override string ToString()
    {
        return $"[{nameof(Id)}: {Id}, {nameof(Mode)}: {Mode}, {nameof(ProtocolId)}: {ProtocolId}]";
    }

    public override bool Equals(object? obj)
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
