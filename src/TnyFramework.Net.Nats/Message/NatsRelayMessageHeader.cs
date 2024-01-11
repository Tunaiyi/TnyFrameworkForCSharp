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

    public class NatsRelayMessageHeader : ValueMessageHeader<string>
    {
        public string Relay { get; internal init; }

        public NatsRelayMessageHeader(string relay) : base(NatsMessageHeaderKeys.NATS_RELAY_HEADER)
        {
            Relay = relay;
        }

        public override string GetValue()
        {
            return Relay;
        }
    }

}
