// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TnyFramework.Common.Logger;
using TnyFramework.Coroutines.Async;
using TnyFramework.Net.Transport;

namespace TnyFramework.Net.Session;

public class TaskResponseSourceMonitor
{
    private const long PERIOD = 100;

    private static readonly ICoroutine COROUTINE = DefaultCoroutineFactory.Default.Create("TaskResponseSourceMonitor");

    private static readonly ILogger LOGGER = LogFactory.Logger<TaskResponseSourceMonitor>();

    private static readonly ConcurrentDictionary<object, TaskResponseSourceMonitor> MONITORS_MAP =
        new ConcurrentDictionary<object, TaskResponseSourceMonitor>();

    private static volatile bool _RUNNING = true;

    private volatile bool close;

    private volatile ConcurrentDictionary<long, TaskResponseSource>? responseSources;

    static TaskResponseSourceMonitor()
    {
        COROUTINE.AsyncExec(async () => {
            while (_RUNNING)
            {
                ClearTimeoutFuture();
                await Task.Delay(TimeSpan.FromMilliseconds(PERIOD));
            }
        });
        Process.GetCurrentProcess().Exited += (_, _) => _RUNNING = false;
    }

    private ConcurrentDictionary<long, TaskResponseSource> ResponseSources {
        get {
            if (responseSources != null)
            {
                return responseSources;
            }
            lock (this)
            {

                if (responseSources != null)
                {
                    return responseSources;
                }
                responseSources = new ConcurrentDictionary<long, TaskResponseSource>();
            }
            return responseSources;
        }
    }

    public int Size => responseSources?.Count ?? 0;

    public static TaskResponseSourceMonitor LoadMonitor(object userId)
    {
        if (MONITORS_MAP.TryGetValue(userId, out var monitor))
        {
            return monitor;
        }
        return MONITORS_MAP.TryAdd(userId, monitor = new TaskResponseSourceMonitor()) ? monitor : MONITORS_MAP[userId];
    }

    public static void RemoveMonitor(object userId)
    {

        if (MONITORS_MAP.TryRemove(userId, out var monitor))
        {
            monitor.Close();
        }
    }

    private static void ClearTimeoutFuture()
    {
        foreach (var monitor in MONITORS_MAP)
        {
            try
            {
                monitor.Value.ClearTimeout();
            } catch (Exception e)
            {
                LOGGER.LogError(e, "");
            }
        }
    }

    private void ClearTimeout()
    {
        var sources = responseSources;
        if (sources == null)
        {
            return;
        }
        var now = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        foreach (var pair in sources)
        {
            try
            {
                var source = pair.Value;
                if (!source.IsTimeout(now))
                    continue;
                source.TrySetCanceled();
                sources.TryRemove(pair.Key, out _);
            } catch (Exception e)
            {
                LOGGER.LogError(e, "");
            }
        }
    }

    public void Close()
    {
        if (close)
        {
            return;
        }
        close = true;
        var sources = responseSources;
        if (sources == null)
        {
            return;
        }
        IList<TaskResponseSource> remain;
        if (!sources.IsEmpty)
        {
            remain = ImmutableList.Create<TaskResponseSource>();
        } else
        {
            remain = ImmutableList.CreateRange(sources.Values);
            sources.Clear();
        }
        foreach (var source in remain)
        {
            try
            {
                source.TrySetCanceled();
            } catch (Exception e)
            {
                LOGGER.LogError(e, "");
            }
        }
    }

    public TaskResponseSource? Get(long messageId)
    {
        var sources = responseSources;
        if (sources == null)
        {
            return null;
        }
        return sources.TryGetValue(messageId, out var source) ? source : null;
    }

    public TaskResponseSource? Poll(long messageId)
    {
        var sources = responseSources;
        if (sources == null)
        {
            return null;
        }
        return sources.TryRemove(messageId, out var source) ? source : null;
    }

    public void Put(long messageId, TaskResponseSource? source)
    {
        if (source == null)
        {
            return;
        }
        if (!close)
        {
            var sources = ResponseSources;
            sources.AddOrUpdate(messageId, source, (_, old) => {
                old.TrySetCanceled();
                return source;
            });
        } else
        {
            source.TrySetCanceled();
        }
    }
}
