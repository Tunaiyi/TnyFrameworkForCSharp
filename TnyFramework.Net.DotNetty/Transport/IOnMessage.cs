using System.Threading.Tasks;
using TnyFramework.Net.DotNetty.Message;
namespace TnyFramework.Net.DotNetty.Transport
{
    public interface IOnMessage
    {
        Task OnMessage(INetTunnel tunnel, IMessage message);
    }
}
