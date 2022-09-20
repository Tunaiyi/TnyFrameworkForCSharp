// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using ProtoBuf;
using TnyFramework.Net.Base;
using MessagerTypes = TnyFramework.Net.Base.MessagerType;

namespace TnyFramework.Net.Message
{

    [ProtoContract]
    public class ForwardMessager : IMessager
    {
        private int messagerTypeId;

        [ProtoMember(1)]
        public long MessagerId { get; set; }

        [ProtoMember(2)]
        public int MessagerTypeId {
            get => messagerTypeId;
            set {
                messagerTypeId = value;
                MessagerType = MessagerTypes.ForId(messagerTypeId);
            }
        }

        [ProtoIgnore]
        public IMessagerType MessagerType { get; private set; }

        public ForwardMessager()
        {
        }

        public ForwardMessager(IMessager messager)
        {
            MessagerId = messager.MessagerId;
            MessagerType = messager.MessagerType;
            messagerTypeId = MessagerType.Id;
        }
    }

}
