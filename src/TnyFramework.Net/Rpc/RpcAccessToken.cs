// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using Newtonsoft.Json;

namespace TnyFramework.Net.Rpc;

[JsonObject(MemberSerialization.OptIn)]
public class RpcAccessToken : RpcAccessIdentify
{
    public IRpcServicer User => UserIdentify;

    [JsonProperty("issueAt")]
    public long IssueAt { get; set; }

    [JsonProperty("user")]
    public RpcAccessIdentify UserIdentify { get; set; }

    public RpcAccessToken()
    {
        UserIdentify = null!;
    }

    public RpcAccessToken(RpcServiceType serviceType, int serverId, RpcAccessIdentify user) : base(serviceType, serverId, user.Index)
    {
        UserIdentify = user;
        IssueAt = DateTimeOffset.Now.ToUnixTimeMilliseconds();
    }

    public override string ToString()
    {
        return
            $"{base.ToString()}, {nameof(ServiceType)}: {ServiceType}, {nameof(ServerId)}: {ServerId}, {nameof(Id)}: {Id}, {nameof(User)}: {User}, {nameof(IssueAt)}: {IssueAt}";
    }
}
