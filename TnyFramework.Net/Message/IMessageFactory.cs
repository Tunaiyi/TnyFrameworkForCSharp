namespace TnyFramework.Net.Message
{
    /// <summary>
    /// 消息工厂
    /// </summary>
    public interface IMessageFactory
    {
        /// <summary>
        /// 通过 content 创建 message
        /// </summary>
        /// <param name="id">消息 di</param>
        /// <param name="subject"> 内容</param>
        /// <returns>创建的消息</returns>
        INetMessage Create(long id, IMessageContent subject);


        /// <summary>
        /// 创建 message
        /// </summary>
        /// <param name="head">消息头</param>
        /// <param name="body">消息体</param>
        /// <returns>创建消息</returns>
        INetMessage Create(INetMessageHead head, object body);
    }
}
