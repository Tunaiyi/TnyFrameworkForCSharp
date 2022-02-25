using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TnyFramework.Common.Exception;
using TnyFramework.Common.Result;
using TnyFramework.Net.Base;
using TnyFramework.Net.Common;
using TnyFramework.Net.Endpoint;
using TnyFramework.Net.Message;
using TnyFramework.Net.Rpc.Attributes;
using TnyFramework.Net.Transport;
namespace TnyFramework.Net.Dispatcher
{
    public class ParamDescription
    {
        public AttributeHolder AttributeHolder { get; }

        /// <summary>
        /// body 为List时 索引
        /// </summary>
        public int Index { get; }

        /// <summary>
        /// body 为Map时 key
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// 参数类型
        /// </summary>
        public ParamMode Mode { get; } = ParamMode.None;

        /// <summary>
        /// 参数所属类型
        /// </summary>
        public Type Type { get; }

        public bool Require { get; }

        public IList<Attribute> Attributes => AttributeHolder.Attributes;

        public MethodControllerHolder Method { get; }


        public ParamDescription(MethodControllerHolder method, ParameterInfo info, ref int index)
        {
            Method = method;
            Type = info.ParameterType;
            AttributeHolder = new AttributeHolder(info.GetCustomAttributes());
            Require = true;
            if (typeof(IEndpoint).IsAssignableFrom(Type))
            {
                Mode = ParamMode.Endpoint;
            } else if (typeof(ISession).IsAssignableFrom(Type))
            {
                Mode = ParamMode.Session;
            } else if (typeof(IServerBootstrapSetting).IsAssignableFrom(Type))
            {
                Mode = ParamMode.Setting;
            } else if (typeof(ITunnel).IsAssignableFrom(Type))
            {
                Mode = ParamMode.Tunnel;
            } else if (typeof(IMessage).IsAssignableFrom(Type))
            {
                Mode = ParamMode.Message;
            } else if (typeof(IResultCode).IsAssignableFrom(Type))
            {
                Mode = ParamMode.Code;
            } else
            {
                if (AttributeHolder.Empty)
                {
                    Index = index;
                    index++;
                    Mode = ParamMode.IndexParam;
                } else
                {
                    foreach (var type in AttributeHolder.AttributeTypes)
                    {
                        if (type == typeof(MsgBodyAttribute))
                        {
                            var body = AttributeHolder.GetAttribute<MsgBodyAttribute>();
                            Mode = ParamMode.Body;
                            Require = body.Require;
                        } else if (type == typeof(MsgParamAttribute))
                        {
                            var msgParam = AttributeHolder.GetAttribute<MsgParamAttribute>();
                            Require = msgParam.Require;
                            if (!string.IsNullOrEmpty(msgParam.Name))
                            {
                                Key = msgParam.Name;
                                Mode = ParamMode.KeyParam;
                            } else
                            {
                                if (msgParam.Index >= 0)
                                {
                                    Index = msgParam.Index;
                                } else
                                {
                                    Index = index;
                                    index++;
                                }
                                Mode = ParamMode.IndexParam;
                            }
                        } else if (type == typeof(UserIdAttribute))
                        {
                            Mode = ParamMode.UserId;
                        } else if (type == typeof(MsgCodeAttribute))
                        {
                            if (Type == typeof(int))
                            {
                                Mode = ParamMode.CodeNum;
                            } else if (typeof(IResultCode).IsAssignableFrom(Type))
                            {
                                Mode = ParamMode.Code;
                            } else
                            {
                                throw new IllegalArgumentException(
                                    $"{typeof(MsgCodeAttribute)} 类型参数只能是 {typeof(IResultCode)} {typeof(int)} 无法为 {Type}");
                            }
                        }
                    }
                }
            }
        }


        public object GetValue(INetTunnel tunnel, IMessage message, object body)
        {
            if (body == null)
            {
                body = message.Body;
            }
            var head = message.Head;
            object value = null;
            switch (Mode)
            {
                case ParamMode.Message:
                    value = message;
                    break;
                case ParamMode.Endpoint: {
                    value = tunnel.GetEndpoint();
                    break;
                }
                case ParamMode.Session: {
                    value = tunnel.GetEndpoint();
                    if (Type.IsInstanceOfType(value))
                    {
                        break;
                    }
                    throw new NullReferenceException($"{tunnel} session is null");
                }
                case ParamMode.Setting: {
                    var context = tunnel.Context;
                    value = context.Setting;
                    break;
                }
                case ParamMode.Tunnel:
                    value = tunnel;
                    break;
                case ParamMode.Body:
                    value = body;
                    break;
                case ParamMode.UserId:
                    value = tunnel.GetUserId();
                    break;
                case ParamMode.IndexParam:
                    try
                    {
                        if (body == null)
                        {
                            if (Require)
                            {
                                throw new ResultCodeException(NetResultCode.SERVER_EXECUTE_EXCEPTION, $"{message} 收到消息体为 null");
                            }
                            break;
                        }
                        if (body is IList list)
                        {
                            value = list[Index];
                        } else
                        {
                            throw new ResultCodeException(NetResultCode.SERVER_EXECUTE_EXCEPTION, $"{Method} 收到消息体为 {body.GetType()}, 不可通过index获取");
                        }
                    } catch (SystemException e)
                    {
                        throw new ResultCodeException(NetResultCode.SERVER_EXECUTE_EXCEPTION, e, ($"{Method} 调用异常"));
                    }
                    break;
                case ParamMode.KeyParam:
                    if (body == null)
                    {
                        if (Require)
                        {
                            throw new ResultCodeException(NetResultCode.SERVER_EXECUTE_EXCEPTION, $"{message} 收到消息体为 null");
                        }
                        break;
                    }
                    if (body is IDictionary dic)
                    {
                        value = dic[Key];
                    } else
                    {
                        throw new ResultCodeException(NetResultCode.SERVER_EXECUTE_EXCEPTION, $"{message} 非 {typeof(IDictionary)}");
                    }
                    break;
                case ParamMode.Code:
                    value = ResultCode.ForId(head.Code);
                    break;
                case ParamMode.CodeNum:
                    value = head.Code;
                    break;
            }
            // TODO 是否需要信息转换类型
            // if (value != null && !this.paramClass.isInstance(value))
            // {
            //     value = ObjectAide.convertTo(value, this.paramClass);
            // }
            return value;
        }
    }
}
