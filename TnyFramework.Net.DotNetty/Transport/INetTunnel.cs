using TnyFramework.Net.DotNetty.Message;
namespace TnyFramework.Net.DotNetty.Transport
{
    public interface INetTunnel : ITunnel, IReceiver, ISender
    {
        /// <summary>
        /// 访问 id
        /// </summary>
        /// <returns></returns>
        void SetAccessId(long accessId);


        /// <summary>
        /// Message 工厂
        /// </summary>
        IMessageFactory MessageFactory { get; }


        /// <summary>
        /// 打开通道
        /// </summary>
        /// <returns></returns>
        bool Open();


        /// <summary>
        /// 关闭通道
        /// </summary>
        bool Close();


        /**
         * 断开连接
         */
        bool Disconnect();


    }
}
