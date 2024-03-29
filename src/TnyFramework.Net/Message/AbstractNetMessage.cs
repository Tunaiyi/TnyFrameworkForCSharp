// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TnyFramework.Common.Attribute;

namespace TnyFramework.Net.Message;

public abstract class AbstractNetMessage : INetMessage
{
    private readonly INetMessageHead head;

    private readonly AttributesContext attributes = new();

    protected AbstractNetMessage(INetMessageHead head)
    {
        this.head = head;
        Body = null!;
    }

    protected AbstractNetMessage(INetMessageHead head, object? body)
    {
        this.head = head;
        Body = body;
    }

    public long Id => head.Id;

    public int ProtocolId => head.ProtocolId;

    public int Line => head.Line;

    public int Code => head.Code;

    public long Time => head.Time;

    public long ToMessage => head.ToMessage;

    public MessageMode Mode => head.Mode;

    public IMessageHead Head => head;

    public object? Body { get; }

    public IAttributes Attribute => attributes.Attributes;

    public bool ExistBody => Body != null;

    public T? GetHeader<T>(string key) where T : MessageHeader => head.GetHeader<T>(key);

    public MessageHeader? GetHeader(string key, Type type) => head.GetHeader(key, type);

    public MessageHeader? GetHeader(string key) => head.GetHeader(key);

    public IList<T> GetHeaders<T>() where T : MessageHeader => head.GetHeaders<T>();

    public IList<MessageHeader> GetHeaders(Type type) => head.GetHeaders(type);

    public T? GetHeader<T>(MessageHeaderKey<T> key) where T : MessageHeader => head.GetHeader(key);

    bool IMessageHeaderContainer.IsHasHeaders() => head.IsHasHeaders();

    public IDictionary<string, MessageHeader> GetAllHeaderMap() => head.GetAllHeaderMap();

    public bool IsForward() => head.IsForward();

    public RpcForwardHeader? ForwardHeader => head.ForwardHeader;

    public IList<MessageHeader> GetAllHeaders() => head.GetAllHeaders();

    public bool ExistHeader(string key) => head.ExistHeader(key);

    public bool ExistHeader<T>(string key) where T : MessageHeader => head.ExistHeader<T>(key);

    public bool ExistHeader(MessageHeaderKey key) => head.ExistHeader(key);

    public bool ExistHeader<T>(MessageHeaderKey<T> key) where T : MessageHeader => head.ExistHeader(key);

    public T? PutHeader<T>(T header) where T : MessageHeader => head.PutHeader(header);

    public T? PutHeaderIfAbsent<T>(T header) where T : MessageHeader =>
        head.PutHeaderIfAbsent(header);

    public bool RemoveHeader<T>(string key) => head.RemoveHeader<T>(key);

    public bool RemoveHeader<T>(MessageHeaderKey<T> key) where T : MessageHeader => head.RemoveHeader(key);

    public void RemoveHeaders(IEnumerable<string> keys) => head.RemoveHeaders(keys);

    public void RemoveAllHeaders() => head.RemoveAllHeaders();

    public bool IsOwn(IProtocol protocol) => head.IsOwn(protocol);

    public void AllotMessageId(long id) => head.AllotMessageId(id);

    public T? BodyAs<T>()
    {
        var type = typeof(T);
        var body = Body;
        if (body == null)
            return default;
        if (type.IsInstanceOfType(Body))
        {
            return (T) body;
        }
        throw new InvalidCastException($"{body.GetType()} can not convert to {type}");
    }

    public override string ToString()
    {
        return $"[id={Id},mode={Mode},protocol={ProtocolId},to={ToMessage},date={new DateTime(Time):o},code={Code},body={Format(Body)}]";
    }

    private static string? Format(object? value)
    {
        switch (value)
        {
            case MessageParamList: return value.ToString();
            case IList list:
                var values = new object[list.Count];
                for (var i = 0; i < values.Length; i++)
                {
                    values[i] = list[i]!;
                }
                return $"[{string.Join(", ", values)}]";
            case IDictionary dictionary:
                var kv = (from object key in dictionary.Keys select $"{key}={dictionary[key]}").ToList();
                return "{" + string.Join(", ", kv) + "}";
            default:
                return value?.ToString();
        }

    }
}
