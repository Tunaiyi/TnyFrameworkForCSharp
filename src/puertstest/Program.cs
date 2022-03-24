#region

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using ProtoBuf;
using Puerts.Api;
using Puerts.Api.General;
using puertstest.World.Data;
using puertstest.World.Game;
using StackExchange.Redis;
using TnyFramework.Coroutines.Async;

#endregion

namespace puertstest
{

    public class TxtLoader : IPuertsLoader
    {
        private readonly string root = Path.Combine(
            Regex.Replace(
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase),
                "^file:(\\\\)?", ""), "js");


        public bool FileExists(string filepath)
        {
            if (filepath.EndsWith(".js"))
            {
                filepath += ".txt";
            }
            return File.Exists(Path.Combine(root, filepath));
        }


        public string ReadFile(string filepath, out string debugPath)
        {
            if (filepath.EndsWith(".js"))
            {
                filepath = filepath + ".txt";
            }
            debugPath = Path.Combine(root, filepath);
            return File.ReadAllText(debugPath);
        }
    }

    public class Debugger
    {
        public static void Log(string message)
        {
            Console.WriteLine(message);
        }
    }

    class Program
    {
        public static async Task Main(string[] args)
        {
            var puertsLoader = new TxtLoader();
            var envFactory = new GeneralPuertsEnvFactory(puertsLoader)
                .AddCustomization(new CommonPuertsCustomization());
            // PuertsManager.EnvFactory.AddCustomization(env => {
            //     env.UsingFunc<Item, int>();
            //     env.UsingFunc<int>();
            // });
            var coroutineFactory = new DefaultCoroutineFactory("Main", new ForkJoinThreadCoroutineExecutor(4, "Main"));
            Console.WriteLine($"main == ManagedThreadId : {Thread.CurrentThread.ManagedThreadId}-{Thread.CurrentThread.Name}");
            CoroutineSynchronizationContext.InitializeSynchronizationContext(coroutineFactory.Create());
            // Console.WriteLine($"main == ManagedThreadId : {Thread.CurrentThread.ManagedThreadId}-{Thread.CurrentThread.Name}");
            // await Task.Yield();
            // Console.WriteLine($"init env == ManagedThreadId : {Thread.CurrentThread.ManagedThreadId}-{Thread.CurrentThread.Name}");
            // var testEnv = new JsEnv(new TxtLoader());
            // Console.WriteLine($"inited env == ManagedThreadId : {Thread.CurrentThread.ManagedThreadId}-{Thread.CurrentThread.Name}");
            // testEnv.UsingFunc<int>();
            // var func = testEnv.Eval<Func<int>>("()=>{return 100;}");
            // Console.WriteLine($"create Eval == ManagedThreadId : {Thread.CurrentThread.ManagedThreadId}-{Thread.CurrentThread.Name}");
            // Console.WriteLine(func());
            // Console.WriteLine($"invoked Eval == ManagedThreadId : {Thread.CurrentThread.ManagedThreadId}-{Thread.CurrentThread.Name}");
            // for (var i = 0; i < 200; i++)
            // {
            //     var source = new TaskCompletionSource<bool>();
            //     var thread = new Thread(() => {
            //         Console.WriteLine(func());
            //         source.SetResult(true);
            //     });
            //     thread.Start();
            //     var _ = source.Task.Result;
            // }

            // await Task.Yield();
            // Console.WriteLine($"invoke Eval == ManagedThreadId : {Thread.CurrentThread.ManagedThreadId}-{Thread.CurrentThread.Name}");
            // Console.WriteLine(func());
            // Console.WriteLine($"invoked Eval == ManagedThreadId : {Thread.CurrentThread.ManagedThreadId}-{Thread.CurrentThread.Name}");
            // for (var index = 0; index < 1000; index++)
            // {
            //     Task.Run(() => {
            //         try
            //         {
            //             Console.WriteLine($"{func()} ManagedThreadId : {Thread.CurrentThread.ManagedThreadId}-{Thread.CurrentThread.Name}");
            //         } catch (Exception e)
            //         {
            //             Console.WriteLine(e);
            //         }
            //     });
            // }
            //
            await Task.Yield();
            Console.WriteLine($"main == ManagedThreadId : {Thread.CurrentThread.ManagedThreadId}-{Thread.CurrentThread.Name}");
            var itemService = new ItemService();
            Console.WriteLine($"ItemService == ManagedThreadId : {Thread.CurrentThread.ManagedThreadId}-{Thread.CurrentThread.Name}");

            // await initCoroutine.Run(() => {
            Console.WriteLine($"initCoroutine === ManagedThreadId : {Thread.CurrentThread.ManagedThreadId}-{Thread.CurrentThread.Name}");
            Console.WriteLine(Directory.GetCurrentDirectory());
            itemService.Init(envFactory, true);
            ItemModels.Ins.SetProvider(itemService);
            Console.WriteLine($"initCoroutine === ManagedThreadId : {Thread.CurrentThread.ManagedThreadId}-{Thread.CurrentThread.Name}");
            // });
            await Task.Yield();

            const long roomId = 94649259453920008L;
            const int playerId = 111968;
            // await coroutine.Exec(async () => {
            Console.WriteLine($"redis == ManagedThreadId : {Thread.CurrentThread.ManagedThreadId}-{Thread.CurrentThread.Name}");
            var redis = await ConnectionMultiplexer.ConnectAsync("127.0.0.1:6379", options => { options.Password = "tNbrH3NSErGd"; });
            var database = redis.GetDatabase(5);
            var values = database.SortedSetRangeByRank(new RedisKey($"R2:WS:frames:{{{roomId}}}"));

            var snaps = new NetLockStepSnaps();
            var gamerIds = new List<long> {playerId};
            Console.WriteLine($"create snap == ManagedThreadId : {Thread.CurrentThread.ManagedThreadId}-{Thread.CurrentThread.Name}");

            snaps.JoinRoom(roomId, 0, 0);
            foreach (var redisValue in values)
            {
                var bytes = (byte[]) redisValue.Box();
                using (var stream = new MemoryStream(bytes))
                {
                    var frameData = new RelayFrameData();
                    Serializer.Deserialize(stream, frameData);
                    snaps.AddFrameData(frameData);
                }
            }

            Console.WriteLine($"run 1 == ManagedThreadId : {Thread.CurrentThread.ManagedThreadId}-{Thread.CurrentThread.Name}");
            var totalStopwatch = Stopwatch.StartNew();
            var lockStepDataCoder = new LockStepCoder();
            // worlds.JoinWorld();
            var frames = new List<GFrame>();
            while (snaps.HasUnreadFrame())
            {
                var frame = new GFrame();
                snaps.ReadFrame(frame);
                frames.Add(frame);
                if (frame.ValidDataCount > 0)
                {
                    frame.Decode(lockStepDataCoder);
                }
            }
            totalStopwatch.Stop();
            var codeTime = totalStopwatch.ElapsedMilliseconds;

            await Task.Delay(10);
            Console.WriteLine($"run 2 == ManagedThreadId : {Thread.CurrentThread.ManagedThreadId}-{Thread.CurrentThread.Name}");

            var worlds = Worlds.Create(null, false);
            WorldsContext.Ins = worlds;
            var data = new WorldOriginalDTO(roomId, WorldType.DEFAULT_1, playerId, gamerIds);
            worlds.CreateWorld(data);

            totalStopwatch.Restart();
            foreach (var frame in frames)
            {
                worlds.WorldDriver.Work(frame);
            }
            totalStopwatch.Stop();
            var runTime = totalStopwatch.ElapsedMilliseconds;
            Console.WriteLine($"stopwatch runTime : {runTime} | codeTime {codeTime} | total : {codeTime + runTime} | frames size : {frames.Count}");
            worlds.ExitWorld();


            worlds.CreateWorld(data);
            totalStopwatch.Restart();
            foreach (var frame in frames)
            {
                worlds.WorldDriver.Work(frame);
            }
            totalStopwatch.Stop();
            runTime = totalStopwatch.ElapsedMilliseconds;
            Console.WriteLine($"stopwatch runTime : {runTime} | frames size : {frames.Count}");

            // });
            Console.ReadLine();
        }
    }

}
