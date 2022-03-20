using System;
using TnyFramework.Net.Message;
namespace TnyFramework.Net.Rpc.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class RpcPushAttribute : RpcAttribute
    {
        public RpcPushAttribute(int protocol) : base(protocol, MessageMode.Push)
        {
        }


        public RpcPushAttribute(int protocol, params int[] line) : base(protocol, MessageMode.Push)
        {
            Line = line;
        }
    }
}
