using System;

namespace TnyFramework.Net.Attributes
{

    [AttributeUsage(AttributeTargets.Parameter)]
    public class RpcBodyAttribute : Attribute
    {
        public RpcBodyAttribute()
        {
        }
    }

}
