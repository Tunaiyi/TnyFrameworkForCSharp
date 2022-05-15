using System;

namespace TnyFramework.Net.Attributes
{

    [AttributeUsage(AttributeTargets.Parameter)]
    public class RpcOptionalAttribute : Attribute
    {
        public RpcOptionalAttribute()
        {
        }
    }

}
