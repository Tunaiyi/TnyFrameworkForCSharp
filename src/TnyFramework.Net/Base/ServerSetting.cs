using System.Collections.Generic;
namespace TnyFramework.Net.Base
{
    public class ServerSetting : IServerSetting
    {
        public string Name { get; set; }

        public string ServeName { get; set; }

        /// <summary>
        /// 是否使用 libuv
        /// </summary>
        public bool Libuv { get; set; } = false;


        /// <summary>
        /// 绑定域名
        /// </summary>
        public string Host { get; set; } = "0.0.0.0";


        /// <summary>
        /// 绑定端口
        /// </summary>
        public int Port { get; set; } = 1088;

        /// <summary>
        /// 协议
        /// </summary>
        public string Scheme { get; set; } = "tcp";

        /// <summary>
        /// 上报绑定域名
        /// </summary>
        public string ServeHost { get; set; }


        /// <summary>
        /// 上报绑定端口
        /// </summary>
        public int ServePort { get; set; }

        /// <summary>
        /// 信息
        /// </summary>
        public IDictionary<string, string> Metadata { get; set; }
    }
}
