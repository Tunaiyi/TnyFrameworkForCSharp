using System;

namespace TnyFramework.Net.Attributes
{

    [AttributeUsage(AttributeTargets.Parameter)]
    public class RpcToAttribute : Attribute
    {
        public RpcToAttribute()
        {
        }
    }

}
