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
using TnyFramework.Common.Exceptions;
using TnyFramework.Common.Extensions;
using TnyFramework.Common.Result;
using TnyFramework.Net.Message;

namespace TnyFramework.Net.Rpc.Remote;

public class RpcRemoteInvokeParams
{
    public object[] Params { get; }

    public IRpcServicer From { get; internal set; }

    public IContact Sender { get; internal set; }

    public IRpcServicer To { get; internal set; }

    public IContact Receiver { get; internal set; }

    public object RouteValue { get; internal set; }

    public bool Forward { get; internal set; }

    public IResultCode Code { get; internal set; } = ResultCode.SUCCESS;

    private readonly IDictionary<string, MessageHeader> headerMap = new Dictionary<string, MessageHeader>();

    public IDictionary<string, MessageHeader> HeaderMap => headerMap.ToImmutableDictionary();

    public RpcRemoteInvokeParams(int size)
    {
        From = null!;
        Sender = null!;
        To = null!;
        Receiver = null!;
        RouteValue = null!;
        Params = new object[size];
    }

    public IEnumerable<MessageHeader> GetAllHeaders()
    {
        if (!Forward)
            return headerMap.Values;
        if (headerMap.ContainsKey(MessageHeaderKeys.RPC_FORWARD_KEY))
            return headerMap.Values;
        if (!To.IsNotNull() && !From.IsNotNull() && !Receiver.IsNotNull() && !Sender.IsNotNull())
            return headerMap.Values;
        var forwardHeader = new RpcForwardHeader()
            .SetFrom(From)
            .SetTo(To)
            .SetSender(Sender)
            .SetReceiver(Receiver);
        headerMap.Put(forwardHeader.Key, forwardHeader);
        return headerMap.Values;
    }

    internal void SetParams(int index, object value)
    {
        if (Params[index].IsNull())
        {
            Params[index] = value;
        } else
        {
            throw new IllegalArgumentException($"参数 {index} 已设置");
        }
    }

    public object? GetBody()
    {
        return Params.Length == 0 ? null : Params[0];
    }

    internal RpcRemoteInvokeParams PutHeader(MessageHeader messageHeader)
    {
        headerMap.TryAdd(messageHeader.Key, messageHeader);
        return this;
    }

    internal RpcRemoteInvokeParams SetTo(IRpcServiceType toService)
    {
        To = new ForwardPoint(toService);
        return this;
    }

    internal RpcRemoteInvokeParams SetCode(object code)
    {
        switch (code)
        {
            case int id:
                Code = ResultCode.ForId(id);
                break;
            case ResultCode resultCode:
                Code = resultCode;
                break;
            default:
                throw new InvalidCastException($"{code.GetType()}类型参数 {code}, 无法解析位 {typeof(IResultCode)}");
        }
        return this;
    }

    internal RpcRemoteInvokeParams SetBody(object body)
    {
        SetParams(0, body);
        return this;
    }
}
