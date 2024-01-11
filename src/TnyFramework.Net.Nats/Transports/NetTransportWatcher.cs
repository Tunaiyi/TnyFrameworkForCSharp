// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TnyFramework.Common.Logger;
using TnyFramework.Common.Util;

namespace TnyFramework.Net.Nats.Transports
{

    public delegate void WatchHandle(TransportWatch watch);

    public class NetTransportWatcher : IAsyncDisposable
    {
        private readonly ConcurrentDictionary<string, TransportWatch> transports = new();

        private readonly WatchHandle handler;

        private readonly int loopInterval;

        private readonly CancellationTokenSource cancellation = new();

        private readonly Task watching;

        private readonly ILogger logger;

        public NetTransportWatcher(WatchHandle handler, int loopInterval)
        {
            logger = LogFactory.Logger<NetTransportWatcher>();
            this.handler = handler;
            this.loopInterval = loopInterval;
            watching = Task.Run(WatchLoop, cancellation.Token);
        }

        public bool Watch(NatsTransport transport, int timeoutMills)
        {
            if (cancellation.IsCancellationRequested)
            {
                return false;
            }
            var watch = new TransportWatch(transport, timeoutMills);
            return transports.TryAdd(watch.Key, watch);
        }

        public bool Complete(string key, out NatsTransport transport)
        {
            if (!transports.TryGetValue(key, out var watch))
            {
                transport = null!;
                return false;
            }
            if (TryDone(watch, TryComplete, 0))
            {
                transport = watch.Transport;
                return true;
            }
            transport = default!;
            return false;
        }

        private async Task WatchLoop()
        {
            var nowTick = Ticks.Tick();
            while (!cancellation.IsCancellationRequested)
            {
                foreach (var pair in transports)
                {
                    var watch = pair.Value;
                    if (!watch.Running)
                    {
                        transports.TryRemove(pair.Key, out _);
                        continue;
                    }
                    TryDone(watch, TryTimeout, nowTick);
                }
                await Task.Delay(loopInterval);
            }
            foreach (var watch in transports.Values)
            {
                TryDone(watch, TryCancel, 0);
            }
        }

        private bool TryComplete(TransportWatch watch, int unused)
        {
            return watch.Complete();
        }

        private bool TryCancel(TransportWatch watch, int unused)
        {
            return watch.Cancel();
        }

        private bool TryTimeout(TransportWatch watch, long now)
        {
            return watch.Timeout(now);
        }

        private bool TryDone<T>(TransportWatch watch, Func<TransportWatch, T, bool> done, T state)
        {
            if (!done(watch, state))
            {
                return false;
            }
            FireHandle(watch);
            transports.TryRemove(watch.Key, out _);
            return true;
        }

        private void FireHandle(TransportWatch watch)
        {
            try
            {
                handler(watch);
            } catch (Exception e)
            {
                logger.LogError(e, "{transport} is timeout exception", watch.Transport.RemoteAccessKey);
            }
        }

        public async ValueTask DisposeAsync()
        {
            cancellation.Cancel();
            await watching;
            cancellation.Dispose();
        }
    }

    public class TransportWatch
    {
        private readonly long timeoutTick;

        private volatile int status;

        public TransportWatch(NatsTransport transport, long delay)
        {
            Transport = transport;
            timeoutTick = Ticks.Tick() + Ticks.ToTick(delay);
        }

        public TransportWatchStatus Status {
            get => (TransportWatchStatus) status;
            set => status = (int) value;
        }

        public NatsTransport Transport { get; }

        internal string Key => Transport.RemoteAccessKey;

        internal bool Running => status == (int) TransportWatchStatus.Running;

        internal bool Completed => status == (int) TransportWatchStatus.Complete;

        internal bool Canceled => status == (int) TransportWatchStatus.Cancel;

        internal bool Complete()
        {
            return Done(TransportWatchStatus.Complete, 0);
        }

        internal bool Cancel()
        {
            return Done(TransportWatchStatus.Cancel, 0);
        }

        internal bool Timeout(long nowTick)
        {
            return Done(TransportWatchStatus.Timeout, nowTick, IsTimeout);
        }

        private bool IsTimeout(long nowTick)
        {
            return nowTick >= timeoutTick;
        }

        private bool Done<T>(TransportWatchStatus updateStatus, T state, Func<T, bool>? check = null)
        {
            var update = (int) updateStatus;
            while (true)
            {
                var current = status;
                if (current != (int) TransportWatchStatus.Running)
                {
                    return false;
                }
                if (check != null && !check(state))
                {
                    return false;
                }
                if (Interlocked.CompareExchange(ref status, update, current) == current)
                {
                    return true;
                }
            }
        }
    }

}
