using System;

namespace TnyFramework.Net.Attributes
{

    [AttributeUsage(AttributeTargets.Parameter)]
    public class RpcFromAttribute : Attribute
    {
        public RpcFromAttribute()
        {
        }
    }

}
