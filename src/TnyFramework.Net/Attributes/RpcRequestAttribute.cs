using System;
using TnyFramework.Net.Message;

namespace TnyFramework.Net.Attributes
{

    [AttributeUsage(AttributeTargets.Method)]
    public class RpcRequestAttribute : RpcProtocolAttribute
    {
        public RpcRequestAttribute(int protocol) : base(protocol, MessageMode.Request)
        {
        }

        public RpcRequestAttribute(int protocol, int line) : base(protocol, line, MessageMode.Request)
        {
        }
    }

}
