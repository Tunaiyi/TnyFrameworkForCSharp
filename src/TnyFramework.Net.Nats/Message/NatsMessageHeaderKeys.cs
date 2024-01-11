// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using TnyFramework.Net.Message;

namespace TnyFramework.Net.Nats.Message
{

    public static class NatsMessageHeaderKeys
    {
        /// <summary>
        /// RpcForwardHeader key name
        /// </summary>
        private const string NATS_RELAY_KEY = "Nats-Relay";

        /// <summary>
        /// RpcForwardHeader MessageHeaderKey
        /// </summary>
        public static readonly MessageHeaderKey<NatsRelayMessageHeader> NATS_RELAY_HEADER =
            MessageHeaderKey<NatsRelayMessageHeader>.Of(NATS_RELAY_KEY, MessageHeaderUsage.LocalFeedback);
    }

}
