using TnyFramework.Net.Base;

namespace TnyFramework.Net.Message
{

    internal class InnerMessager : IMessager
    {
        public long MessagerId { get; }

        public IMessagerType MessagerType { get; }

        public InnerMessager(IMessagerType messagerType, long messagerId)
        {
            MessagerType = messagerType;
            MessagerId = messagerId;
        }
    }

}
