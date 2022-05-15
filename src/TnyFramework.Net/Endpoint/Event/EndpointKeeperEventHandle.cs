namespace TnyFramework.Net.Endpoint.Event
{

    /// <summary>
    /// 添加终端
    /// </summary>
    public delegate void EndpointKeeperAddEndpoint(IEndpointKeeper keeper, IEndpoint endpoint);

    /// <summary>
    /// 移除终端
    /// </summary>
    public delegate void EndpointKeeperRemoveEndpoint(IEndpointKeeper keeper, IEndpoint endpoint);

}
