using TnyFramework.Net.Base;

namespace TnyFramework.Net.Message
{

    public class InnerMessagerFactory : IMessagerFactory
    {
        public IMessager CreateMessager(IMessagerType type, long messagerId)
        {
            return new InnerMessager(type, messagerId);
        }

        public IMessager CreateMessager(ForwardMessager messager)
        {
            return messager;
        }
    }

}
