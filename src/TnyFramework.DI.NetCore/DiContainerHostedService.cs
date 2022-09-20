using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using TnyFramework.DI.Attributes;
using TnyFramework.DI.Extensions;

namespace TnyFramework.DI.NetCore
{

    public class DiContainerHostedService : IHostedService
    {
        private readonly IServiceProvider provider;

        public DiContainerHostedService(IServiceProvider provider)
        {
            this.provider = provider;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            foreach (var type in ComponentTypeSelector.Types)
            {
                var component = type.GetCustomAttribute<ComponentAttribute>();
                if (component == null || component.Lazy)
                {
                    continue;
                }
                provider.GetService(type);
            }
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }

}
