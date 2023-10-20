// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TnyFramework.Coroutines.Async;
using Xunit;
using Xunit.Abstractions;

namespace TnyFramework.Coroutines.Test
{

    public class CoroutinesTest
    {
        private readonly ILogger logger;

        public CoroutinesTest(ITestOutputHelper output)
        {
            logger = output.BuildLogger();
        }

        [Fact]
        public async Task TestCoroutineSynchronizationContext()
        {
            CoroutineSynchronizationContext.InitializeSynchronizationContext();
            var factory = new DefaultCoroutineFactory("Actor");
            var coroutine1 = factory.Create();
            var coroutine2 = factory.Create();
            // logger.LogInformation("[Run : {CoroName} at Thread-{ThreadId}] [GOGO] 0", Coroutine.Current, Thread.CurrentThread.ManagedThreadId);
            // await coroutine1.Exec(() => DelayTest(3));
            // logger.LogInformation("[Run : {CoroName} at Thread-{ThreadId}] [GOGO] 1", Coroutine.Current, Thread.CurrentThread.ManagedThreadId);
            // await coroutine2.Exec(TestCount);
            // logger.LogInformation("[Run : {CoroName} at Thread-{ThreadId}] [GOGO] 2", Coroutine.Current, Thread.CurrentThread.ManagedThreadId);
            // await coroutine1.Exec(SendOrPostCallback);
            // logger.LogInformation("[Run : {CoroName} at Thread-{ThreadId}] [GOGO] 3", Coroutine.Current, Thread.CurrentThread.ManagedThreadId);
            await coroutine1.AsyncAction(() => Console.WriteLine("222"));
            await coroutine1.AsyncExec(() => DelayTest("单协程coroutine1测试", 1));
            await coroutine2.AsyncExec(async () => {
                logger.LogInformation("[Run : {CoroName} at Thread-{ThreadId}] [C2 CALL C1 DelayTest] Start", Coroutine.CurrentCoroutine,
                    Thread.CurrentThread.ManagedThreadId);
                try
                {
                    await coroutine1.AsyncExec(() => DelayTest("协程内启动协程", 4));
                } catch (Exception e)
                {
                    logger.LogError(e, "[Run : {CoroName} at Thread-{ThreadId}] [C2 CALL C1 DelayTest] Exception", Coroutine.CurrentCoroutine,
                        Thread.CurrentThread.ManagedThreadId);
                }
                logger.LogInformation("[Run : {CoroName} at Thread-{ThreadId}] [C2 CALL C1 DelayTest] Done", Coroutine.CurrentCoroutine,
                    Thread.CurrentThread.ManagedThreadId);
            });
            await Task.Delay(100);
            await coroutine1.Shutdown(5000);
            logger.LogInformation("[Run : {CoroName} at Thread-{ThreadId}] [GOGO] 4", Coroutine.CurrentCoroutine,
                Thread.CurrentThread.ManagedThreadId);
        }

        [Fact]
        public async Task TestTaskScheduler()
        {
            // var executor = new WorkStealingThreadPoolCoroutineExecutor(2, "TestTaskScheduler");
            var executor = SingleThreadCoroutineExecutor.Default;
            var factory = new DefaultCoroutineFactory("Actor", executor);
            var coroutine1 = factory.Create();
            var coroutine2 = factory.Create();
            logger.LogInformation("[Run : {CoroName} at Thread-{ThreadId}] [GOGO] 0", Coroutine.CurrentCoroutine,
                Thread.CurrentThread.ManagedThreadId);
            await coroutine1.AsyncAction(() => DelayTest("单协程coroutine1测试-开始启动", 3));
            logger.LogInformation("[Run : {CoroName} at Thread-{ThreadId}] [GOGO] 1", Coroutine.CurrentCoroutine,
                Thread.CurrentThread.ManagedThreadId);
            // await coroutine2.Exec(TestCount);
            // logger.LogInformation("[Run : {CoroName} at Thread-{ThreadId}] [GOGO] 2", Coroutine.Current, Thread.CurrentThread.ManagedThreadId);
            // await coroutine1.Exec(SendOrPostCallback);
            // logger.LogInformation("[Run : {CoroName} at Thread-{ThreadId}] [GOGO] 3", Coroutine.Current, Thread.CurrentThread.ManagedThreadId);
            await coroutine1.AsyncAction(() => logger.LogInformation("222"));
            await coroutine1.AsyncExec(() => DelayTest("单协程coroutine1测试-后启动", 1));
            await coroutine2.AsyncExec(async () => {
                logger.LogInformation("[Run : {CoroName} at Thread {ThreadName} ({ThreadId})] [C2 CALL C1 DelayTest] Start",
                    Coroutine.CurrentCoroutine,
                    Thread.CurrentThread.Name,
                    Thread.CurrentThread.ManagedThreadId);
                try
                {
                    await coroutine1.AsyncExec(() => DelayTest("协程内启动协程", 3));
                } catch (Exception e)
                {
                    logger.LogError(e, "[Run : {CoroName} at Thread {ThreadName} ({ThreadId})] [C2 CALL C1 DelayTest] Exception",
                        Coroutine.CurrentCoroutine,
                        Thread.CurrentThread.Name,
                        Thread.CurrentThread.ManagedThreadId);
                }
                logger.LogInformation("[Run : {CoroName} at Thread {ThreadName} ({ThreadId})] [C2 CALL C1 DelayTest] Done",
                    Coroutine.CurrentCoroutine,
                    Thread.CurrentThread.Name,
                    Thread.CurrentThread.ManagedThreadId);
            });
            await Task.Delay(10000);
            await coroutine1.Shutdown(5000);
            await coroutine2.Shutdown(5000);
            // logger.LogInformation("[Run : {CoroName} at Thread-{ThreadId}] [GOGO] 4", Coroutine.CurrentCoroutine,
            //     Thread.CurrentThread.ManagedThreadId);
        }

        private async Task DelayTest(string taskNam, int time)
        {
            var times = 0;
            while (times++ < time)
            {
                var delay = TimeSpan.FromSeconds(2);
                logger.LogInformation("[Run : {CoroName} at Thread : {ThreadName} (id:{ThreadId})] [{name}] {Times} 次 {Delay} 开始",
                    Coroutine.CurrentCoroutine,
                    Thread.CurrentThread.Name,
                    Thread.CurrentThread.ManagedThreadId,
                    taskNam,
                    times, delay);
                await Task.Delay(delay);
                logger.LogInformation("[Run : {CoroName} at Thread : {ThreadName} (id:{ThreadId})] [{name}] {Times} 次 {Delay} 结束",
                    Coroutine.CurrentCoroutine,
                    Thread.CurrentThread.Name,
                    Thread.CurrentThread.ManagedThreadId,
                    taskNam,
                    times, delay);
            }
        }
    }

}
