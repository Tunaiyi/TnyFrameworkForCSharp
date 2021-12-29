namespace TnyFramework.Net.DotNetty.Bootstrap
{
    public class ServerSettings
    {
        public string Name { get; set; }

        /// <summary>
        /// 是否使用 libuv
        /// </summary>
        public bool IsLibuv { get; set; } = true;


        /// <summary>
        /// 绑定域名
        /// </summary>
        public string Host { get; set; } = "0.0.0.0";


        /// <summary>
        /// 绑定端口
        /// </summary>
        public int Port { get; set; } = 1088;
    }
}
