// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

namespace TnyFramework.Net.Message
{

    public static class MessageHeaderKeys
    {
        /// <summary>
        /// RpcForwardHeader type proto id
        /// </summary>
        public const int RPC_FORWARD_TYPE_PROTO = 100;

        /// <summary>
        /// RpcForwardHeader key name
        /// </summary>
        public const string RPC_FORWARD_KEY = "Rpc-Forward";

        /// <summary>
        /// RpcForwardHeader MessageHeaderKey
        /// </summary>
        public static readonly MessageHeaderKey<RpcForwardHeader> RPC_FORWARD_HEADER =
            MessageHeaderKey<RpcForwardHeader>.Of(RPC_FORWARD_KEY, MessageHeaderUsage.Transient);

        /// <summary>
        /// RpcOriginalMessageIdHeader type proto id
        /// </summary>
        public const int RPC_ORIGINAL_MESSAGE_ID_TYPE_PROTO = 101;

        /// <summary>
        /// RpcOriginalMessageIdHeader key name
        /// </summary>
        public const string RPC_ORIGINAL_MESSAGE_ID_KEY = "Rpc-Original-Message-Id";

        /// <summary>
        /// RpcOriginalMessageIdHeader MessageHeaderKey
        /// </summary>
        public static readonly MessageHeaderKey<RpcOriginalMessageIdHeader> RPC_ORIGINAL_MESSAGE_ID_HEADER =
            MessageHeaderKey<RpcOriginalMessageIdHeader>.Of(RPC_ORIGINAL_MESSAGE_ID_KEY, MessageHeaderUsage.Feedback);

        public const int RPC_TRACING_TYPE_PROTO = 102;

        public const string RPC_TRACING_TYPE_PROTO_KEY = "Rpc-Tracing";

        public static readonly MessageHeaderKey<RpcTracingHeader> RPC_TRACING_HEADER =
            MessageHeaderKey<RpcTracingHeader>.Of(RPC_TRACING_TYPE_PROTO_KEY, MessageHeaderUsage.Infect);
    }

}
