#region

using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TnyFramework.Common.Event;
using TnyFramework.Common.Invoke;
using TnyFramework.Common.Logger;
using TnyFramework.Coroutines.Async;

#endregion

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


        private class CPlayer
        {
            private string Name { get; }

            private int Age { get; }


            public CPlayer(string name, int age)
            {
                Name = name;
                Age = age;
            }


            public string Say(string content)
            {
                return $"{Name}[{Age}] say : {content}";
            }


            public static string Run(string content, int value1, int value2, int value3)
            {
                return $"Run {content} => {value1}, {value2}, {value3}";
            }
        }

        public interface IHandler
        {
            void Run();
        }

        public class ComboHandler : IHandler
        {
            private readonly IHandler handler1;
            private readonly IHandler handler2;


            public ComboHandler(IHandler handler1, IHandler handler2)
            {
                this.handler1 = handler1;
                this.handler2 = handler2;
            }


            public void Run()
            {
                handler1?.Run();
                handler2?.Run();
            }
        }


        public class EventTest
        {
            private ComboHandler action;

            private int status;

            public static int value = 0;


            public EventTest(int status)
            {
                this.status = status;
            }


            public void Run()
            {
                action?.Run();
            }


            public void Add(ComboHandler action)
            {
                var eventHandler = this.action;
                while (true)
                {
                    var eventHandler2 = eventHandler;
                    Console.WriteLine("eventHandler2  " + Thread.CurrentThread.ManagedThreadId + eventHandler2);
                    // var value2 = (Action)Delegate.Combine(eventHandler2, action);
                    var value2 = new ComboHandler(this.action, action);
                    if (Interlocked.CompareExchange(ref value, 0, 1) == 0)
                    {
                        Thread.Sleep(1000);
                    }
                    eventHandler = Interlocked.CompareExchange(ref this.action, value2, eventHandler2);
                    if (eventHandler == eventHandler2)
                    {
                        Console.WriteLine("success " + Thread.CurrentThread.ManagedThreadId + eventHandler2);
                        break;
                    }
                    Console.WriteLine("failed " + Thread.CurrentThread.ManagedThreadId + eventHandler2);
                }
            }
        }

        public delegate void Run(string name);

        public delegate void Run1(string name, int value);


        private static async Task Main(string[] args)
        {

            var bus = EventBuses.Create<Run>();
            bus.Add((name) => Console.WriteLine($"bus {name} run"));
            bus.Add((name) => Console.WriteLine($"bus {name} stop"));
            bus.Notify("Parent");

            var childBus = bus.ForkChild();
            childBus.Notify("Lily");
            childBus.Add((name) => Console.WriteLine($"childBus {name} calling"));
            childBus.Notify("Child");

            var bus1 = EventBuses.Create<Run1>();
            bus1.Notify("Tom", 19);


            var constructor = typeof(CPlayer).GetConstructor(new[] {typeof(string), typeof(int)});
            var cPlayerCreator = FastFuncFactory.Invoker(constructor);
            var cPlayer = (CPlayer) cPlayerCreator.Invoke(null, "abc", 10);
            Console.WriteLine(cPlayer.Say("22222"));

            var runMethod = typeof(CPlayer).GetMethod(nameof(CPlayer.Run));
            var runCaller = FastFuncFactory.Invoker(runMethod);
            Console.WriteLine(runCaller.Invoke("22222", 1, 2, 3));

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
                say.Invoke(player, new object[] {"abc"});
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
                func.Method.Invoke(player, new object[] {"abc"});

            }
            watch.Stop();
            Console.WriteLine("func.Method.Invoke : " + watch.ElapsedMilliseconds);

            say = typeof(Player).GetMethod("Say");
            if (say == null)
                return;
            // var exp = new FastFuncFactory<Player, object, object>().Create(say);

            var exp = FastFuncFactory.CreateFactory(say).CreateInvoker(say);
            watch.Restart();
            times = 0;
            while (times++ < maxTime)
            {
                // exp.DynamicInvoke(player, "abc");
                // exp.Invoke(player, "abc");
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
                Thread.Sleep((int) 2000L);
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
