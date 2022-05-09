namespace TnyFramework.Net.Message
{

    public static class MessageHeaderConstants
    {
        /// <summary>
        /// RpcForwardHeader type proto id
        /// </summary>
        public const int RPC_FORWARD_HEADER_TYPE_PROTO = 100;

        /// <summary>
        /// RpcForwardHeader key name
        /// </summary>
        public const string RPC_FORWARD_HEADER_KEY = "Rpc-Forward";

        /// <summary>
        /// RpcForwardHeader MessageHeaderKey
        /// </summary>
        public static readonly MessageHeaderKey<RpcForwardHeader> RPC_FORWARD_HEADER =
            MessageHeaderKey<RpcForwardHeader>.Of(RPC_FORWARD_HEADER_KEY);

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
        public static readonly MessageHeaderKey<RpcOriginalMessageIdHeader> RPC_ORIGINAL_MESSAGE_ID =
            MessageHeaderKey<RpcOriginalMessageIdHeader>.Of(RPC_ORIGINAL_MESSAGE_ID_KEY);
    }

}
