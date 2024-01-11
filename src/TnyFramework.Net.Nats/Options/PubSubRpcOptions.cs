// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

namespace TnyFramework.Net.Nats.Options
{

    public class PubSubRpcOptions : IPubSubRpcOptions
    {
        private const string TOPIC_CHANNEL_TOKEN = "ch";

        public int ConnectTimeout { get; set; } = 5000;

        public int Concurrency { get; set; } = 1;

        public int? MessageCapacity { get; set; }

        public int WriteMaxBufferSize { get; set; } = 16 * 1024 * 1024;

        public string TopicSeparator { get; init; } = ".";

        public string TopicPrefix { get; init; } = "rpc";

        public string TopicChannelToken { get; init; } = TOPIC_CHANNEL_TOKEN;
    }

}
