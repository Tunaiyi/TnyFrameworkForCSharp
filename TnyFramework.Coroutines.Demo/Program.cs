using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TnyFramework.Common.Invoke;
using TnyFramework.Common.Logger;
using TnyFramework.Coroutines.Async;
namespace TnyFramework.Coroutines.Demo
{
    internal class Program
    {
        private static readonly ILogger LOGGER = LogFactory.Logger<Program>();

        private class Player
        {
            private string Name = "Tom";


            public string Say(String content)
            {
                return $"{Name} say : {content}";
            }
        }


        private static async Task Main(string[] args)
        {

            const int maxTime = 1000000;
            var delegateType = typeof(Func<,,>).MakeGenericType(typeof(Player), typeof(string), typeof(string));
            var player = new Player();

            //创建LabelTarget用来返回值 (invoker, content) => invoker.Say((string)content)
            // var invoker = Expression.Parameter(typeof(Player), "invoker");
            // var content = Expression.Parameter(typeof(object), "content");
            // var contentCast = Expression.Convert(content, typeof(string));
            // var callSay = Expression.Call(invoker, say, contentCast);
            // var exp = Expression.Lambda<Func<Player, object, object>>(callSay, invoker, content).Compile();

            var watch = Stopwatch.StartNew();
            var times = 0;

            var say = typeof(Player).GetMethod("Say");
            if (say == null)
                return;

            watch.Restart();
            times = 0;
            while (times++ < maxTime)
            {
                say.Invoke(player, new object[] { "abc" });
            }
            watch.Stop();
            Console.WriteLine("sayMethod.Invoke : " + watch.ElapsedMilliseconds);


            var func = Delegate.CreateDelegate(delegateType, say);
            watch.Restart();
            times = 0;
            while (times++ < maxTime)
            {
                func.DynamicInvoke(player, "abc");
            }
            watch.Stop();
            Console.WriteLine("func.DynamicInvoke : " + watch.ElapsedMilliseconds);


            watch.Restart();
            times = 0;
            while (times++ < maxTime)
            {
                func.Method.Invoke(player, new object[] { "abc" });

            }
            watch.Stop();
            Console.WriteLine("func.Method.Invoke : " + watch.ElapsedMilliseconds);

            say = typeof(Player).GetMethod("Say");
            if (say == null)
                return;
            var exp = new FastInvokerFactory<Player, object, object>().Create(say);

            watch.Restart();
            times = 0;
            while (times++ < maxTime)
            {
                // exp.DynamicInvoke(player, "abc");
                exp.Invoke(player, "abc");
            }
            watch.Stop();
            Console.WriteLine("exp.DynamicInvoke : " + watch.ElapsedMilliseconds + " content " + exp.Invoke(player, "abc"));


            watch.Restart();
            times = 0;
            while (times++ < maxTime)
            {
                player.Say("abc");
            }
            watch.Stop();
            Console.WriteLine("player.say : " + watch.ElapsedMilliseconds);

            // watch.Restart();
            // times = 0;
            // while (times++ < maxTime)
            // {
            //     exp.Method.Invoke(player, new object[] { "abc" });
            // }
            // watch.Stop();
            // Console.WriteLine("exp.Method.Invoke : " + watch.ElapsedMilliseconds);


            CoroutineSynchronizationContext.InitializeSynchronizationContext();
            var factory = new DefaultCoroutineFactory("Actor");
            var coroutine1 = factory.Create();
            var coroutine2 = factory.Create();
            // LOGGER.LogInformation("[Run : {} at Thread-{}] [GOGO] 0", Coroutine.Current, Thread.CurrentThread.ManagedThreadId);
            // await coroutine1.Exec(() => DelayTest(3));
            // LOGGER.LogInformation("[Run : {} at Thread-{}] [GOGO] 1", Coroutine.Current, Thread.CurrentThread.ManagedThreadId);
            // await coroutine2.Exec(TestCount);
            // LOGGER.LogInformation("[Run : {} at Thread-{}] [GOGO] 2", Coroutine.Current, Thread.CurrentThread.ManagedThreadId);
            // await coroutine1.Exec(SendOrPostCallback);
            // LOGGER.LogInformation("[Run : {} at Thread-{}] [GOGO] 3", Coroutine.Current, Thread.CurrentThread.ManagedThreadId);
            await coroutine1.Run(() => Console.WriteLine("222"));
            await coroutine1.Exec(() => DelayTest(1));
            await coroutine2.Exec(async () => {
                LOGGER.LogInformation("[Run : {} at Thread-{}] [C2 CALL C1 DelayTest] Start", Coroutine.Current,
                    Thread.CurrentThread.ManagedThreadId);
                try
                {
                    await coroutine1.Exec(() => DelayTest(4));
                } catch (System.Exception e)
                {
                    LOGGER.LogError(e, "[Run : {} at Thread-{}] [C2 CALL C1 DelayTest] Exception", Coroutine.Current,
                        Thread.CurrentThread.ManagedThreadId);
                }
                LOGGER.LogInformation("[Run : {} at Thread-{}] [C2 CALL C1 DelayTest] Done", Coroutine.Current,
                    Thread.CurrentThread.ManagedThreadId);
            });
            await Task.Delay(100);
            await coroutine1.Shutdown(5000);
            LOGGER.LogInformation("[Run : {} at Thread-{}] [GOGO] 4", Coroutine.Current, Thread.CurrentThread.ManagedThreadId);
            Console.ReadKey();
        }


        private static async Task SendOrPostCallback()
        {
            var source = new TaskCompletionSource<int>();
            await Task.Run(() => {
                Thread.Sleep((int)2000L);
                LOGGER.LogInformation("[Run : {} at Thread-{}] [SendOrPostCallback] TaskCompletionSource SetResult", Coroutine.Current,
                    Thread.CurrentThread.ManagedThreadId);
                source.SetResult(1);
            });
            LOGGER.LogInformation("[Run : {} at Thread-{}] [SendOrPostCallback] Start", Coroutine.Current,
                Thread.CurrentThread.ManagedThreadId);
            await source.Task;
            LOGGER.LogInformation("[Run : {} at Thread-{}] [SendOrPostCallback] Done!!!", Coroutine.Current,
                Thread.CurrentThread.ManagedThreadId);
        }


        private static async Task DelayTest(int time)
        {
            var times = 0;
            while (times++ < time)
            {
                var delay = TimeSpan.FromSeconds(3);
                LOGGER.LogInformation("[Run : {} at Thread-{}] [DelayTest] {} 次 {} 开始", Coroutine.Current,
                    Thread.CurrentThread.ManagedThreadId,
                    times, delay);
                await Task.Delay(delay);
                LOGGER.LogInformation("[Run : {} at Thread-{}] [DelayTest] {} 次 {} 结束", Coroutine.Current,
                    Thread.CurrentThread.ManagedThreadId,
                    times, delay);
            }
        }


        private static async Task TestCount()
        {
            var index = 0;
            while (index < 3)
            {
                LOGGER.LogInformation("[Run : {} at Thread-{}] [TestCount] INDEX = {}",
                    Coroutine.Current, Thread.CurrentThread.ManagedThreadId, index);
                index++;
                LOGGER.LogInformation("[Run : {} at Thread-{}] [TestCount] Yield before",
                    Coroutine.Current, Thread.CurrentThread.ManagedThreadId, index);
                await Task.Yield();
                LOGGER.LogInformation("[Run : {} at Thread-{}] [TestCount] Yielded",
                    Coroutine.Current, Thread.CurrentThread.ManagedThreadId, index);
            }
        }
    }
}
