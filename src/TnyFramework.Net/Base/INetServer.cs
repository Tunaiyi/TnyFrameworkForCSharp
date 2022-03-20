namespace TnyFramework.Net.Base
{
    public interface INetServer
    {
        /// <summary>
        /// 服务名
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 设置
        /// </summary>
        IServerSetting Setting { get; }


        /// <summary>
        /// 是否启动
        /// </summary>
        /// <returns></returns>
        bool IsOpen();
    }
}
