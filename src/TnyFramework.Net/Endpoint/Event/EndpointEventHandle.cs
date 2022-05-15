namespace TnyFramework.Net.Endpoint.Event
{

    /// <summary>
    /// 终端上线
    /// </summary>
    public delegate void EndpointOnline(IEndpoint endpoint);

    /// <summary>
    /// 终端下线
    /// </summary>
    public delegate void EndpointOffline(IEndpoint endpoint);

    /// <summary>
    /// 终端关闭
    /// </summary>
    public delegate void EndpointClose(IEndpoint endpoint);

}
