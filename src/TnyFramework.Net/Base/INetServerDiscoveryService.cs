using System.Threading.Tasks;

namespace TnyFramework.Net.Base
{

    public interface INetServerDiscoveryService
    {
        Task RegisterInstance(INetApplication netApplication, INetServer server);

        Task DeregisterInstance(INetServer setting);
    }

}
