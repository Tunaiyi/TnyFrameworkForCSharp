namespace TnyFramework.Net.Transport.Event
{

    public delegate void TunnelActivate(ITunnel tunnel);

    public delegate void TunnelUnactivated(ITunnel tunnel);

    public delegate void TunnelClose(ITunnel tunnel);

}
