// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TnyFramework.Common.Extensions;
using TnyFramework.Common.Result;
using TnyFramework.Net.Transport;

namespace TnyFramework.Net.Message;

public class DefaultMessageContent : RequestContent
{
    public override IResultCode ResultCode { get; }

    public override int ProtocolId { get; }

    public override int Line { get; }

    public override long ToMessage { get; }

    public override MessageMode Mode { get; }

    public override object? Body { get; protected set; }

    public override MessageContent WithHeader(MessageHeader? header)
    {
        if (header == null)
            return this;
        PutHeader(header);
        return this;
    }

    public override MessageContent WithHeader<TH>(Action<TH>? action)
    {
        var header = new TH();
        action?.Invoke(header);
        PutHeader(header);
        return this;
    }

    public override MessageContent WithHeaders(IEnumerable<MessageHeader> values)
    {
        foreach (var header in values)
        {
            if (header.IsNull())
                continue;
            PutHeader(header);
        }
        return this;
    }

    public DefaultMessageContent(MessageMode mode, IProtocol protocol, IResultCode resultCode,
        long toMessage = MessageConstants.EMPTY_MESSAGE_ID)
    {
        ResultCode = resultCode;
        Body = null;
        responseSource = null!;
        ToMessage = toMessage;
        Mode = mode;
        ProtocolId = protocol.ProtocolId;
        Line = protocol.Line;
    }

    public override bool ExistBody => Body != null;

    public override T BodyAs<T>()
    {
        if (Body is T body)
            return body;
        return default!;
    }

    public override MessageContent WithBody(object? messageBody)
    {
        if (Body == null)
        {
            Body = messageBody;
        }
        return this;
    }

    public override void Cancel(bool mayInterruptIfRunning)
    {
        responseSource?.TrySetCanceled();
    }

    public override void Cancel(Exception cause)
    {
        responseSource?.SetException(cause);
    }

    public override RequestContent WillRespondAwaiter(long timeout)
    {
        responseSource = new TaskResponseSource(timeout);
        return this;
    }

    public override bool Respond(out Task<IMessage> task)
    {
        task = responseSource?.Task!;
        return task != null;
    }

    public override bool IsWaitRespond => responseSource != null;
}
