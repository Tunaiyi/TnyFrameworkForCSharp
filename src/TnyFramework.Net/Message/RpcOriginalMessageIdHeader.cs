// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using ProtoBuf;
using TnyFramework.Codec.Attributes;
using TnyFramework.Codec.ProtobufNet.Attributes;
using TnyFramework.Codec.ProtobufNet.TypeProtobuf;

namespace TnyFramework.Net.Message;

[Codable(ProtobufMimeType.PROTOBUF_TYPE)]
[TypeProtobuf(MessageHeaderKeys.RPC_ORIGINAL_MESSAGE_ID_TYPE_PROTO)]
[ProtoContract]
public class RpcOriginalMessageIdHeader : ValueMessageHeader<long>
{
    [ProtoMember(1)]
    public long MessageId { get; set; }

    public RpcOriginalMessageIdHeader() : base(MessageHeaderKeys.RPC_TRACING_HEADER)
    {
    }

    public override long GetValue()
    {
        return MessageId;
    }
}
