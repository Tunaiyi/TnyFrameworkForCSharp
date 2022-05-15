using System.Threading.Tasks;
using TnyFramework.Net.Endpoint;
using TnyFramework.Net.Message;

namespace TnyFramework.Net.Transport
{

    public interface IMessageTransporter : IConnection
    {
        /// <summary>
        /// 绑定通道通道
        /// </summary>
        /// <param name="tunnel">通道</param>
        void Bind(INetTunnel tunnel);

        /// <summary>
        /// 发送消
        /// </summary>
        /// <param name="message">消息</param>
        /// <returns>写出等待对象</returns>
        Task Write(IMessage message);

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="maker">消息创建器</param>
        /// <param name="factory">消息消息工厂</param>
        /// <param name="context">消息上下文</param>
        /// <returns>写出等待对象</returns>
        Task Write(IMessageAllocator maker, IMessageFactory factory, MessageContext context);

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="maker">消息创建器</param>
        /// <param name="factory">消息消息工厂</param>
        /// <param name="context">消息上下文</param>
        /// <returns>写出等待对象</returns>
        Task Write(MessageAllocator maker, IMessageFactory factory, MessageContext context);
    }

}
