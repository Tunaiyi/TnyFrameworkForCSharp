namespace TnyFramework.Net.Rpc
{

    public interface IRpcToken : IRpcServicer
    {
        IRpcServicer User { get; }

        long IssueAt { get; }
    }

}
