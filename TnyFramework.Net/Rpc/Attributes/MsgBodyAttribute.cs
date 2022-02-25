using System;
namespace TnyFramework.Net.Rpc.Attributes
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class MsgBodyAttribute : Attribute
    {
        public MsgBodyAttribute(bool require = true)
        {
        }


        /// <summary>
        /// 是否是必须的, 默认为 true
        /// </summary>
        public bool Require { get; set; } = true;
    }
}
