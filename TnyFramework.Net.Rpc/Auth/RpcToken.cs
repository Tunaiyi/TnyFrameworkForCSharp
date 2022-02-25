using System;
using Newtonsoft.Json;
namespace TnyFramework.Net.Rpc.Auth
{
    [JsonObject(MemberSerialization.OptIn)]
    public class RpcToken : RpcLinkerId, IRpcToken
    {
        public IRpcLinkerId User => _User;

        [JsonProperty("issueAt")]
        public long IssueAt { get; set; }

        [JsonProperty("user")]
        public RpcLinkerId _User { get; set; }


        public RpcToken()
        {
        }


        public RpcToken(string service, long serverId, long id, IRpcLinkerId user) : base(service, serverId, id)
        {
            if (user is RpcLinkerId value)
            {
                _User = value;
            } else
            {
                _User = new RpcLinkerId(user.Service, user.ServerId, user.Id);
            }
            IssueAt = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        }


        public override string ToString()
        {
            return
                $"{base.ToString()}, {nameof(Service)}: {Service}, {nameof(ServerId)}: {ServerId}, {nameof(Id)}: {Id}, {nameof(User)}: {User}, {nameof(IssueAt)}: {IssueAt}";
        }
    }
}
