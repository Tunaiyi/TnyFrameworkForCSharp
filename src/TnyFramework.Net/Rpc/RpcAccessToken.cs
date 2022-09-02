using System;
using Newtonsoft.Json;

namespace TnyFramework.Net.Rpc
{

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

}
