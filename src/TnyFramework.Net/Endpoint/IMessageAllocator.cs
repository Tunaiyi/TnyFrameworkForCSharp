using TnyFramework.Net.Message;
using TnyFramework.Net.Transport;
namespace TnyFramework.Net.Endpoint
{
    public delegate IMessage MessageAllocator(IMessageFactory factory, MessageContext context);

    public interface IMessageAllocator
    {
        IMessage Allocate(IMessageFactory factory, MessageContext context);
    }
}
