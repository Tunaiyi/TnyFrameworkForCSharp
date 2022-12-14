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
using TnyFramework.Net.Attributes;
using TnyFramework.Net.Base;
using TnyFramework.Net.Plugin;

namespace TnyFramework.Net.Dispatcher
{

    public abstract class ControllerHolder
    {
        protected ControllerHolder(object executor)
        {
            ControllerType = executor.GetType();
        }

        protected void Init(MessageDispatcherContext context,
            IEnumerable<BeforePluginAttribute> beforePlugins, IEnumerable<AfterPluginAttribute> afterPlugins,
            AuthenticationRequiredAttribute authAttribute, AppProfileAttribute appProfile, ScopeProfileAttribute scopeProfile)
        {
            AuthAttribute = authAttribute;
            BeforePlugins = InitPlugins(context.CommandPlugins, beforePlugins);
            AfterPlugins = InitPlugins(context.CommandPlugins, afterPlugins);
            UserGroups = authAttribute?.UserGroups;
            AppTypes = appProfile?.AppTypes;
            Scopes = scopeProfile?.Scopes;
        }

        private IList<CommandPluginHolder> InitPlugins(IList<ICommandPlugin> commandPlugins, IEnumerable<PluginAttribute> attributes)
        {
            IList<CommandPluginHolder> holders = new List<CommandPluginHolder>();
            foreach (var attribute in attributes)
            {
                var plugin = commandPlugins.First((plg) => attribute.PluginType.IsInstanceOfType(plg));
                if (plugin == null)
                {
                    throw new NullReferenceException($"{ControllerType} ??? {attribute.GetType()} ?????? {attribute.PluginType} ??? null");
                }
                var pluginHolder = new CommandPluginHolder(this, plugin, attribute);
                holders.Add(pluginHolder);
            }
            return ImmutableList.CreateRange(holders);
        }

        /// <summary>
        /// ???????????????
        /// </summary>
        public Type ControllerType { get; }

        public string Name { get; protected set; }

        /// <summary>
        /// ???????????????
        /// </summary>
        public virtual IList<CommandPluginHolder> BeforePlugins { get; private set; }

        /// <summary>
        /// ???????????????
        /// </summary>
        public virtual IList<CommandPluginHolder> AfterPlugins { get; private set; }

        /// <summary>
        /// ????????????
        /// </summary>
        public AuthenticationRequiredAttribute AuthAttribute { get; private set; }

        /// <summary>
        /// ?????????????????????
        /// </summary>
        public IList<string> UserGroups { get; private set; }

        /// <summary>
        /// ????????????
        /// </summary>
        public IList<string> AppTypes { get; private set; }

        /// <summary>
        /// ?????????
        /// </summary>
        public IList<string> Scopes { get; private set; }

        public virtual bool IsAuth()
        {
            return AuthAttribute != null && AuthAttribute.Enable;
        }

        public virtual Type AuthValidatorType {
            get {
                var auth = AuthAttribute;
                if (auth != null && auth.Enable && auth.Validator != null)
                {
                    return auth.Validator;
                }
                return null;
            }
        }

        public abstract TAttribute GetTypeAttribute<TAttribute>() where TAttribute : Attribute;

        public abstract IList<TAttribute> GetTypeAttributes<TAttribute>() where TAttribute : Attribute;

        // /// <summary>
        // /// ??????????????????????????????????????????????????????
        // /// </summary>
        // /// <typeparam name="TAttribute">?????????????????????</typeparam>
        // /// <returns>
        // /// ???????????? ( [A] int a, int b, [A] int c, [B] int d)
        // ///  ?????? A : [[A], null, [A], null] <br/>
        // ///  ?????? B : [null, null, null, [B]]
        // /// </returns>
        // public abstract IList<TAttribute> GetParamAttributes<TAttribute>() where TAttribute : Attribute;

        // /// <summary>
        // /// ???????????????????????????
        // /// </summary>
        // /// <typeparam name="TAttribute"></typeparam>
        // /// <returns>???????????? true ???????????? false</returns>
        // public abstract bool IsMethodAttributeExist<TAttribute>() where TAttribute : Attribute;

        // /// <summary>
        // /// ????????????????????????????????????
        // /// </summary>
        // /// <param name="index">??????????????????</param>
        // /// <returns>
        // ///     ????????????????????????????????? ( [A] [B] int a, int b, [A] int C)
        // ///     ?????? 0 : {[A], [B]}
        // ///     ?????? 1 : {}
        // ///     ?????? 2 : {[A]}
        // /// </returns>
        // public abstract List<Attribute> GetParamAnnotationsByIndex(int index);

        public virtual bool IsUserGroup(IMessagerType messagerType)
        {
            return UserGroups == null || UserGroups.Count == 0 || UserGroups.Contains(messagerType.Group);
        }

        public virtual bool IsActiveByAppType(string appType)
        {
            return AppTypes == null || AppTypes.Count == 0 || AppTypes.Contains(appType);
        }

        public virtual bool IsActiveByScope(string scope)
        {
            return Scopes == null || Scopes.Count == 0 || Scopes.Contains(scope);
        }

        public bool IsScopeLimit() => Scopes == null || Scopes.Count == 0;

        public IList<CommandPluginHolder> ControllerBeforePlugins { get; }

        public IList<CommandPluginHolder> ControllerAfterPlugins { get; }
    }

}
