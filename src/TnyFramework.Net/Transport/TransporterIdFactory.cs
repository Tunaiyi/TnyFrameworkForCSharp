using System.Threading;

namespace TnyFramework.Net.Transport
{

    public static class TransporterIdFactory
    {
        private static volatile int _TUNNEL_ID_CREATOR = 0;

        private static volatile int _TRANSPORT_ID_CREATOR = 0;

        private static volatile int _ENDPOINT_ID_CREATOR = 0;

        public static long NewEndpointId()
        {
            return Interlocked.Increment(ref _ENDPOINT_ID_CREATOR);
        }

        public static long NewTunnelId()
        {
            return Interlocked.Increment(ref _TUNNEL_ID_CREATOR);
        }

        public static long NewTransportId()
        {
            return Interlocked.Increment(ref _TRANSPORT_ID_CREATOR);
        }
    }

}
