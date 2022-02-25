using System;
using TnyFramework.Net.Message;
namespace TnyFramework.Net.Rpc.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class RpcAttribute : Attribute
    {
        public RpcAttribute(int protocol, params MessageMode[] modes)
        {
            Protocol = protocol;
            MessageModes = modes;
        }


        public RpcAttribute(int protocol, int line, params MessageMode[] modes)
        {
            Protocol = protocol;
            MessageModes = modes;
            Line = new[] { line };
        }


        /// <summary>
        /// 协议 id
        /// </summary>
        public int Protocol { get; set; }

        /// <summary>
        /// 接收线路 id
        /// </summary>
        public int[] Line { get; set; } = { };

        /// <summary>
        /// 处理消息类型
        /// </summary>
        public MessageMode[] MessageModes { get; set; }
    }
}
