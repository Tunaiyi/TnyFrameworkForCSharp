using TnyFramework.Net.Endpoint;

namespace TnyFramework.Net.Transport
{

    public interface ITunnel : ICommunicator, IConnection
    {
        /// <summary>
        /// 通道 Id
        /// </summary>
        long Id { get; }

        /// <summary>
        /// 访问 id
        /// </summary>
        /// <returns></returns>
        long AccessId { get; }

        /// <summary>
        /// 通道模式
        /// </summary>
        TunnelMode Mode { get; }

        /// <summary>
        /// 通道状态
        /// </summary>
        TunnelStatus Status { get; }

        /// <summary>
        /// 是否已经开启
        /// </summary>
        /// <returns></returns>
        bool IsOpen();

        /// <summary>
        /// 会话
        /// </summary>
        IEndpoint GetEndpoint();
    }

    public interface ITunnel<out TUserId> : ICommunicator<TUserId>, ITunnel
    {
        /// <summary>
        /// 会话
        /// </summary>
        IEndpoint<TUserId> Endpoint { get; }
    }

}
