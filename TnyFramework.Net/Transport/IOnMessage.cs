using System.Threading.Tasks;
using TnyFramework.Net.Message;
namespace TnyFramework.Net.Transport
{
    public interface IOnMessage
    {
        Task OnMessage(INetTunnel tunnel, IMessage message);
    }
}
