using System;
namespace TnyFramework.Net.Rpc.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    public class AfterPluginAttribute : PluginAttribute
    {
        /// <summary>
        /// 执行后插件
        /// </summary>
        /// <param name="pluginType">插件类型, pluginType 必须 实现 ICommandPlugin</param>
        /// <param name="attribute">参数</param>
        public AfterPluginAttribute(Type pluginType, string attribute = default) : base(pluginType, attribute)
        {
        }




        public override bool IsBefore()
        {
            return false;
        }


        public override bool IsAfter()
        {
            return true;
        }
    }
}
