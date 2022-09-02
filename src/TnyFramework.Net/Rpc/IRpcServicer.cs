namespace TnyFramework.Net.Rpc
{

    public interface IRpcServicer
    {
        int ServerId { get; }

        IRpcServiceType ServiceType { get; }
        
    }

}
