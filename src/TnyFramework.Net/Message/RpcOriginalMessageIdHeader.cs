// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using ProtoBuf;
using TnyFramework.Codec;
using TnyFramework.Codec.Attributes;
using TnyFramework.Codec.ProtobufNet.Attributes;

namespace TnyFramework.Net.Message
{

    [Codable(MimeTypes.PROTOBUF_TYPE)]
    [TypeProtobuf(MessageHeaderConstants.RPC_ORIGINAL_MESSAGE_ID_TYPE_PROTO)]
    [ProtoContract]
    public class RpcOriginalMessageIdHeader : MessageHeader<RpcOriginalMessageIdHeader>
    {
        [ProtoMember(1)]
        public long MessageId { get; set; }

        public override string Key => MessageHeaderConstants.RPC_ORIGINAL_MESSAGE_ID_KEY;
    }

}
