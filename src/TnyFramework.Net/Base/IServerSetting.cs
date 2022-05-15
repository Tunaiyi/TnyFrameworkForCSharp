using System.Collections.Generic;

namespace TnyFramework.Net.Base
{

    public interface IServerSetting : IServerBootstrapSetting
    {
        /// <summary>
        /// 上报绑定域名
        /// </summary>
        string ServeHost { get; }

        /// <summary>
        /// 上报绑定端口
        /// </summary>
        int ServePort { get; }

        /// <summary>
        /// 是否是使用 libuv
        /// </summary>
        bool Libuv { get; set; }

        /// <summary>
        /// 信息
        /// </summary>
        IDictionary<string, string> Metadata { get; }
    }

}
