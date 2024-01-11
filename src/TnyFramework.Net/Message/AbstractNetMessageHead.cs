// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System.Collections.Generic;

namespace TnyFramework.Net.Message;

public abstract class AbstractNetMessageHead : MessageHeaderContainer, INetMessageHead
{
    protected AbstractNetMessageHead()
    {
    }

    protected AbstractNetMessageHead(IDictionary<string, MessageHeader> headers) : base(headers)
    {
    }

    public abstract MessageMode Mode { get; init; }

    public abstract long ToMessage { get; init; }

    public abstract int ProtocolId { get; init; }

    public abstract int Line { get; init; }

    public abstract long Id { get; init; }

    public abstract int Code { get; init; }

    public abstract long Time { get; init; }

    public abstract bool IsOwn(IProtocol protocol);

    public abstract void AllotMessageId(long id);
}
