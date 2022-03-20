using System;
namespace TnyFramework.Net.Rpc.Auth
{
    public interface IRpcToken : IRpcLinkerId
    {
        IRpcLinkerId User { get; }

        long IssueAt { get; }
    }
}
