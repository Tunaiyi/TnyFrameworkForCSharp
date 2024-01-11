// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using ProtoBuf;

namespace TnyFramework.Net.Nats.Codecs.TypeProtobuf.Proto
{

    [ProtoContract]
    public class PlayerProto : PooledProto<PlayerProto>
    {
        [ProtoMember(1, Name = "id")]
        public long Id { get; set; }

        [ProtoMember(2, Name = "toMessage")]
        public string Name { get; set; }

        public PlayerProto()
        {
            Name = null!;
            Id = 0;
        }

        public PlayerProto(long id, string name)
        {
            Id = id;
            Name = name;
        }

        protected override void DoClear()
        {
            Name = null!;
            Id = 0;
        }
    }

}
