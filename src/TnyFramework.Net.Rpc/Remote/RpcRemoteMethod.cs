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
using System.Linq;
using System.Reflection;
using Microsoft.IdentityModel.Tokens;
using TnyFramework.Common.Exceptions;
using TnyFramework.Common.Extensions;
using TnyFramework.Net.Application;
using TnyFramework.Net.Attributes;
using TnyFramework.Net.Command.Dispatcher;
using TnyFramework.Net.Message;
using TnyFramework.Net.Rpc.Attributes;

namespace TnyFramework.Net.Rpc.Remote
{

    public class RpcRemoteMethod
    {
        /// <summary>
        /// 服务类型
        /// </summary>
        public IContactType ServiceType { get; }

        /// <summary>
        /// 目标
        /// </summary>
        public RpcServiceType? TargetServiceType { get; }

        /// <summary>
        /// 转发
        /// </summary>
        public RpcServiceType? ForwardServiceType { get; }

        /// <summary>
        /// 是否通过代理调用
        /// </summary>
        public bool Forward { get; }

        /// <summary>
        /// 请求模式
        /// </summary>
        public MessageMode Mode { get; }

        /// <summary>
        /// 方法名
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 方法
        /// </summary>
        public MethodInfo Method { get; }

        /// <summary>
        /// 协议 id
        /// </summary>
        public int Protocol { get; }

        /// <summary>
        /// 线路
        /// </summary>
        public int Line { get; }

        /// <summary>
        /// RPC 返回模式
        /// </summary>
        public RpcReturnMode ReturnMode { get; }

        /// <summary>
        /// 返回值类
        /// </summary>
        public Type ReturnType { get; }

        /// <summary>
        /// 返回 body 类型
        /// </summary>
        public RpcBodyMode BodyMode { get; }

        /// <summary>
        /// 返回 body 类型
        /// </summary>
        public Type BodyType { get; }

        /// <summary>
        /// 路由类型
        /// </summary>
        public Type RouterType { get; } = typeof(IRpcRouter);

        /// <summary>
        /// 异步
        /// </summary>
        public RpcInvokeMode Invocation { get; }

        /// <summary>
        /// 安静模式
        /// </summary>
        public bool Silently { get; }

        /// <summary>
        /// 超时
        /// </summary>
        public int Timeout { get; }

        /// <summary>
        /// 消息参数大小
        /// </summary>
        public int MessageParamSize { get; }

        /// <summary>
        /// 参数描述列表
        /// </summary>
        public IList<RpcRemoteParamDescription> Parameters { get; }

        public static IEnumerable<RpcRemoteMethod> MethodsOf(Type rpcType)
        {
            var rpcService = rpcType.GetCustomAttribute<RpcRemoteServiceAttribute>();
            if (rpcService == null)
            {
                throw new NullReferenceException($"{rpcType} 没有标识 {typeof(RpcRemoteServiceAttribute)} 注解");
            }
            return rpcType.GetMethods()
                .Select(m => MethodOf(m, rpcType, rpcService))
                .Where(rpcMethod => rpcMethod.IsNotNull())
                .ToImmutableList();
        }

        private static RpcRemoteMethod MethodOf(MethodInfo method, Type rpcClass, RpcRemoteServiceAttribute rpcService)
        {
            var profile = RpcProfile.OneOf(method);
            if (profile == null)
            {
                return null!;
            }
            var optionsAttribute = rpcClass.GetCustomAttribute<RpcRemoteOptionsAttribute>() ?? new RpcRemoteOptionsAttribute();
            return new RpcRemoteMethod(method, profile, rpcService, optionsAttribute);
        }

