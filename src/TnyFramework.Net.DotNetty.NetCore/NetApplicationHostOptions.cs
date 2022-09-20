// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using Microsoft.Extensions.Configuration;
using TnyFramework.Net.Base;

namespace TnyFramework.Net.DotNetty.NetCore
{

    public class NetApplicationHostOptions
    {
        public static readonly string ROOT_PATH = ConfigurationPath.Combine("Tny", "Net");

        public string Name { get; set; }

        public int ServerId { get; set; }

        public string AppType { get; set; } = "default";

        public string ScopeType { get; set; } = "online";

        public string Locale { get; set; } = "zh-CN";

        public ServerSetting Server { get; set; }

        public ServerSetting RpcServer { get; set; }
    }

}
