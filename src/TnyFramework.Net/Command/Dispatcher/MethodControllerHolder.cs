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
using System.Reflection;
using System.Text;
using TnyFramework.Common.Exceptions;
using TnyFramework.Common.Extensions;
using TnyFramework.Common.FastInvoke;
using TnyFramework.Common.FastInvoke.ActionInvoke;
using TnyFramework.Common.FastInvoke.FuncInvoke;
using TnyFramework.Net.Attributes;
using TnyFramework.Net.Base;
using TnyFramework.Net.Common;
using TnyFramework.Net.Message;
using TnyFramework.Net.Plugin;
using TnyFramework.Net.Transport;

namespace TnyFramework.Net.Command.Dispatcher
{

    public class MethodControllerHolder : ControllerHolder
    {
        /// <summary>
        /// 调用器
        /// </summary>
        private readonly IFastInvoker invoker;

        /// <summary>
        /// 执行者
        /// </summary>
        private readonly object executor;

        /// <summary>
        /// 执行前
        /// </summary>
        private readonly PluginChain? beforeChain;

        /// <summary>
        /// 执行后
        /// </summary>
        private readonly PluginChain? afterChain;

        /// <summary>
        /// 类 Controller 信息
        /// </summary>
        private readonly TypeControllerHolder typeController;

        /// <summary>
        /// 参数注解
        /// </summary>
        private readonly IDictionary<Type, IList<Attribute?>> indexParamAnnotationsMap;

        private readonly RpcProfile rpcProfile;

        public void Run()
        {
        }

        public MethodControllerHolder(object executor, MethodInfo method, RpcProfile rpcProfile,
            MessageDispatcherContext context,
            TypeControllerHolder typeController)
            : base(executor)
        {
            this.rpcProfile = rpcProfile;
            invoker = method.ReturnType == typeof(void)
                ? FastActionFactory.CreateFactory(method).CreateInvoker(method)
                : FastFuncFactory.CreateFactory(method).CreateInvoker(method);
            Init(context,
                method.GetCustomAttributes<BeforePluginAttribute>(),
                method.GetCustomAttributes<AfterPluginAttribute>(),
                method.GetCustomAttribute<AuthenticationRequiredAttribute>(),
                method.GetCustomAttribute<AppProfileAttribute>(),
                method.GetCustomAttribute<ScopeProfileAttribute>());
            // var findMethod = ControllerType.GetMethod("FindParamAttributes");
            // if (findMethod == null)
            //     throw new NullReferenceException();
            var nameBuilder = new StringBuilder();
            var type = executor.GetType();
            nameBuilder.Append(type.Name).Append(".").Append(method.Name);
            SimpleName = nameBuilder.ToString();
            var parameterInfos = method.GetParameters();
            var paramAnnotationsMap = new Dictionary<Type, IDictionary<int, Attribute>>();
            ParamDescriptions = InitParamDescriptions(paramAnnotationsMap, parameterInfos, nameBuilder, method);
            nameBuilder.Append(")");
            Name = nameBuilder.ToString();
            this.executor = executor;
            this.typeController = typeController;
            foreach (var plugin in BeforePlugins)
            {
                beforeChain = AppendPlugin(beforeChain, plugin);
            }
            foreach (var plugin in AfterPlugins)
            {
                afterChain = AppendPlugin(afterChain, plugin);
            }
            ReturnType = method.ReturnType;
            indexParamAnnotationsMap = InitIndexParamAttributes(paramAnnotationsMap, ParamDescriptions.Count);
            MethodAttributeHolder = new AttributeHolder(method);

        }

        private IList<ControllerParamDescription> InitParamDescriptions(
            IDictionary<Type, IDictionary<int, Attribute>> paramAnnotationsMap,
            IEnumerable<ParameterInfo> parameterInfos, StringBuilder nameBuilder, MethodInfo method)
        {
            var indexCreator = new ParamIndexCreator(method);
            var paramDescriptions = new List<ControllerParamDescription>();
            var index = 0;
            foreach (var parameterInfo in parameterInfos)
            {
                nameBuilder.Append(index > 0 ? ", " : "(");
                nameBuilder.Append(parameterInfo.ParameterType);
                var paramDescription = new ControllerParamDescription(this, parameterInfo, indexCreator);
                paramDescriptions.Add(paramDescription);
                foreach (var attribute in paramDescription.Attributes)
                {
                    var attributeType = attribute.GetType();
                    if (!paramAnnotationsMap.TryGetValue(attributeType, out var attributes))
                    {
                        attributes = new Dictionary<int, Attribute>();
                        paramAnnotationsMap[attributeType] = attributes;
                    }
                    attributes.Add(index, attribute);
                }
                index++;
            }
            return paramDescriptions.ToImmutableList();
        }

        private static IDictionary<Type, IList<Attribute?>> InitIndexParamAttributes(
            Dictionary<Type, IDictionary<int, Attribute>> paramAnnotationsMap, int index)
        {

            var indexAnnotationsMap = new Dictionary<Type, IList<Attribute?>>();
            foreach (var pair in paramAnnotationsMap)
            {
                var attributes = new List<Attribute?>();
                for (var i = 0; i < index; i++)
                {
                    attributes.Add(null);
                }
                foreach (var indexPair in pair.Value)
                {
                    attributes[indexPair.Key] = indexPair.Value;
                }
                indexAnnotationsMap.Add(pair.Key, attributes.ToImmutableList());
            }
            return indexAnnotationsMap.ToImmutableDictionary();
        }

