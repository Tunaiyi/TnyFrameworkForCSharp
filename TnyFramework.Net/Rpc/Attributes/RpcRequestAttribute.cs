using System;
using TnyFramework.Net.Message;
namespace TnyFramework.Net.Rpc.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class RpcRequestAttribute : RpcAttribute
    {
        public RpcRequestAttribute(int protocol) : base(protocol, MessageMode.Request)
        {
        }


        public RpcRequestAttribute(int protocol, params int[] line) : base(protocol, MessageMode.Request)
        {
            Line = line;
        }
    }
}
