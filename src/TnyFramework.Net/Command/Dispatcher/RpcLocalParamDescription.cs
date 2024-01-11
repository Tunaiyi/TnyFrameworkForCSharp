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
using System.Reflection;
using TnyFramework.Common.Exceptions;
using TnyFramework.Common.Extensions;
using TnyFramework.Common.Result;
using TnyFramework.Net.Attributes;
using TnyFramework.Net.Common;
using TnyFramework.Net.Exceptions;
using TnyFramework.Net.Message;
using TnyFramework.Net.Rpc;
using TnyFramework.Net.Session;
using TnyFramework.Net.Transport;

namespace TnyFramework.Net.Command.Dispatcher;

public class RpcLocalParamDescription
{
    /// <summary>
    /// body 为List时 索引
    /// </summary>
    public int Index { get; }

    /// <summary>
    /// 参数模式
    /// </summary>
    public ParamMode Mode { get; } = ParamMode.None;

    /// <summary>
    /// Header Key
    /// </summary>
    public string HeaderKey { get; } = "";

    /// <summary>
    /// 是否是必须
    /// </summary>
    public bool Require { get; }

    /// <summary>
    /// 特性持有器
    /// </summary>
    public AttributeHolder AttributeHolder { get; }

    /// <summary>
    /// 参数所属类型
    /// </summary>
    public Type ParamType { get; }

    public IList<Attribute> Attributes => AttributeHolder.Attributes;

    public MethodControllerHolder Method { get; }

    public RpcLocalParamDescription(MethodControllerHolder method, ParameterInfo info, ParamIndexCreator indexCreator)
    {
        Method = method;
        ParamType = info.ParameterType;
        AttributeHolder = new AttributeHolder(info.GetCustomAttributes());
        var optional = AttributeHolder.GetAttribute<RpcOptionalAttribute>();
        Require = optional == null;
        if (typeof(ISession).IsAssignableFrom(ParamType))
        {
            Mode = ParamMode.Session;
        } else if (typeof(ITunnel).IsAssignableFrom(ParamType))
        {
            Mode = ParamMode.Tunnel;
        } else if (typeof(IMessage).IsAssignableFrom(ParamType))
        {
            Mode = ParamMode.Message;
        } else if (typeof(IResultCode).IsAssignableFrom(ParamType))
        {
            Mode = ParamMode.Code;
        } else if (typeof(MessageHeader).IsAssignableFrom(ParamType))
        {
            Mode = ParamMode.Header;
            var header = AttributeHolder.GetAttribute<RpcHeaderAttribute>();
            if (header != null)
            {
                HeaderKey = header.Key;
            }
        } else
        {
            foreach (var type in AttributeHolder.AttributeTypes)
            {
                if (type == typeof(RpcBodyAttribute))
                {
                    Mode = ParamMode.Body;
                } else if (type == typeof(RpcParamAttribute))
                {
                    var msgParam = AttributeHolder.GetAttribute<RpcParamAttribute>();
                    if (msgParam?.Index >= 0)
                    {
                        Index = indexCreator.Use(msgParam.Index);
                    } else
                    {
                        Index = indexCreator.Peek();
                    }
                    Mode = ParamMode.Param;
                } else if (type == typeof(IdentifyAttribute))
                {
                    Mode = ParamMode.Identify;
                } else if (type == typeof(IdentifyTokenAttribute))
                {
                    Mode = ParamMode.IdentifyToken;
                } else if (type == typeof(RpcCodeAttribute))
                {
                    if (ParamType == typeof(int))
                    {
                        Mode = ParamMode.CodeNum;
                    } else if (typeof(IResultCode).IsAssignableFrom(ParamType))
                    {
                        Mode = ParamMode.Code;
                    } else
                    {
                        throw new IllegalArgumentException(
                            $"{typeof(RpcCodeAttribute)} 类型参数只能是 {typeof(IResultCode)} {typeof(int)} 无法为 {ParamType}");
                    }
                } else if (type == typeof(RpcFromAttribute))
                {
                    if (typeof(IContact).IsAssignableFrom(ParamType))
                    {
                        Mode = ParamMode.Sender;
                    }
                    if (typeof(IRpcServicer).IsAssignableFrom(ParamType))
                    {
                        Mode = ParamMode.FromService;
                    }
                } else if (type == typeof(RpcToAttribute))
                {
                    if (typeof(IContact).IsAssignableFrom(ParamType))
                    {
                        Mode = ParamMode.Receiver;
                    }
                    if (typeof(IRpcServicer).IsAssignableFrom(ParamType))
                    {
                        Mode = ParamMode.ToService;
                    }
                }
                if (Mode != ParamMode.None)
                {
                    break;
                }
            }
        }
        if (Mode != ParamMode.None || !Require)
            return;
        if (method.MessageMode == MessageMode.Request)
        {
            Index = indexCreator.Peek();
            Mode = ParamMode.Param;
        } else
        {
            Mode = ParamMode.Body;
        }
    }

