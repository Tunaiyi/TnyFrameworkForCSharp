using System;
using TnyFramework.Net.Message;
namespace TnyFramework.Net.Rpc.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class RpcResponseAttribute : RpcAttribute
    {
        public RpcResponseAttribute(int protocol) : base(protocol, MessageMode.Response)
        {
        }


        public RpcResponseAttribute(int protocol, params int[] line) : base(protocol, MessageMode.Response)
        {
            Line = line;
        }
    }
}
