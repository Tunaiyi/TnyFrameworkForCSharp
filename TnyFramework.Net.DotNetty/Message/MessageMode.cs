//
//  文件名称：MessageMode.cs
//  简   述：信息模式
//  创建标识：lrg 2021/7/21
//
using System;

namespace TnyFramework.Net.DotNetty.Message
{
    [Flags]
    public enum MessageMode
    {
        /// <summary>
        /// 请求
        /// </summary>
        Request = 0,

        /// <summary>
        /// 响应
        /// </summary>
        Response = 1,

        /// <summary>
        /// 推送
        /// </summary>
        Push = 2,

        /// <summary>
        /// ping
        /// </summary>
        Ping = 0xFF,

        /// <summary>
        /// pong
        /// </summary>
        Pong = 0xFE,
    }

    public static class MessageModeExtensions
    {
        public static byte GetOption(this MessageMode value)
        {
            return (byte)value;
        }

        public static MessageType GetMessageType(this MessageMode self)
        {
            switch (self)
            {
                case MessageMode.Ping:
                    return MessageType.Ping;
                case MessageMode.Pong:
                    return MessageType.Pong;
                case MessageMode.Request:
                case MessageMode.Response:
                case MessageMode.Push:
                default:
                    return MessageType.Message;
            }
        }
    }

}
