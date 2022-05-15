using System.Threading.Tasks;
using TnyFramework.Net.Endpoint;
using TnyFramework.Net.Message;

namespace TnyFramework.Net.Transport
{

    public interface ITransport
    {
        /// <summary>
        /// 写出消息
        /// </summary>
        /// <param name="message">发送消息</param>
        /// <returns>发送promise</returns>
        Task Write(IMessage message);

        /// <summary>
        /// 写出消息
        /// </summary>
        /// <param name="allocator">消费分发器</param>
        /// <param name="messageContext">消息上下文</param>
        /// <returns></returns>
        Task Write(MessageAllocator allocator, MessageContext messageContext);

        /// <summary>
        /// 写出消息
        /// </summary>
        /// <param name="allocator">消费分发器</param>
        /// <param name="messageContext">消息上下文</param>
        /// <returns></returns>
        Task Write(IMessageAllocator allocator, MessageContext messageContext);
    }

}
