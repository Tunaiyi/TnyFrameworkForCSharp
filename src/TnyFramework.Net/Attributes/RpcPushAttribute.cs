using System;
using TnyFramework.Net.Message;

namespace TnyFramework.Net.Attributes
{

    [AttributeUsage(AttributeTargets.Method)]
    public class RpcPushAttribute : RpcProtocolAttribute
    {
        public RpcPushAttribute(int protocol) : base(protocol, MessageMode.Push)
        {
        }

        public RpcPushAttribute(int protocol, int line) : base(protocol, line, MessageMode.Push)
        {

        }
    }

}
