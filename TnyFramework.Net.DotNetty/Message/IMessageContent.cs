using System;
namespace TnyFramework.Net.DotNetty.Message
{
    /// <summary>
    /// 消息内容接口
    /// </summary>
    public interface IMessageContent : IMessageSchema
    {
        /// <summary>
        /// 结果码
        /// </summary>
        int GetCode();


        /// <summary>
        /// 是否存在消息
        /// </summary>
        bool ExistBody { get; }

        /// <summary>
        /// 获取消息体
        /// </summary>
        object Body { get; }


        /// <summary>
        /// 获取消息体
        /// </summary>
        /// <typeparam name="T">消息体类型</typeparam>
        /// <returns></returns>
        T BodyAs<T>();
    }
}
