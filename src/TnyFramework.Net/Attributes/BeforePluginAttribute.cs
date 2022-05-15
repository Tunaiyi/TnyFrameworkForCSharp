using System;

namespace TnyFramework.Net.Attributes
{

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    public class BeforePluginAttribute : PluginAttribute
    {
        /// <summary>
        /// 执行前插件
        /// </summary>
        /// <param name="pluginType">插件类型, pluginType 必须 实现 ICommandPlugin</param>
        /// <param name="attribute">参数</param>
        public BeforePluginAttribute(Type pluginType, string attribute = default) : base(pluginType, attribute)
        {
        }

        public override bool IsBefore()
        {
            return true;
        }

        public override bool IsAfter()
        {
            return false;
        }
    }

}
