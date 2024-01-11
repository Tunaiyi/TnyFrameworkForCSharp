// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System.Collections.Generic;

namespace TnyFramework.Net.Nats.Options
{

    public class NatsServerSetting : INatsServerSetting
    {
        public string Name { get; set; } = "";

        public string Service => Name;

        public string ServeName { get; set; } = "";

        public string Host { get; set; } = "";

        public int Port { get; set; } = 4222;

        public string Scheme { get; set; } = "nats";

        public string ServeHost { get; set; } = "";

        public int ServePort { get; set; }

        public int ServerId { get; set; }

        public int ConnectTimeout { get; set; } = 5000;

        public int ListenerSize { get; set; } = 1;

        public PubSubRpcOptions RpcOptions { get; set; } = new();

        public IDictionary<string, string>? Metadata { get; set; }

        public IPubSubRpcOptions GetRpcOptions()
        {
            return RpcOptions;
        }
    }

}
