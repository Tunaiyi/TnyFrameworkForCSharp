// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TnyFramework.Coroutines.Async;
using TnyFramework.Net.Base;
using TnyFramework.Net.DotNetty.NetCore.Launcher;

namespace TnyFramework.Net.DotNetty.NetCore
{

    public class NetHostedService : IHostedService
    {
        private readonly ILogger<NetHostedService> logger;

        private readonly INetApplication application;

        private readonly INetServerDiscoveryService serverDiscoveryService;

        private readonly ICoroutine coroutine;

        private readonly ApplicationLifecycleProcessor lifecycleProcessor;

        public NetHostedService(INetApplication application,
            INetServerDiscoveryService serverDiscoveryService,
            IServiceProvider provider,
            ILogger<NetHostedService> logger)
        {
            this.logger = logger;
            this.application = application;
            this.serverDiscoveryService = serverDiscoveryService;
            ApplicationLifecycleProcessor.LoadHandler(provider);
            lifecycleProcessor = new ApplicationLifecycleProcessor();
            coroutine = DefaultCoroutineFactory.Default.Create("NetHostedService");
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await lifecycleProcessor.OnPrepareStart();
            var context = application.AppContext;
            logger.LogInformation("{AppName}(s{ServerId})[{AppType}|{ScopeType}] starting", context.Name, context.ServerId, context.AppType,
                context.ScopeType);
            await application.Start();
            logger.LogInformation("{AppName}(s{ServerId})[{AppType}|{ScopeType}] started", context.Name, context.ServerId, context.AppType,
                context.ScopeType);
            await lifecycleProcessor.OnPostStart();
            IList<Task> tasks = new List<Task>();
            foreach (var server in application.Servers)
            {
                tasks.Add(coroutine.AsyncExec(handle: async () => {
                    while (true)
                    {
                        try
                        {
                            logger.LogInformation("[{Ins}] RegisterInstance", server.Name);
                            await serverDiscoveryService.RegisterInstance(application, server);
                            logger.LogInformation("[{Ins}] RegisterInstance success", server.Name);
                            break;
                        } catch (System.Exception e)
                        {
                            logger.LogError(e, "{Ins} RegisterInstance exception", server.Name);
                            await Task.Delay(3000, cancellationToken);
                        }
                    }
                }));
            }
            await Task.WhenAll(tasks.ToArray());
            logger.LogInformation("{AppName}(s{ServerId})[{AppType}|{ScopeType}] RegisterInstance []",
                context.Name, context.ServerId, context.AppType, context.ScopeType);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            var appContext = application.AppContext;
            logger.LogInformation("{AppName}(s{ServerId})[{AppType}|{ScopeType}] closing", appContext.Name, appContext.ServerId, appContext.AppType,
                appContext.ScopeType);
            await application.Close();
            logger.LogInformation("{AppName}(s{ServerId})[{AppType}|{ScopeType}] closed", appContext.Name, appContext.ServerId, appContext.AppType,
                appContext.ScopeType);
            foreach (var server in application.Servers)
            {

                try
                {
                    logger.LogInformation("[{Ins}] DeregisterInstance", server.Name);
                    await serverDiscoveryService.DeregisterInstance(server);
                    logger.LogInformation("[{Ins}] DeregisterInstance success", server.Name);
                } catch (System.Exception e)
                {
                    logger.LogError(e, "{Ins} DeregisterInstance exception", server.Name);
                }

            }
            logger.LogInformation("{AppName}(s{ServerId})[{AppType}|{ScopeType}] DeregisterInstance",
                appContext.Name, appContext.ServerId, appContext.AppType, appContext.ScopeType);
            await application.Close();
            await lifecycleProcessor.OnClosed();
        }
    }

}
