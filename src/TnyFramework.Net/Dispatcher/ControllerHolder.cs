using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using TnyFramework.Net.Message;
using TnyFramework.Net.Plugin;
using TnyFramework.Net.Rpc.Attributes;
namespace TnyFramework.Net.Dispatcher
{
    public abstract class ControllerHolder
    {
        protected ControllerHolder(object executor)
        {
            ControllerType = executor.GetType();
        }


        protected void Init(MessageDispatcherContext context,
            IEnumerable<MessageMode> messageModes, IEnumerable<BeforePluginAttribute> beforePlugins, IEnumerable<AfterPluginAttribute> afterPlugins,
            AuthenticationRequiredAttribute authAttribute, AppProfileAttribute appProfile, ScopeProfileAttribute scopeProfile)
        {
            MessageModes = ImmutableList.CreateRange(messageModes);
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
                    throw new NullReferenceException($"{ControllerType} 的 {attribute.GetType()} 插件 {attribute.PluginType} 为 null");
                }
                var pluginHolder = new CommandPluginHolder(this, plugin, attribute);
                holders.Add(pluginHolder);
            }
            return ImmutableList.CreateRange(holders);
        }


        /// <summary>
        /// 控制器类型
        /// </summary>
        public Type ControllerType { get; }

        public string Name { get; protected set; }

        /// <summary>
        /// 执行前插件
        /// </summary>
        public virtual IList<CommandPluginHolder> BeforePlugins { get; private set; }

        /// <summary>
        /// 执行后插件
        /// </summary>
        public virtual IList<CommandPluginHolder> AfterPlugins { get; private set; }

        /// <summary>
        /// 处理的消息类型
        /// </summary>
        public virtual IList<MessageMode> MessageModes { get; private set; }


        /// <summary>
        /// 认证配置
        /// </summary>
        public AuthenticationRequiredAttribute AuthAttribute { get; private set; }

        /// <summary>
        /// 用户组名称列表
        /// </summary>
        public IList<string> UserGroups { get; private set; }

        /// <summary>
        /// 应用类型
        /// </summary>
        public IList<string> AppTypes { get; private set; }

        /// <summary>
        /// 作用域
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
        // /// 获取某方上参数指定特性类型的特性列表
        // /// </summary>
        // /// <typeparam name="TAttribute">指定的特性类型</typeparam>
        // /// <returns>
        // /// 特性列表 ( [A] int a, int b, [A] int c, [B] int d)
        // ///  获取 A : [[A], null, [A], null] <br/>
        // ///  获取 B : [null, null, null, [B]]
        // /// </returns>
        // public abstract IList<TAttribute> GetParamAttributes<TAttribute>() where TAttribute : Attribute;


        // /// <summary>
        // /// 指定的特性是否存在
        // /// </summary>
        // /// <typeparam name="TAttribute"></typeparam>
        // /// <returns>存在返回 true 否则返回 false</returns>
        // public abstract bool IsMethodAttributeExist<TAttribute>() where TAttribute : Attribute;


        // /// <summary>
        // /// 获取某个参数上的特性列表
        // /// </summary>
        // /// <param name="index">参数位置索引</param>
        // /// <returns>
        // ///     返回指定参数的特性列表 ( [A] [B] int a, int b, [A] int C)
        // ///     获取 0 : {[A], [B]}
        // ///     获取 1 : {}
        // ///     获取 2 : {[A]}
        // /// </returns>
        // public abstract List<Attribute> GetParamAnnotationsByIndex(int index);


        public virtual bool IsUserGroup(string group)
        {
            return UserGroups == null || UserGroups.Count == 0 || UserGroups.Contains(group);
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