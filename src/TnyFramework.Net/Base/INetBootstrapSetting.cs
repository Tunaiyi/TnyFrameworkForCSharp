namespace TnyFramework.Net.Base
{
    public interface INetBootstrapSetting
    {
        /// <summary>
        /// 服务名
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 服务发现名
        /// </summary>
        string ServeName { get; }
    }
}
