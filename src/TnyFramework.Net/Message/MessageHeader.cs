// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

namespace TnyFramework.Net.Message;

/// <summary>
/// 消息头信息
/// </summary>
public abstract class MessageHeader
{
    public MessageHeaderKey HeaderKey { get; }

    public abstract object Value { get; }

    public string Key => HeaderKey.Value;

    public MessageHeaderUsage Usage => HeaderKey.Usage;

    protected MessageHeader(MessageHeaderKey headerKey)
    {
        HeaderKey = headerKey;
    }
}

/// <summary>
/// 消息头信息
/// </summary>
public abstract class MessageHeader<T> : MessageHeader
{
    public abstract T GetValue();

    protected MessageHeader(MessageHeaderKey headerKey) : base(headerKey)
    {
    }
}

/// <summary>
/// 值是Header的消息头信息
/// </summary>
public abstract class SelfMessageHeader<T> : MessageHeader<T> where T : SelfMessageHeader<T>
{
    public override object Value { get; }

    protected SelfMessageHeader(MessageHeaderKey headerKey) : base(headerKey)
    {
        Value = this;
    }

    public override T GetValue()
    {
        return (T) Value;
    }
}

/// <summary>
/// 值是Value消息头信息
/// </summary>
public abstract class ValueMessageHeader<TValue> : MessageHeader<TValue>
{
    protected ValueMessageHeader(MessageHeaderKey headerKey) : base(headerKey)
    {
    }

    public override object Value => GetValue()!;
}
