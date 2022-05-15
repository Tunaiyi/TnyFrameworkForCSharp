using System;
using TnyFramework.Net.Message;

namespace TnyFramework.Net.Attributes
{

    [AttributeUsage(AttributeTargets.Method)]
    public class RpcAttribute : RpcProtocolAttribute
    {
        public RpcAttribute(int protocol, params MessageMode[] modes) : base(protocol, modes)
        {
        }

        public RpcAttribute(int protocol, int line, params MessageMode[] modes) : base(protocol, line, modes)
        {
        }
    }

}
