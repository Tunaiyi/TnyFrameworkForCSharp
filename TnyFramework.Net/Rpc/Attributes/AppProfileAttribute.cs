using System;
namespace TnyFramework.Net.Rpc.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class AppProfileAttribute : Attribute
    {
        public AppProfileAttribute(params string[] appTypes)
        {
            AppTypes = appTypes;
        }


        /// <summary>
        /// 限制应用类型
        /// </summary>
        public string[] AppTypes { get; }
    }
}
