using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using TnyFramework.Net.Base;
using TnyFramework.Net.Dispatcher;
using TnyFramework.Net.DotNetty.Bootstrap;
namespace TnyFramework.Net.DotNetty.Configuration.Guide
{
    public class NetApplication : INetApplication
    {
        private readonly IServiceProvider serviceProvider;
        private IList<INettyServerGuide> serverGuides;
        private bool registered;


        public NetApplication(IServiceProvider serviceProvider, IEnumerable<INettyServerGuide> serverGuides)
        {
            this.serviceProvider = serviceProvider;
            this.serverGuides = serverGuides.ToImmutableList();
            AppContext = serviceProvider.GetRequiredService<INetAppContext>();
            Register();
        }


        public IList<INetServer> Servers => serverGuides.Select(guide => (INetServer)guide).ToList();

        public INetAppContext AppContext { get; }


        public async Task Start()
        {
            foreach (var guide in serverGuides)
            {
                await guide.Open();
            }
        }


        public async Task Close()
        {
            foreach (var guide in serverGuides)
            {
                await guide.Close();
            }
        }


        private void Register()
        {
            if (registered)
                return;
            var controllers = serviceProvider.GetServices<IController>();
            var dispatcher = serviceProvider.GetService<IMessageDispatcher>();
            if (dispatcher == null)
            {
                throw new NullReferenceException("MessageDispatcher is null");
            }
            foreach (var controller in controllers)
            {
                dispatcher.AddController(controller);
            }
            registered = true;
        }
    }
}
