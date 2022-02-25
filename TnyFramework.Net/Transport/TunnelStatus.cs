namespace TnyFramework.Net.Transport
{
    public enum TunnelStatus
    {
        /// <summary>
        /// 初始化
        /// </summary>
        Init = 1,

        ///
        /// 连接
        ///
        Open = 2,

        ///
        /// 挂起
        ///
        Suspend = 3,

        ///
        /// 关闭
        ///
        Closed = 4,
    }

    public static class TunnelStatusExtents
    {
        public static int Value(this TunnelStatus status)
        {
            return (int)status;
        }
    }
}
