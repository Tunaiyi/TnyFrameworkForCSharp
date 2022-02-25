using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TnyFramework.Coroutines.Async;
using TnyFramework.Net.Base;
namespace TnyFramework.Net.DotNetty.AspNetCore
{
    public class NetHostedService : IHostedService
    {
        private readonly ILogger<NetHostedService> logger;

        private readonly INetApplication application;

        private readonly INetServerDiscoveryService serverDiscoveryService;

        private readonly ICoroutine coroutine;


        public NetHostedService(INetApplication application,
            INetServerDiscoveryService serverDiscoveryService,
            ILogger<NetHostedService> logger)
        {
            this.logger = logger;
            this.application = application;
            this.serverDiscoveryService = serverDiscoveryService;
            coroutine = DefaultCoroutineFactory.Default.Create("NetHostedService");
        }


        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var appContext = application.AppContext;
            logger.LogInformation("{}(s{})[{}|{}] starting", appContext.Name, appContext.ServerId, appContext.AppType, appContext.ScopeType);
            await application.Start();
            logger.LogInformation("{}(s{})[{}|{}] started", appContext.Name, appContext.ServerId, appContext.AppType, appContext.ScopeType);
            IList<Task> tasks = new List<Task>();
            foreach (var server in application.Servers)
            {
                tasks.Add(coroutine.Exec(action: async () => {
                    while (true)
                    {
                        try
                        {
                            logger.LogInformation("[{}] RegisterInstance", server.Name);
                            await serverDiscoveryService.RegisterInstance(application, server);
                            logger.LogInformation("[{}] RegisterInstance success", server.Name);
                            break;
                        } catch (System.Exception e)
                        {
                            logger.LogError(e, "{} RegisterInstance exception", server.Name);
                            await Task.Delay(3000, cancellationToken);
                        }
                    }
                }));
            }
            await Task.WhenAll(tasks.ToArray());
            logger.LogInformation("{}(s{})[{}|{}] RegisterInstance []",
                appContext.Name, appContext.ServerId, appContext.AppType, appContext.ScopeType);
        }


        public async Task StopAsync(CancellationToken cancellationToken)
        {
            var appContext = application.AppContext;
            logger.LogInformation("{}(s{})[{}|{}] closing", appContext.Name, appContext.ServerId, appContext.AppType, appContext.ScopeType);
            await application.Close();
            logger.LogInformation("{}(s{})[{}|{}] closed", appContext.Name, appContext.ServerId, appContext.AppType, appContext.ScopeType);
            foreach (var server in application.Servers)
            {
                try
                {
                    logger.LogInformation("[{}] DeregisterInstance", server.Name);
                    await serverDiscoveryService.DeregisterInstance(server);
                    logger.LogInformation("[{}] DeregisterInstance success", server.Name);
                } catch (System.Exception e)
                {
                    logger.LogError(e, "{} DeregisterInstance exception", server.Name);
                }

            }
            logger.LogInformation("{}(s{})[{}|{}] DeregisterInstance",
                appContext.Name, appContext.ServerId, appContext.AppType, appContext.ScopeType);
        }
    }
}
