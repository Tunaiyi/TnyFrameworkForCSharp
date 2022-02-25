using System;
namespace TnyFramework.Net.Rpc.Attributes
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class MsgCodeAttribute : Attribute
    {
        /// <summary>
        /// Message 的 result code 值
        /// </summary>
        public MsgCodeAttribute()
        {
        }
    }
}
