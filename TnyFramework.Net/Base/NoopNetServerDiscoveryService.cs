using System.Threading.Tasks;
namespace TnyFramework.Net.Base
{
    public class NoopNetServerDiscoveryService : INetServerDiscoveryService
    {
        public Task RegisterInstance(INetApplication netApplication, INetServer server)
        {
            return Task.CompletedTask;
        }
        
        public Task DeregisterInstance(INetServer setting)
        {
            return Task.CompletedTask;
        }
        
    }
}
