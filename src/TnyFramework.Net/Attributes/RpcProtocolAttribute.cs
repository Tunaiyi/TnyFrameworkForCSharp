using System;
using TnyFramework.Net.Message;

namespace TnyFramework.Net.Attributes
{

    [AttributeUsage(AttributeTargets.Method)]
    public abstract class RpcProtocolAttribute : Attribute
    {
        protected RpcProtocolAttribute(int protocol, params MessageMode[] modes)
        {
            Protocol = protocol;
            MessageModes = modes;
        }

        protected RpcProtocolAttribute(int protocol, int line, params MessageMode[] modes)
        {
            Protocol = protocol;
            MessageModes = modes;
            Line = line;
        }

        /// <summary>
        /// 协议 id
        /// </summary>
        public int Protocol { get; }

        /// <summary>
        /// 接收线路 id
        /// </summary>
        public int Line { get; } = 0;

        /// <summary>
        /// 处理消息类型
        /// </summary>
        public MessageMode[] MessageModes { get; }
    }

}
