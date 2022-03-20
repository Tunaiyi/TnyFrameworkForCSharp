using System;
namespace TnyFramework.Net.Rpc.Attributes
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class UserIdAttribute : Attribute
    {
        /// <summary>
        /// Message 的用户 id
        /// </summary>
        /// <param name="require"></param>
        public UserIdAttribute(bool require = true)
        {
        }
    }
}
