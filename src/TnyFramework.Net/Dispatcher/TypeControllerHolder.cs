using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using TnyFramework.Net.Rpc.Attributes;
namespace TnyFramework.Net.Dispatcher
{
    public class TypeControllerHolder : ControllerHolder
    {
        private readonly AttributeHolder attributeHolder;


        protected static IList<TAttribute> FindParamAttributes<TAttribute>(MemberInfo memberInfo) where TAttribute : Attribute
        {
            var newAttributes = memberInfo.GetCustomAttributes<TAttribute>().ToList();
            return newAttributes.ToImmutableList();
        }


        public TypeControllerHolder(object executor, MessageDispatcherContext context) :
            base(executor)
        {
            var type = executor.GetType();
            var rpcController = type.GetCustomAttribute<RpcControllerAttribute>();
            if (rpcController == null)
            {
                throw new NullReferenceException($"{type} [{typeof(RpcControllerAttribute)}] 特性不存在");
            }
            Init(context, rpcController.MessageModes,
                type.GetCustomAttributes<BeforePluginAttribute>(),
                type.GetCustomAttributes<AfterPluginAttribute>(),
                type.GetCustomAttribute<AuthenticationRequiredAttribute>(),
                type.GetCustomAttribute<AppProfileAttribute>(),
                type.GetCustomAttribute<ScopeProfileAttribute>());
            // var findMethod = ControllerType.GetMethod("FindParamAttributes");
            // if (findMethod == null)
            //     throw new NullReferenceException();
            attributeHolder = new AttributeHolder(ControllerType);
            MethodControllers = InitMethodHolder(executor, context);
        }



        private static RpcAttribute FindRpcAttribute(MemberInfo info)
        {
            var request = info.GetCustomAttribute<RpcRequestAttribute>();
            if (request != null)
                return request;
            var push = info.GetCustomAttribute<RpcPushAttribute>();
            if (push != null)
                return push;
            var response = info.GetCustomAttribute<RpcResponseAttribute>();
            if (response != null)
                return response;
            return info.GetCustomAttribute<RpcAttribute>();
            throw new NullReferenceException(
                $"{info} 没有存在注解 {typeof(RpcRequestAttribute)},  {typeof(RpcPushAttribute)}, {typeof(RpcResponseAttribute)}, {typeof(RpcAttribute)} 中的一个");
        }


        private IList<MethodControllerHolder> InitMethodHolder(object executor, MessageDispatcherContext context)
        {
            var type = executor.GetType();
            var methods = (
                from method in type.GetMethods()
                let rpcAttribute = FindRpcAttribute(method)
                where rpcAttribute != null
                select new MethodControllerHolder(executor, method, rpcAttribute, context, this)
                into holder
                where holder.Protocol > 0
                select holder).ToList();
            return methods.ToImmutableList();
        }


        public IList<MethodControllerHolder> MethodControllers { get; }


        public override TAttribute GetTypeAttribute<TAttribute>()
        {
            var attributes = GetTypeAttributes<TAttribute>();
            var value = (attributes.Count > 0 ? attributes[0] : default);
            return value;
        }


        public override IList<TAttribute> GetTypeAttributes<TAttribute>()
        {
            return attributeHolder.GetAttributes<TAttribute>();
        }
    }
}
