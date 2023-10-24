// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SkyApm;
using SkyApm.Config;
using SkyApm.Diagnostics;
using SkyApm.Diagnostics.MSLogging;
using SkyApm.Logging;
using SkyApm.Sampling;
using SkyApm.Service;
using SkyApm.Tracing;
using SkyApm.Transport;
using SkyApm.Transport.Grpc;
using SkyApm.Utilities.Configuration;
using SkyApm.Utilities.DependencyInjection;
using SkyApm.Utilities.Logging;
using TnyFramework.Net.Apm.Skywalking.NetCore.Hosting;

namespace TnyFramework.Net.Apm.Skywalking.NetCore.Configurations
{

    public static class TnyRpcServiceCollectionExtensions
    {
        /**
         * 注入 TnyRpcSkyApm 所需要的服务
         */
        public static IServiceCollection AddTnyRpcSkyApm(this IServiceCollection services, Action<SkyApmExtensions>? extensionsSetup = null)
        {
            services.AddTnyRpcSkyApmCore(extensionsSetup);
            return services;
        }

        private static IServiceCollection AddTnyRpcSkyApmCore(this IServiceCollection services, Action<SkyApmExtensions>? extensionsSetup = null)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddSingleton<ISegmentDispatcher, AsyncQueueSegmentDispatcher>();
            services.AddSingleton<IExecutionService, RegisterService>();
            services.AddSingleton<IExecutionService, LogReportService>();
            services.AddSingleton<IExecutionService, PingService>();
            services.AddSingleton<IExecutionService, SegmentReportService>();
            services.AddSingleton<IExecutionService, CLRStatsService>();
            services.AddSingleton<IInstrumentStartup, InstrumentStartup>();
            services.AddSingleton(RuntimeEnvironment.Instance);
            services.AddSingleton<TracingDiagnosticProcessorObserver>();
            services.AddSingleton<IConfigAccessor, ConfigAccessor>();
            services.AddSingleton<IConfigurationFactory, ConfigurationFactory>();
            services.AddSingleton<IHostedService, InstrumentationHostedService>();
            services.AddSingleton<IEnvironmentProvider, HostingEnvironmentProvider>();
            services.AddSingleton<ISkyApmLogDispatcher, AsyncQueueSkyApmLogDispatcher>();
            services.AddTracing().AddSampling().AddGrpcTransport().AddSkyApmLogging();
            var extensions = services.AddSkyApmExtensions()
                .AddMSLogging();
            extensionsSetup?.Invoke(extensions);
            return services;
        }

        private static IServiceCollection AddTracing(this IServiceCollection services)
        {
            services.AddSingleton<ITracingContext, TracingContext>();
            services.AddSingleton<ICarrierPropagator, CarrierPropagator>();
            services.AddSingleton<ICarrierFormatter, Sw8CarrierFormatter>();
            services.AddSingleton<ISegmentContextFactory, SegmentContextFactory>();
            services.AddSingleton<IEntrySegmentContextAccessor, EntrySegmentContextAccessor>();
            services.AddSingleton<ILocalSegmentContextAccessor, LocalSegmentContextAccessor>();
            services.AddSingleton<IExitSegmentContextAccessor, ExitSegmentContextAccessor>();
            services.AddSingleton<ISegmentContextAccessor, SegmentContextAccessor>();
            services.AddSingleton<ISamplerChainBuilder, SamplerChainBuilder>();
            services.AddSingleton<IUniqueIdGenerator, UniqueIdGenerator>();
            services.AddSingleton<ISegmentContextMapper, SegmentContextMapper>();
            services.AddSingleton<IBase64Formatter, Base64Formatter>();
            return services;
        }

        private static IServiceCollection AddSampling(this IServiceCollection services)
        {
            services.AddSingleton<SimpleCountSamplingInterceptor>();
            services.AddSingleton<ISamplingInterceptor>(p => p.GetService<SimpleCountSamplingInterceptor>()!);
            services.AddSingleton<IExecutionService>(p => p.GetService<SimpleCountSamplingInterceptor>()!);
            services.AddSingleton<ISamplingInterceptor, RandomSamplingInterceptor>();
            services.AddSingleton<ISamplingInterceptor, IgnorePathSamplingInterceptor>();
            return services;
        }

        private static IServiceCollection AddGrpcTransport(this IServiceCollection services)
        {
            services.AddSingleton<ISegmentReporter, SegmentReporter>();
            services.AddSingleton<ILoggerReporter, LoggerReporter>();
            services.AddSingleton<ICLRStatsReporter, CLRStatsReporter>();
            services.AddSingleton<ConnectionManager>();
            services.AddSingleton<IPingCaller, PingCaller>();
            services.AddSingleton<IServiceRegister, ServiceRegister>();
            services.AddSingleton<IExecutionService, ConnectService>();
            return services;
        }

        private static IServiceCollection AddSkyApmLogging(this IServiceCollection services)
        {
            services.AddSingleton<ILoggerFactory, DefaultLoggerFactory>();
            return services;
        }
    }

}
