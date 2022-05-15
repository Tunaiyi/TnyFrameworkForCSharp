using System;

namespace TnyFramework.Net.Rpc.Attributes
{

    /// <summary>
    /// 忽略作为远程参数/消息体
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public class RpcIgnoreAttribute : Attribute
    {
    }

}
