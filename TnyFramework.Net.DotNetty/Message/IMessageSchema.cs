namespace TnyFramework.Net.DotNetty.Message
{
    /// <summary>
    /// 消息结构
    /// </summary>
    public interface IMessageSchema : IProtocol
    {
        /// <summary>
        /// 响应消息, -1 为无
        /// </summary>
        long ToMessage { get; }

        /// <summary>
        /// 消息类型
        /// </summary>
        MessageType Type { get; }

        /// <summary>
        /// 获取消息模式
        /// </summary>
        MessageMode Mode { get; }
    }
}
