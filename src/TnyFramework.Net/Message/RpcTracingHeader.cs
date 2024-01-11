// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System.Collections.Generic;
using System.Linq;
using ProtoBuf;
using TnyFramework.Codec.Attributes;
using TnyFramework.Codec.ProtobufNet.Attributes;
using TnyFramework.Codec.ProtobufNet.TypeProtobuf;

namespace TnyFramework.Net.Message;

[Codable(ProtobufMimeType.PROTOBUF_TYPE)]
[TypeProtobuf(MessageHeaderKeys.RPC_TRACING_TYPE_PROTO)]
[ProtoContract]
public class RpcTracingHeader : SelfMessageHeader<RpcTracingHeader>
{
    [ProtoMember(1, IsPacked = true, OverwriteList = true)]
    public IDictionary<string, string> Attributes { get; set; } = new Dictionary<string, string>();

    public RpcTracingHeader() : base(MessageHeaderKeys.RPC_ORIGINAL_MESSAGE_ID_HEADER)
    {
    }
    // public override string Key => MessageHeaderKeys.RPC_TRACING_TYPE_PROTO_KEY;
    //
    // public override bool IsTransitive => true;

    public override string ToString()
    {
        return $"{nameof(Attributes)}: [{string.Join(",", Attributes.Select(kv => kv.Key + "=" + kv.Value).ToArray())}]";
    }
}
