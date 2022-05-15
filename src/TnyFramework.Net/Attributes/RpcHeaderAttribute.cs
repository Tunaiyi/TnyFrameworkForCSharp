using System;

namespace TnyFramework.Net.Attributes
{

    [AttributeUsage(AttributeTargets.Parameter)]
    public class RpcHeaderAttribute : Attribute
    {
        public RpcHeaderAttribute(string key)
        {
            Key = key;
        }

        public string Key { get; }
    }

}
