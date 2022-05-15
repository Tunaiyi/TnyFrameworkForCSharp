using System;
using TnyFramework.Net.Message;

namespace TnyFramework.Net.Attributes
{

    [AttributeUsage(AttributeTargets.Method)]
    public class RpcResponseAttribute : RpcProtocolAttribute
    {
        public RpcResponseAttribute(int protocol) : base(protocol, MessageMode.Response)
        {
        }

        public RpcResponseAttribute(int protocol, int line) : base(protocol, line, MessageMode.Response)
        {
        }
    }

}
