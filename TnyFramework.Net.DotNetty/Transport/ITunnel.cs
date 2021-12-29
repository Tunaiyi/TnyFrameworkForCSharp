namespace TnyFramework.Net.DotNetty.Transport
{
    public interface ITunnel
    {
        /// <summary>
        /// 访问 id
        /// </summary>
        /// <returns></returns>
        long AccessId { get; }

        /// <summary>
        /// 通道模式
        /// </summary>
        TunnelMode Mode { get; }
    }
}