    public object? GetValue(INetTunnel tunnel, IMessage message, object? body)
    {
        if (body == null)
        {
            body = message.Body;
        }
        var head = message.Head;
        object? value = null;
        var context = tunnel.Context;
        switch (Mode)
        {
            case ParamMode.None:
                break;
            case ParamMode.Message:
                value = message;
                break;
            case ParamMode.Session: {
                value = tunnel.Session;
                if (ParamType.IsInstanceOfType(value))
                {
                    break;
                }
                throw new NullReferenceException($"{tunnel} session is null");
            }
            case ParamMode.Tunnel:
                value = tunnel;
                break;
            case ParamMode.Body:
                value = body;
                break;
            case ParamMode.Identify:
                value = tunnel.Identify;
                break;
            case ParamMode.IdentifyToken:
                value = tunnel.IdentifyToken;
                break;
            case ParamMode.Header:
                if (HeaderKey.IsNotBlank())
                {
                    value = message.GetHeader(HeaderKey, ParamType);
                } else
                {
                    var headers = message.GetHeaders(ParamType);
                    if (headers.Count == 1)
                    {
                        value = headers[0];
                    }
                    if (headers.Count > 1)
                    {
                        throw new CommandException(NetResultCode.SERVER_EXECUTE_EXCEPTION,
                            $"{ParamType} 类型的MessageHeader多于一个 {headers}, 必须通过 MsgHeader 指定 key");
                    }
                }
                break;
            case ParamMode.Param:
                try
                {
                    if (body != null)
                    {
                        if (body is IList paramBody)
                        {
                            value = paramBody[Index];
                        } else
                        {
                            throw new CommandException(NetResultCode.SERVER_EXECUTE_EXCEPTION, $"{Method} 收到消息体为 {body.GetType()}, 不可通过index获取");
                        }
                    }
                } catch (CommandException)
                {
                    throw;
                } catch (Exception e)
                {
                    throw new CommandException(NetResultCode.SERVER_EXECUTE_EXCEPTION, e, $"{Method} 调用异常");
                }
                break;
            case ParamMode.Code:
                value = ResultCode.ForId(head.Code);
                break;
            case ParamMode.CodeNum:
                value = head.Code;
                break;
            case ParamMode.FromService: {
                var forwardHeader = head.GetHeader(MessageHeaderKeys.RPC_FORWARD_HEADER);
                if (forwardHeader != null)
                {
                    value = forwardHeader.From;
                }
                break;
            }
            case ParamMode.ToService: {
                var forwardHeader = head.GetHeader(MessageHeaderKeys.RPC_FORWARD_HEADER);
                if (forwardHeader != null)
                {
                    value = forwardHeader.To;
                }
                break;
            }
            case ParamMode.Sender: {
                var forwardHeader = head.GetHeader(MessageHeaderKeys.RPC_FORWARD_HEADER);
                if (forwardHeader != null)
                {
                    value = context.ContactFactory.CreateContact(forwardHeader.Sender!);
                }
                break;
            }
            case ParamMode.Receiver: {
                var forwardHeader = head.GetHeader(MessageHeaderKeys.RPC_FORWARD_HEADER);
                if (forwardHeader != null)
                {
                    value = context.ContactFactory.CreateContact(forwardHeader.Receiver!);
                }
                break;
            }
        }
        if (Require && value == null)
        {
            throw new CommandException(NetResultCode.SERVER_EXECUTE_EXCEPTION, $"{Method} 第 {Index} 个参数不可为 null");
        }
        // TODO 是否需要信息转换类型
        // if (value != null && !this.paramClass.isInstance(value))
        // {
        //     value = ObjectAide.convertTo(value, this.paramClass);
        // }
        return value;
    }
}