        private static PluginChain AppendPlugin(PluginChain? context, CommandPluginHolder plugin)
        {
            if (context == null)
            {
                context = new PluginChain(plugin);
            }
            context.Append(new PluginChain(plugin));
            return context;
        }

        /// <summary>
        /// 返回类型
        /// </summary>
        public Type ReturnType { get; }

        /// <summary>
        /// 参数
        /// </summary>
        public IList<ControllerParamDescription> ParamDescriptions { get; }

        /// <summary>
        /// 参数个数
        /// </summary>
        public int ParametersSize => ParamDescriptions.Count;

        /// <summary>
        /// 方法注解
        /// </summary>
        /// <returns></returns>
        private AttributeHolder MethodAttributeHolder { get; }

        public string SimpleName { get; }

        public int Protocol => rpcProfile.IsNotNull() ? rpcProfile.Protocol : -1;

        /// <summary>
        /// 处理的消息类型
        /// </summary>
        public MessageMode MessageMode => rpcProfile.Mode;

        public bool IsHasAuthValidator => AuthValidatorType != null;

        public override bool IsUserGroup(IMessagerType messagerType)
        {
            return UserGroups != null ? base.IsUserGroup(messagerType) : typeController.IsUserGroup(messagerType);
        }

        public override bool IsActiveByAppType(string appType)
        {
            return AppTypes != null ? base.IsActiveByAppType(appType) : typeController.IsActiveByAppType(appType);
        }

        public override bool IsActiveByScope(string scope)
        {
            return Scopes != null ? base.IsActiveByScope(scope) : typeController.IsActiveByScope(scope);
        }

        public override bool IsAuth()
        {
            return AuthAttribute != null ? base.IsAuth() : typeController.IsAuth();
        }

        public override Type? AuthValidatorType {
            get {
                var auth = AuthAttribute;
                return auth != null ? base.AuthValidatorType : typeController.AuthValidatorType;
            }
        }

        public override IList<CommandPluginHolder> BeforePlugins {
            get {
                var plugin = base.BeforePlugins;
                return plugin.Count == 0 ? typeController.BeforePlugins : plugin;
            }
        }

        public override IList<CommandPluginHolder> AfterPlugins {
            get {
                var plugin = base.AfterPlugins;
                return plugin.Count == 0 ? typeController.AfterPlugins : plugin;
            }
        }

        public object? GetParameterValue(int index, INetTunnel tunnel, IMessage message, object? body)
        {
            if (index >= ParamDescriptions.Count)
            {
                throw new ResultCodeException(NetResultCode.SERVER_EXECUTE_EXCEPTION,
                    $"{this} 获取 index 为 {index} 的ParamDesc越界, index < {ParamDescriptions.Count}");
            }

            var desc = ParamDescriptions[index];
            if (desc == null)
            {
                throw new ResultCodeException(NetResultCode.SERVER_EXECUTE_EXCEPTION,
                    $"{this} 获取 index 为 {index} 的ParamDesc为null");
            }
            return desc.GetValue(tunnel, message, body);
        }

        public override TAttribute GetTypeAttribute<TAttribute>()
        {
            return typeController.GetTypeAttribute<TAttribute>();
        }

        public override IList<TAttribute> GetTypeAttributes<TAttribute>()
        {
            return typeController.GetTypeAttributes<TAttribute>();
        }

        public TAttribute? GetMethodAttribute<TAttribute>() where TAttribute : Attribute
        {
            return MethodAttributeHolder.GetAttribute<TAttribute>();
        }

        public IList<TAttribute> GetMethodAttributes<TAttribute>() where TAttribute : Attribute
        {
            return MethodAttributeHolder.GetAttributes<TAttribute>();
        }

        public IList<Attribute?> GetParameterAnnotationsByType(Type type)
        {
            return !indexParamAnnotationsMap.TryGetValue(type, out var attributes)
                ? ImmutableList.Create<Attribute?>()
                : attributes;
        }

        public IList<Attribute> GetParameterAnnotationsByIndex(int index)
        {
            return ParamDescriptions[index].Attributes;
        }

        public IList<Type> GetParameterAnnotationTypes()
        {
            return ImmutableList.CreateRange(indexParamAnnotationsMap.Keys);
        }

        public object Invoke(INetTunnel tunnel, IMessage message)
        {
            // 获取调用方法的参数类型
            var parameters = new object [ParametersSize];
            var body = message.Body;
            for (var index = 0; index < parameters.Length; index++)
            {
                parameters[index] = GetParameterValue(index, tunnel, message, body)!;
            }
            return invoker.Invoke(executor, parameters);
        }

        public void BeforeInvoke(ITunnel tunnel, IMessage message, RpcInvokeContext context)
        {
            if (beforeChain == null)
            {
                return;
            }
            beforeChain.Execute(tunnel, message, context);
        }

        public void AfterInvoke(ITunnel tunnel, IMessage message, RpcInvokeContext context)
        {
            if (afterChain == null)
            {
                return;
            }
            afterChain.Execute(tunnel, message, context);
        }

        public override string ToString()
        {
            return $"{nameof(Name)}: {Name}";
        }
    }

}
