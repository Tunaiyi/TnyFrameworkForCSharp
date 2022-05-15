using System;

namespace TnyFramework.Net.Message
{

    [Flags]
    public enum MessageType
    {
        /// <summary>
        /// 处理请求
        /// </summary>
        Message = 0,

        /// <summary>
        /// Ping
        /// </summary>
        Ping = 1,

        /// <summary>
        /// Pong
        /// </summary>
        Pong = 2,
    }

    public static class MessageTypeExtensions
    {
        public static byte GetOption(this MessageType value)
        {
            return (byte) value;
        }
    }

}
