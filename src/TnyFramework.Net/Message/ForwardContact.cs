// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using ProtoBuf;
using TnyFramework.Net.Application;

namespace TnyFramework.Net.Message;

[ProtoContract]
public class ForwardContact : IContact
{
    private int contactTypeId;

    [ProtoMember(1)]
    public long ContactId { get; set; }

    [ProtoMember(2)]
    public int ContactTypeId {
        get => contactTypeId;
        set {
            contactTypeId = value;
            ContactType = Application.ContactType.ForId(contactTypeId);
        }
    }

    [ProtoIgnore]
    public IContactType ContactType { get; private set; }

    public ForwardContact()
    {
        ContactType = null!;
    }

    public ForwardContact(IContact contact)
    {
        ContactId = contact.ContactId;
        ContactType = contact.ContactType;
        contactTypeId = ContactType.Id;
    }
}
