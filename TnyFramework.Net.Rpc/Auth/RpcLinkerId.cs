using System;
using Newtonsoft.Json;
namespace TnyFramework.Net.Rpc.Auth
{
    public interface IRpcLinkerId : IComparable<IRpcLinkerId>
    {
        string Service { get; }
        long ServerId { get; }
        long Id { get; }
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class RpcLinkerId : IRpcLinkerId
    {
        [JsonProperty("service")]
        public string Service { get; set; }

        [JsonProperty("serverId")]
        public long ServerId { get; set; }

        [JsonProperty("id")]
        public long Id { get; set; }


        public RpcLinkerId()
        {
        }


        public RpcLinkerId(string service, long serverId, long id)
        {
            Service = service;
            ServerId = serverId;
            Id = id;
        }


        public int CompareTo(IRpcLinkerId other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            var serviceComparison = string.Compare(Service, other.Service, StringComparison.Ordinal);
            if (serviceComparison != 0) return serviceComparison;
            var serverIdComparison = ServerId.CompareTo(other.ServerId);
            if (serverIdComparison != 0) return serverIdComparison;
            return Id.CompareTo(other.Id);
        }


        public override string ToString()
        {
            return $"{nameof(Service)}: {Service}, {nameof(ServerId)}: {ServerId}, {nameof(Id)}: {Id}";
        }
    }
}
