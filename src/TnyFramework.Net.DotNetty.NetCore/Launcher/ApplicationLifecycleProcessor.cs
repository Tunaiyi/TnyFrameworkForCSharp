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
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TnyFramework.Common.Extensions;
using TnyFramework.Common.Lifecycle;
using TnyFramework.Common.Lifecycle.Attributes;
using TnyFramework.Common.Lifecycle.Exceptions;
using TnyFramework.Common.Logger;

namespace TnyFramework.Net.DotNetty.NetCore.Launcher
{

    public class ApplicationLifecycleProcessor
    {
        private static readonly ILogger LOGGER = LogFactory.Logger<ApplicationLifecycleProcessor>();

        private static readonly Dictionary<LifecycleStage, SortedSet<Lifecycle>> LIFECYCLE_MAP =
            new Dictionary<LifecycleStage, SortedSet<Lifecycle>>();

        private static readonly Dictionary<Lifecycle, List<ILifecycleHandler>> HANDLER_MAP = new Dictionary<Lifecycle, List<ILifecycleHandler>>();

        public static void LoadHandler(IServiceProvider provider)
        {
            AddAppPrepareStarts(provider.GetServices<IAppPrepareStart>());
            AddAppPostStart(provider.GetServices<IAppPostStart>());
            AddAppClosed(provider.GetServices<IAppClosed>());
        }

        public static void AddAppPrepareStarts(params IAppPrepareStart[] handlers)
        {
            AddAppPrepareStarts((IEnumerable<IAppPrepareStart>) handlers);
        }

        public static void AddAppPrepareStarts(IEnumerable<IAppPrepareStart> handlers)
        {
            AddLifecycleHandler(LifecycleStage.PREPARE_START, h => h.GetPrepareStarter(), handlers);
        }

        public static void AddAppPostStart(params IAppPostStart[] handlers)
        {
            AddAppPostStart((IEnumerable<IAppPostStart>) handlers);
        }

        public static void AddAppPostStart(IEnumerable<IAppPostStart> handlers)
        {
            AddLifecycleHandler(LifecycleStage.POST_START, h => h.GetPostStarter(), handlers);
        }

        public static void AddAppClosed(params IAppClosed[] handlers)
        {
            AddAppClosed((IEnumerable<IAppClosed>) handlers);
        }

        public static void AddAppClosed(IEnumerable<IAppClosed> handlers)
        {
            AddLifecycleHandler(LifecycleStage.CLOSED, h => h.GetPostCloser(), handlers);
        }

        private static void AddLifecycleHandler<THandle, TLifecycle>(
            LifecycleStage<TLifecycle> lifecycleStage, Func<THandle, TLifecycle> lifecycleFunc, IEnumerable<THandle> handlers)
            where THandle : ILifecycleHandler
            where TLifecycle : Lifecycle
        {
            var lifecycles = handlers.Select(handle => {
                    var lifecycle = lifecycleFunc(handle);
                    HANDLER_MAP.ComputeIfAbsent(lifecycle, _ => new List<ILifecycleHandler>()).Add(handle);
                    return lifecycle;
                })
                .ToImmutableList();
            LIFECYCLE_MAP.ComputeIfAbsent(lifecycleStage, _ => new SortedSet<Lifecycle>())
                .AddRang(lifecycles);
        }

        private Task DoRun<THandle>(THandle handle, Action<THandle> runner, int currentIndex, bool errorContinue, bool runAsync = false)
            where THandle : ILifecycleHandler
        {
            if (runAsync)
            {
                return Task.Run(() => DoRun(handle, runner, currentIndex, errorContinue));
            }
            try
            {
                runner(handle);
                // TransactionManager.close();
            } catch (System.Exception e)
            {
                LOGGER.LogError(e, "服务生命周期 {} # 处理器 [{}] index {} | -> 异常", nameof(THandle), handle.GetType(), currentIndex);
                // TransactionManager.rollback(e);
                if (!errorContinue)
                {
                    throw new LifecycleProcessException(e);
                }
            }
            return Task.CompletedTask;
        }

        public async Task OnPrepareStart(bool errorContinue = false)
        {
            await Process<IAppPrepareStart, PrepareStarter>(
                LifecycleStage.PREPARE_START, nameof(IAppPrepareStart.OnPrepareStart), handler => handler.OnPrepareStart(), errorContinue);
        }

        public async Task OnPostStart(bool errorContinue = false)
        {
            await Process<IAppPostStart, PostStarter>(
                LifecycleStage.POST_START, nameof(IAppPostStart.OnPostStart), handler => handler.OnPostStart(), errorContinue);
        }

        public async Task OnClosed(bool errorContinue = false)
        {
            await Process<IAppClosed, PostCloser>(
                LifecycleStage.CLOSED, nameof(IAppClosed.OnClosed), handler => handler.OnClosed(), errorContinue);
        }

        private async Task Process<THandle, TLifecycle>(LifecycleStage<TLifecycle> stage, string methodName, Action<THandle> runner,
            bool errorContinue)
            where THandle : ILifecycleHandler
            where TLifecycle : Lifecycle
        {

            const string name = nameof(THandle);
            LOGGER.LogInformation("服务生命周期处理 {} ! 初始化开始......", name);

            var lifecycleList = LIFECYCLE_MAP.GetOrDefault(stage, () => new SortedSet<Lifecycle>());
            var index = 0;
            IDictionary<Lifecycle, List<ILifecycleHandler>> cloneMap = new Dictionary<Lifecycle, List<ILifecycleHandler>>();
            foreach (var pair in HANDLER_MAP)
            {
                cloneMap.Add(pair.Key, new List<ILifecycleHandler>(pair.Value));
            }
            var tasks = new List<Task>();
            var stopwatch = new Stopwatch();
            foreach (var lifecycle in lifecycleList)
            {
                var currentLifecycle = lifecycle.GetHead<TLifecycle>();
                while (currentLifecycle != null)
                {
                    var processors = cloneMap.Get(currentLifecycle);
                    if (processors != null)
                    {
                        cloneMap.Remove(currentLifecycle);
                        foreach (var processor in processors)
                        {
                            stopwatch.Restart();
                            if (!(processor is THandle handle))
                            {
                                throw new InvalidCastException($"{processor.GetType()} 无法强制为 {typeof(THandle)} class");
                            }
                            var handleType = processor.GetType();
                            var methodInfo = handleType.GetMethod(methodName);
                            var isAsync = methodInfo != null && methodInfo.GetCustomAttribute<AsyncProcessAttribute>() != null;
                            LOGGER.LogInformation("服务生命周期 {} # 处理器 [{}] index {} |", name, processor.GetType(), index);
                            if (isAsync)
                            {
                                tasks.Add(DoRun(handle, runner, index, errorContinue, true));
                            } else
                            {
                                var _ = DoRun(handle, runner, index, errorContinue);
                            }
                            stopwatch.Stop();
                            LOGGER.LogInformation("服务生命周期 {} # 处理器 [{}] index {} | -> 耗时 {} 完成",
                                name, processor.GetType(), index, stopwatch.ElapsedMilliseconds);
                            index++;
                        }
                    }
                    currentLifecycle = currentLifecycle.GetNext<TLifecycle>();
                }
            }
            foreach (var task in tasks)
            {
                await task;
            }
            LOGGER.LogInformation("服务生命周期处理 {} 完成! 共 {} 个初始化器!", name, index);
        }
    }

}
