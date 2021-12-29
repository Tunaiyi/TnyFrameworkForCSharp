//
//  文件名称：MessageType.cs
//  简   述：信息类型
//  创建标识：lrg 2021/7/21
//
using System;

namespace TnyFramework.Net.DotNetty.Message
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
            return (byte)value;
        }

    }
}

