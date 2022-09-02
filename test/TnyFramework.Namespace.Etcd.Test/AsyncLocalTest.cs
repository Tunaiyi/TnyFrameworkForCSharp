using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using TnyFramework.Common.Logger;
using TnyFramework.Coroutines.Async;

namespace TnyFramework.Namespace.Etcd.Test
{

    public class AsyncLocalTest
    {
        private static readonly ILogger LOGGER = LogFactory.Logger<AsyncLocalTest>();

        [Test]
        public async Task TestAsyncLocal()
        {
            var local = new AsyncLocal<string>();
            var coroutine = DefaultCoroutineFactory.Default.Create();

            var task1 = coroutine.AsyncExec(async () => {
                local.Value = "ALocal-1";
                await Task.Delay(100);
                LOGGER.LogInformation("T {}({}) | ACoro {}", Thread.CurrentThread.Name, Thread.CurrentThread.ManagedThreadId, local.Value);
                local.Value = "ALocal-2";
                await Task.Delay(100);
                LOGGER.LogInformation("T {}({}) | ACoro {}", Thread.CurrentThread.Name, Thread.CurrentThread.ManagedThreadId, local.Value);
            });

            var task2 = coroutine.AsyncExec(async () => {
                local.Value = "BLocal-1";
                await Task.Delay(100);
                LOGGER.LogInformation("T {}({}) | BCoro {}", Thread.CurrentThread.Name, Thread.CurrentThread.ManagedThreadId, local.Value);
                local.Value = "BLocal-2";
                await Task.Delay(100);
                LOGGER.LogInformation("T {}({}) | BCoro {}", Thread.CurrentThread.Name, Thread.CurrentThread.ManagedThreadId, local.Value);
            });
            await task1;
            await task2;

        }
    }

}
