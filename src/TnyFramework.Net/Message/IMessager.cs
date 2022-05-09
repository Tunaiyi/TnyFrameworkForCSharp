using TnyFramework.Net.Base;

namespace TnyFramework.Net.Message
{

    /// <summary>
    /// 消息发送接收者
    /// </summary>
    public interface IMessager
    {
        /// <summary>
        /// 消息者 Id
        /// </summary>
        long MessagerId { get; }

        /// <summary>
        /// 消息者类型
        /// </summary>
        IMessagerType MessagerType { get; }
    }

}
