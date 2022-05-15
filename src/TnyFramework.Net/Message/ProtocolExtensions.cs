namespace TnyFramework.Net.Message
{

    public static class ProtocolExtensions
    {
        public static IProtocol Protocol(this int id)
        {
            return Protocols.Protocol(id);
        }

        public static IProtocol Protocol(this int id, int line)
        {
            return Protocols.Protocol(id, line);
        }
    }

}
