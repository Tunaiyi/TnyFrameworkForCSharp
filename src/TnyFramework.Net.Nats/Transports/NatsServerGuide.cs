// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NATS.Client.Core;
using TnyFramework.Common.Logger;
using TnyFramework.Net.Application;
using TnyFramework.Net.Extensions;
using TnyFramework.Net.Nats.Options;
using TnyFramework.Net.Transport;

namespace TnyFramework.Net.Nats.Transports
{

    public class NatsServerGuide : NetServer<INatsServerSetting>, INatsServerGuide, INatsClientGuide
    {
        private const int INIT_STATUS = 0;
        private const int STOP_STATUS = 1;
        private const int START_STATUS = 2;

        private volatile int status = INIT_STATUS;
        private readonly IMessageCodec messageCodec;
        private readonly NatsOpts natsOpts;
        private readonly INetworkContext context;
        private readonly INatsServerSetting setting;
        private readonly INetAppContext appContext;
        private readonly CancellationTokenSource cancellation = new();
        private readonly TaskScheduler taskScheduler = TaskScheduler.Default;

        private readonly List<NatsChannel> channels = new();

        private volatile int index;

        public NatsServerGuide(NatsOpts natsOpts, INatsServerSetting setting, INetAppContext appContext,
            INetworkContext context, IMessageCodec messageCodec)
        {
            this.messageCodec = messageCodec;
            this.context = context;
            this.setting = setting;
            this.appContext = appContext;
            var url = natsOpts.Url;
            if (setting.IsHasEndPoint())
            {
                url = this.setting.Url();
            }
            this.natsOpts = new NatsOpts {
                LoggerFactory = LogFactory.DefaultFactory,
                Url = url,
                Name = this.setting.ServiceName() + "-NatsClient",
                Echo = natsOpts.Echo,
                Verbose = natsOpts.Verbose,
                Headers = natsOpts.Headers,
                AuthOpts = natsOpts.AuthOpts,
                TlsOpts = natsOpts.TlsOpts,
                SerializerRegistry = natsOpts.SerializerRegistry,
                WriterBufferSize = natsOpts.WriterBufferSize,
                ReaderBufferSize = natsOpts.ReaderBufferSize,
                UseThreadPoolCallback = natsOpts.UseThreadPoolCallback,
                InboxPrefix = natsOpts.InboxPrefix,
                NoRandomize = natsOpts.NoRandomize,
                PingInterval = natsOpts.PingInterval,
                MaxPingOut = natsOpts.MaxPingOut,
                ReconnectWaitMin = natsOpts.ReconnectWaitMin,
                ReconnectJitter = natsOpts.ReconnectJitter,
                ConnectTimeout = natsOpts.ConnectTimeout,
                ObjectPoolSize = natsOpts.ObjectPoolSize,
                RequestTimeout = natsOpts.RequestTimeout,
                CommandTimeout = natsOpts.CommandTimeout,
                SubscriptionCleanUpInterval = natsOpts.SubscriptionCleanUpInterval,
                WriterCommandBufferLimit = natsOpts.WriterCommandBufferLimit,
                HeaderEncoding = natsOpts.HeaderEncoding,
                WaitUntilSent = natsOpts.WaitUntilSent,
                MaxReconnectRetry = natsOpts.MaxReconnectRetry,
                ReconnectWaitMax = natsOpts.ReconnectWaitMax,
                IgnoreAuthErrorAbort = natsOpts.IgnoreAuthErrorAbort,
            };
        }

        public string Service => setting.ServiceName();

        public override INatsServerSetting ServiceSetting => setting;

        public override bool IsOpen()
        {
            return status == START_STATUS;
        }

        public async Task Open()
        {
            if (cancellation.IsCancellationRequested)
            {
                return;
            }
            if (Interlocked.CompareExchange(ref status, START_STATUS, INIT_STATUS) != INIT_STATUS)
            {
                return;
            }
            var rpcOptions = setting.GetRpcOptions();
            for (var i = 0; i < rpcOptions.Concurrency; i++)
            {
                var channel = new NatsChannel(setting, natsOpts, taskScheduler, messageCodec, context,
                    //Random.Shared.NextInt64(1000000, 10000000),
                    appContext.ServerId, i, cancellation.Token);
                channels.Add(channel);
                await channel.Start();
            }
        }

        public ValueTask<IClient> Client(Uri url, ConnectedHandle? handle = null)
        {
            var currentIndex = Interlocked.Increment(ref index);
            var channel = channels[currentIndex % channels.Count];
            return ValueTask.FromResult<IClient>(new NatsClient(url, channel, handle));
        }

        public Task Close()
        {
            if (cancellation.IsCancellationRequested)
            {
                return Task.CompletedTask;
            }
            Interlocked.Exchange(ref status, STOP_STATUS);
            cancellation.Cancel();
            channels.Clear();
            return Task.CompletedTask;
        }

        public bool IsClose()
        {
            return cancellation.IsCancellationRequested;
        }
    }

}