        private RpcRemoteMethod(MethodInfo method, RpcProfile profile, RpcRemoteServiceAttribute rpcService, RpcRemoteOptionsAttribute defaultOptions)
        {
            Method = method;
            Protocol = profile.Protocol;
            Line = profile.Line;
            var options = method.GetCustomAttribute<RpcRemoteOptionsAttribute>() ?? defaultOptions;
            ReturnType = options.Router;
            Forward = !CollectionUtilities.IsNullOrEmpty(rpcService.ForwardService);
            Mode = profile.Mode;
            ServiceType = ContactType.ForGroup(rpcService.Service);
            if (Forward)
            {
                TargetServiceType = RpcServiceType.ForService(rpcService.Service);
                ForwardServiceType = RpcServiceType.ForService(rpcService.ForwardService);
            } else
            {
                TargetServiceType = null!;
                ForwardServiceType = null!;
            }
            if (method.DeclaringType != null)
            {
                Name = method.DeclaringType.Name + "." + method.Name;
            } else
            {
                Name = method.Name;
            }
            Invocation = options.Mode;
            Silently = options.Silently;
            Timeout = options.Timeout;
            ReturnType = method.ReturnType;
            ReturnMode = RpcReturnMode.TypeOf(ReturnType);
            if (!ReturnMode.IsCanInvokeBy(Mode))
            {
                throw new IllegalArgumentException($"{method} 返回类型 {ReturnType} 是使用 {Mode} Rpc 模式");
            }
            BodyType = ReturnMode.FindBodyType(method);
            BodyMode = RpcBodyMode.TypeOf(method, BodyType);

            var paramDescriptions = new List<RpcRemoteParamDescription>();

            var parameterInfos = method.GetParameters();

            var indexCreator = new ParamIndexCreator(method);
            var bodySize = 0;
            var paramSize = 0;
            var maxIndex = -1;
            foreach (var paramInfo in parameterInfos)
            {
                var paramType = paramInfo.ParameterType;
                var attributes = paramInfo.GetCustomAttributes();
                var paramDesc = new RpcRemoteParamDescription(this, paramType, attributes, indexCreator);
                paramDescriptions.Add(paramDesc);
                switch (paramDesc.Mode)
                {
                    case ParamMode.Param: {
                        paramSize++;
                        if (paramDesc.Index > maxIndex)
                        {
                            maxIndex = paramDesc.Index;
                        }
                        break;
                    }
                    case ParamMode.Body:
                        bodySize++;
                        break;
                }
            }
            if (bodySize > 0 && paramSize > 0)
            {
                throw new IllegalArgumentException($"{method} 方法不可同时使用 {typeof(RpcParamAttribute)} 与 {typeof(RpcBodyAttribute)}参数");
            }
            if (bodySize > 1)
            {
                throw new IllegalArgumentException($"{method} 方法 {typeof(RpcBodyAttribute)} 参数只能存在一个");
            }
            if (paramSize > 0)
            {
                if (maxIndex >= paramSize)
                {
                    throw new IllegalArgumentException($"{method} 方法 参数最大index {maxIndex} 需要 < 参数个数 {paramSize}");
                }
                MessageParamSize = paramSize;
            }
            if (bodySize == 1)
            {
                MessageParamSize = 1;
            }
            Parameters = paramDescriptions.ToImmutableList();
        }

        public bool IsAsync()
        {
            return ReturnMode.CheckInvocation(Invocation) == RpcInvokeMode.Async;
        }

        public int GetTimeout(int defaultTimeout)
        {
            return Timeout > 0 ? Timeout : defaultTimeout;
        }

        public RpcRemoteInvokeParams GetParams(IList<object> paramValues)
        {
            var invokeParams = new RpcRemoteInvokeParams(MessageParamSize) {
                Forward = Forward
            };
            for (var index = 0; index < paramValues.Count; index++)
            {
                var desc = Parameters[index];
                var value = paramValues[index];
                if (desc.Require && value == null)
                {
                    throw new NullReferenceException($"{Method} 第 {desc.Index} 参数为 null");
                }
                if (Forward)
                {
                    invokeParams.SetTo(TargetServiceType!);
                }
                if (desc.AttributeHolder.GetAttribute<RpcRouteParamAttribute>() != null)
                {
                    invokeParams.RouteValue = value;
                }
                switch (desc.Mode)
                {
                    case ParamMode.Param:
                        invokeParams.SetParams(desc.Index, value);
                        break;
                    case ParamMode.Code:
                    case ParamMode.CodeNum:
                        invokeParams.SetCode(value);
                        break;
                    case ParamMode.Header:
                        if (value is MessageHeader header)
                            invokeParams.PutHeader(header);
                        break;
                    case ParamMode.FromService:
                        if (value is IRpcServicer from)
                            invokeParams.From = from;
                        break;
                    case ParamMode.ToService:
                        if (value is IRpcServicer to)
                            invokeParams.To = to;
                        break;
                    case ParamMode.Sender:
                        if (value is IContact sender)
                            invokeParams.Sender = sender;
                        break;
                    case ParamMode.Receiver:
                        if (value is IContact receiver)
                            invokeParams.Receiver = receiver;
                        break;
                    case ParamMode.Body:
                        invokeParams.SetBody(value);
                        break;
                    case ParamMode.Ignore:
                        break;
                    default:
                        if (desc.Require)
                            throw new IllegalArgumentException($"不支持 {desc.Mode} 参数");
                        break;
                }
            }
            return invokeParams;
        }

        public override string ToString()
        {
            return $"{nameof(ServiceType)}: {ServiceType}, {nameof(Name)}: {Name}";
        }
    }

}
