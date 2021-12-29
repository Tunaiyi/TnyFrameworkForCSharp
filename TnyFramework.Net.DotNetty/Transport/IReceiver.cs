using TnyFramework.Net.DotNetty.Message;
namespace TnyFramework.Net.DotNetty.Transport
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
