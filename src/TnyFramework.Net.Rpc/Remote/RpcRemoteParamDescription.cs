using System;
using System.Collections.Generic;
using TnyFramework.Common.Result;
using TnyFramework.Net.Attributes;
using TnyFramework.Net.Dispatcher;
using TnyFramework.Net.Message;
using TnyFramework.Net.Rpc.Attributes;

namespace TnyFramework.Net.Rpc.Remote
{

    public class RpcRemoteParamDescription
    {
        /// <summary>
        /// body 为List时 索引
        /// </summary>
        public int Index { get; } = -1;

        /// <summary>
        /// 是否是必须
        /// </summary>
        public bool Require { get; } = true;

        /// <summary>
        /// 参数模式
        /// </summary>
        public ParamMode Mode { get; } = ParamMode.None;

        /// <summary>
        /// 参数模式
        /// </summary>
        public Type ParamType { get; }

        /// <summary>
        /// 特性持有器
        /// </summary>
        public AttributeHolder AttributeHolder { get; }

        /// <summary>
        /// 是否是可路由
        /// </summary>
        public bool Route { get; } = false;

        public RpcRemoteParamDescription(RpcRemoteMethod method, Type paramType, IEnumerable<Attribute> paramAnnotations,
            ParamIndexCreator indexCreator)
        {
            ParamType = paramType;
            AttributeHolder = new AttributeHolder(paramAnnotations);
            var optional = AttributeHolder.GetAttribute<RpcOptionalAttribute>();
            if (optional != null)
            {
                Require = false;
            }
            if (typeof(MessageHeader).IsAssignableFrom(paramType))
            {
                Mode = ParamMode.Header;
            } else if (typeof(ResultCode).IsAssignableFrom(paramType))
            {
                Mode = ParamMode.Code;
            } else
            {
                foreach (var attribute in AttributeHolder.Attributes)
                {
                    if (attribute is RpcBodyAttribute)
                    {
                        Mode = ParamMode.Body;
                    } else if (attribute is RpcParamAttribute param)
                    {
                        Index = param.Index < 0 ? indexCreator.Peek() : indexCreator.Use(param.Index);
                        Mode = ParamMode.Param;
                    } else if (attribute is RpcCodeAttribute)
                    {
                        Mode = ParamMode.CodeNum;
                    } else if (attribute is RpcIgnoreAttribute)
                    {
                        Mode = ParamMode.Ignore;
                    } else if (attribute is RpcRouteParamAttribute)
                    {
                        Route = true;
                    } else if (attribute is RpcFromAttribute)
                    {
                        if (typeof(IMessager).IsAssignableFrom(paramType))
                        {
                            Mode = ParamMode.Sender;
                        }
                        if (typeof(IRpcServicer).IsAssignableFrom(paramType))
                        {
                            Mode = ParamMode.FromService;
                        }
                    } else if (attribute is RpcToAttribute)
                    {
                        if (typeof(IMessager).IsAssignableFrom(paramType))
                        {
                            Mode = ParamMode.Receiver;
                        }
                        if (typeof(IRpcServicer).IsAssignableFrom(paramType))
                        {
                            Mode = ParamMode.ToService;
                        }
                    }
                    if (Mode == ParamMode.None)
                    {
                        break;
                    }
                }
            }

            if (Mode != ParamMode.None || !Require)
                return;
            if (method.Mode == MessageMode.Request)
            {
                Index = indexCreator.Peek();
                Mode = ParamMode.Param;
            } else
            {
                Mode = ParamMode.Body;
            }
        }
    }

}
