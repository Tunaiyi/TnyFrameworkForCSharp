using TnyFramework.Net.Message;
using TnyFramework.Net.Rpc;

namespace TnyFramework.Net.Dispatcher
{

    /// <summary>
    /// 转发器
    /// </summary>
    public interface IRpcForwarder
    {
        /// <summary>
        /// 获取转发 AccessPoint
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="from">发送的 Rpc 服务(可选)</param>
        /// <param name="sender">发送者(可选)</param>
        /// <param name="to">目标 Rpc 服务</param>
        /// <param name="receiver">接受者(可选)</param>
        /// <returns></returns>
        IRpcRemoteAccessPoint Forward(IMessage message, IRpcServicer from, IMessager sender, IRpcServicer to, IMessager receiver);
    }

}
