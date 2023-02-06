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
using TnyFramework.Net.Attributes;

namespace TnyFramework.Net.Command.Dispatcher
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
            Init(context,
                type.GetCustomAttributes<BeforePluginAttribute>(),
                type.GetCustomAttributes<AfterPluginAttribute>(),
                type.GetCustomAttribute<AuthenticationRequiredAttribute>(),
                type.GetCustomAttribute<AppProfileAttribute>(),
                type.GetCustomAttribute<ScopeProfileAttribute>());
            // var findMethod = ControllerType.GetMethod("FindParamAttributes");
            // if (findMethod == null)
            //     throw new NullReferenceException();
            attributeHolder = new AttributeHolder(ControllerType);
            MethodControllers = InitMethodHolder(executor, context, rpcController);
        }

        private static IList<RpcProfile> FindRpcProfiles(MethodInfo info, RpcControllerAttribute rpcController)
        {
            return RpcProfile.AllOf(info, rpcController.MessageModes);
        }

        private IList<MethodControllerHolder> InitMethodHolder(object executor, MessageDispatcherContext context,
            RpcControllerAttribute rpcController)
        {
            var type = executor.GetType();
            var methods = (
                from method in type.GetMethods()
                where method.DeclaringType != typeof(object)
                let profiles = FindRpcProfiles(method, rpcController)
                where !profiles.IsNullOrEmpty()
                from rpcProfile in profiles
                select new MethodControllerHolder(executor, method, rpcProfile, context, this)
                into holder
                where holder.Protocol > 0
                select holder).ToImmutableList();
            return methods;
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
