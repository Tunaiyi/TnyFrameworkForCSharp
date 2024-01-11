// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.Collections.Generic;
using System.Linq;
using ProtoBuf;
using TnyFramework.Common.Result;
using TnyFramework.Net.Message;

namespace TnyFramework.Net.Nats.Codecs.TypeProtobuf.Proto
{

    [ProtoContract]
    public class MessageHeadProto : PooledProto<MessageHeadProto>
    {
        #region MessageHeader

        [ProtoMember(1, Name = "id")]
        public long Id { get; set; }

        [ProtoMember(2, Name = "toMessage")]
        public long ToMessage { get; set; }

        [ProtoMember(3, Name = "protocolId")]
        public int ProtocolId { get; set; }

        [ProtoMember(4, Name = "code")]
        public int Code { get; set; }

        [ProtoMember(5, Name = "time", DataFormat = DataFormat.FixedSize)]
        public long Time { get; set; }

        [ProtoMember(6, Name = "mode")]
        public byte Mode { get; set; }

        [ProtoMember(7, Name = "headers")]
        public IDictionary<int, string> Headers { get; set; } = new Dictionary<int, string>();

        #endregion

        // #region MessageBody
        //
        // [ProtoMember(8, Name = "body")]
        // public byte[] Body { get;  set; } = Array.Empty<byte>();
        //
        // #endregion

        public void Copy(INetMessage message)
        {
            Id = message.Id;
            ToMessage = message.ToMessage;
            ProtocolId = message.ProtocolId;
            Code = message.ResultCode;
            Time = message.Time;
            Mode = (byte) message.Mode;
            // var headers = message.GetAllHeaders();
            // foreach (var header in headers)
            // {
            //     var key = header.HeaderKey;
            // }
        }

        public INetMessageHead ToMessageHead()
        {
            return new CommonMessageHead {
                Id = Id,
                ToMessage = ToMessage,
                ProtocolId = ProtocolId,
                Code = Code,
                Time = Time,
                Mode = (MessageMode) Mode,
            };
        }

        protected override void DoClear()
        {
            Id = 0;
            ToMessage = 0;
            ProtocolId = 0;
            Code = ResultCode.SUCCESS_CODE;
            Time = 0;
            Mode = (byte) MessageMode.Request;
            Headers.Clear();
        }

        protected bool Equals(MessageHeadProto other)
        {
            return Id == other.Id && ToMessage == other.ToMessage && ProtocolId == other.ProtocolId &&
                   Code == other.Code && Time == other.Time
                   && Mode == other.Mode
                   // && Body.IsSame(other.Body)
                   && Headers.OrderBy(kv => kv.Key).SequenceEqual(other.Headers.OrderBy(kv => kv.Key));
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((MessageHeadProto) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, ToMessage, ProtocolId, Code, Time, Mode, Headers);
        }
    }

}
