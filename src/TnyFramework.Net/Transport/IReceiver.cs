using TnyFramework.Net.Message;

namespace TnyFramework.Net.Transport
{

    public interface IReceiver
    {
        /// <summary>
        /// 接收消息
        /// </summary>
        /// <param name="message">消息</param>
        /// <returns>是否接收成功</returns>
        bool Receive(IMessage message);
    }

}
