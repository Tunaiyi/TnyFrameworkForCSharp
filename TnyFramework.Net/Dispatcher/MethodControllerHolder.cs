using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;
using System.Text;
using TnyFramework.Common.Exception;
using TnyFramework.Common.Invoke;
using TnyFramework.Net.Common;
using TnyFramework.Net.Message;
using TnyFramework.Net.Plugin;
using TnyFramework.Net.Rpc.Attributes;
using TnyFramework.Net.Transport;
namespace TnyFramework.Net.Dispatcher
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
        private readonly PluginChain beforeChain;

        /// <summary>
        /// 执行后
        /// </summary>
        private readonly PluginChain afterChain;

        /// <summary>
        /// 类 Controller 信息
        /// </summary>
        private readonly TypeControllerHolder typeController;

        /// <summary>
        /// 参数注解
        /// </summary>
        private readonly IDictionary<Type, IList<Attribute>> indexParamAnnotationsMap;


        public void Run()
        {
        }


        public MethodControllerHolder(object executor, MethodInfo method, RpcAttribute rpcController, MessageDispatcherContext context,
            TypeControllerHolder typeController) :
            base(executor)
        {
            invoker = method.ReturnType == typeof(void)
                ? FastActionFactory.CreateFactory(method).CreateInvoker(method)
                : FastFuncFactory.CreateFactory(method).CreateInvoker(method);
            Init(context, rpcController.MessageModes,
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
            nameBuilder.Append(type.Name).Append("#").Append(method.Name);
            var parameterInfos = method.GetParameters();
            var indexCounter = 0;
            var paramAnnotationsMap = new Dictionary<Type, IDictionary<int, Attribute>>();
            ParamDescriptions = InitParamDescriptions(paramAnnotationsMap, parameterInfos, nameBuilder, indexCounter, ref indexCounter);
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
            Protocol = rpcController.Protocol;
            indexParamAnnotationsMap = InitIndexParamAttributes(paramAnnotationsMap, ParamDescriptions.Count);
            MethodAttributeHolder = new AttributeHolder(method);

        }


        private IList<ParamDescription> InitParamDescriptions(IDictionary<Type, IDictionary<int, Attribute>> paramAnnotationsMap,
            IEnumerable<ParameterInfo> parameterInfos, StringBuilder nameBuilder, int indexCounter, ref int index)
        {
            var paramDescriptions = new List<ParamDescription>();
            foreach (var parameterInfo in parameterInfos)
            {
                nameBuilder.Append(index > 0 ? ", " : "(");
                nameBuilder.Append(parameterInfo.ParameterType);
                var paramDescription = new ParamDescription(this, parameterInfo, ref indexCounter);
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


        private static IDictionary<Type, IList<Attribute>> InitIndexParamAttributes(
            Dictionary<Type, IDictionary<int, Attribute>> paramAnnotationsMap, int index)
        {

            var indexAnnotationsMap = new Dictionary<Type, IList<Attribute>>();
            foreach (var pair in paramAnnotationsMap)
            {
                var attributes = new List<Attribute>();
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



        private static PluginChain AppendPlugin(PluginChain context, CommandPluginHolder plugin)
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
        public IList<ParamDescription> ParamDescriptions { get; }

        /// <summary>
        /// 参数个数
        /// </summary>
        public int ParametersSize => ParamDescriptions.Count;


        /// <summary>
        /// 方法注解
        /// </summary>
        /// <returns></returns>
        private AttributeHolder MethodAttributeHolder { get; }

        public int Protocol { get; }

        public override IList<MessageMode> MessageModes {
            get {
                var modes = base.MessageModes;
                if (modes != null && modes.Count > 0)
                {
                    return modes;
                }
                return typeController.MessageModes;
            }
        }



        public override bool IsUserGroup(String group)
        {
            return UserGroups != null ? base.IsUserGroup(group) : typeController.IsUserGroup(group);
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


        public override Type AuthValidatorType {
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


        public object GetParameterValue(int index, INetTunnel tunnel, IMessage message, object body)
        {
            if (index >= ParamDescriptions.Count)
            {
                throw new ResultCodeException(NetResultCode.SERVER_EXECUTE_EXCEPTION,
                    $"{this} 获取 index 为 {index} 的ParamDesc越界, index < {ParamDescriptions.Count}");
            }

            var desc = ParamDescriptions[index];
            if (desc == null)
            {
                throw new ResultCodeException(NetResultCode.SERVER_EXECUTE_EXCEPTION, $"{this} 获取 index 为 {index} 的ParamDesc为null");
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


        public TAttribute GetMethodAttribute<TAttribute>() where TAttribute : Attribute
        {
            return MethodAttributeHolder.GetAttribute<TAttribute>();
        }


        public IList<TAttribute> GetMethodAttributes<TAttribute>() where TAttribute : Attribute
        {
            return MethodAttributeHolder.GetAttributes<TAttribute>();
        }



        public IList<Attribute> GetParameterAnnotationsByType(Type type)
        {
            return !indexParamAnnotationsMap.TryGetValue(type, out var attributes) ? ImmutableList.Create<Attribute>() : attributes;
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
                parameters[index] = GetParameterValue(index, tunnel, message, body);
            }
            return invoker.Invoke(executor, parameters);
        }


        public void BeforeInvoke(ITunnel tunnel, IMessage message, MessageCommandContext context)
        {
            if (beforeChain == null)
            {
                return;
            }
            beforeChain.Execute(tunnel, message, context);
        }


        public void AfterInvoke(ITunnel tunnel, IMessage message, MessageCommandContext context)
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
