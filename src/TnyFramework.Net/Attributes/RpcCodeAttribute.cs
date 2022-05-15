using System;

namespace TnyFramework.Net.Attributes
{

    [AttributeUsage(AttributeTargets.Parameter)]
    public class RpcCodeAttribute : Attribute
    {
        /// <summary>
        /// Message 的 result code 值
        /// </summary>
        public RpcCodeAttribute()
        {
        }
    }

}
