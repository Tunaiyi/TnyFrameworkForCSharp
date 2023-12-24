// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System.Collections.Generic;

namespace TnyFramework.Net.Application
{

    public class ServiceServerSetting : IServiceServerSetting
    {
        public string Name {
            get => Service;
            set => Service = value;
        }

        public string Service { get; private set; } = "";

        public string ServeName { get; set; } = "";

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
        public string ServeHost { get; set; } = "";

        /// <summary>
        /// 上报绑定端口
        /// </summary>
        public int ServePort { get; set; }

        /// <summary>
        /// 信息
        /// </summary>
        public IDictionary<string, string>? Metadata { get; set; }
    }

}
