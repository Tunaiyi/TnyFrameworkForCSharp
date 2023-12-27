// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using TnyFramework.Net.Command.Dispatcher;
using TnyFramework.Net.Command.Dispatcher.Monitor;

namespace TnyFramework.Net.Application
{

    public class NetApplication : INetApplication
    {
        private readonly IServiceProvider serviceProvider;
        private readonly IList<IServerGuide> serverGuides;
        private bool registered;

        public NetApplication(IServiceProvider serviceProvider, IEnumerable<IServerGuide> serverGuides)
        {
            this.serviceProvider = serviceProvider;
            this.serverGuides = serverGuides.ToImmutableList();
            AppContext = serviceProvider.GetRequiredService<INetAppContext>();
            Register();
        }

        public IList<INetServer> Servers => serverGuides.Select(guide => (INetServer) guide).ToList();

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
            var handlers = serviceProvider.GetServices<IRpcMonitorHandler>().ToList();
            var monitors = serviceProvider.GetServices<RpcMonitor>();
            foreach (var monitor in monitors)
            {
                monitor.AddHandlers(handlers);
            }
            registered = true;
        }
    }

}
