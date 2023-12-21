// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using TnyFramework.Common.Exceptions;
using TnyFramework.Common.Extensions;
using TnyFramework.Coroutines.Async;
using TnyFramework.DI.Extensions;
using TnyFramework.DI.Units;

namespace TnyFramework.Demo.Test;

public class Program
{
    // public static Task WaitBackground(this Task task)
    // {
    //     var stack = new StackTrace(1, true);
    //     return Task.Run(async () => {
    //         try
    //         {
    //             await task;
    //         } catch (Exception e)
    //         {
    //             Console.WriteLine(
    //                 $"Wait background exception :\n {e.GetType()} : {e.Message}\n{e.StackTrace}\nbackground wait task was invoked at \n {stack}");
    //             Console.WriteLine(e);
    //         }
    //     });
    // }

    static void TaskScheduler_UnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
    {
        Console.WriteLine($"Unobserved exception: {e.Exception}");
        e.SetObserved();
    }

    private static void TaskScheduler_UnobservedTaskException(object sender, UnhandledExceptionEventArgs e)
    {
        Console.WriteLine($"Unobserved exception: {e.ExceptionObject}");
    }

    private static async Task RunException()
    {
        Console.WriteLine($"ArgumentException before");
        try
        {
            await Task.CompletedTask;
            throw new CommonException("22222");
        } finally
        {
            Console.WriteLine($"ArgumentException after");
        }
    }

    public interface IPlayer
    {
        string Name { get; }
    }

    public class PlayerA : IPlayer
    {
        public PlayerA(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }

    public class PlayerB : IPlayer
    {
        public PlayerB(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }

    public static void Main(string[] args)
    {
        var services = new ServiceCollection();
        services.AddSingletonUnit<IPlayer>("PlayerA", new PlayerA("A"));
        services.AddSingletonUnit<IPlayer, PlayerA>("PlayerB", new PlayerA("B"));
        var provider = services.BuildServiceProvider();
        var playerUnits = provider.GetService<IUnitCollection<IPlayer>>();

        // AppDomain.CurrentDomain.UnhandledException += (o, eventArgs) => { Console.WriteLine($"{eventArgs.ExceptionObject}"); };
        TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
        Task.Delay(100).Wait();
        var executor = TaskScheduler.Default;
        var factory = new DefaultCoroutineFactory("Actor", executor);
        var coroutine1 = factory.Create();
        coroutine1.TaskScheduler().StartNew(RunException);
        Task.Delay(10).Wait();
        GC.Collect();
        Task.Delay(5000).Wait();
        // GC.WaitForPendingFinalizers();
        // Task.Delay(2000).Wait();
    }
}
