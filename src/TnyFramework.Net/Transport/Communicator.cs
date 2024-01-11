// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System.Net;
using TnyFramework.Common.Attribute;
using TnyFramework.Net.Application;

namespace TnyFramework.Net.Transport;

public abstract class Communicator : AttributesContext, ICommunicator
{
    public abstract ICertificate Certificate { get; }

    public long Identify => Certificate.Identify;

    public object? IdentifyToken => Certificate.IdentifyToken;

    public long ContactId => Certificate.ContactId;

    public IContactType ContactType => Certificate.ContactType;

    public string ContactGroup => Certificate.ContactGroup;

    public ICertificate GetCertificate()
    {
        return Certificate;
    }

    public abstract EndPoint? RemoteAddress { get; }

    public abstract EndPoint? LocalAddress { get; }

    public abstract NetAccessMode AccessMode { get; }

    public abstract bool IsActive();

    public abstract bool IsClosed();

    public abstract bool Close();
}
