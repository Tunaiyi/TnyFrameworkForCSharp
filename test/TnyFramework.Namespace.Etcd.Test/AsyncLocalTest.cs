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
using NUnit.Framework;
using TnyFramework.Common.Lifecycle;
using TnyFramework.Common.Logger;
using TnyFramework.Coroutines.Async;

namespace TnyFramework.Namespace.Etcd.Test;

public class AsyncLocalTest
{
    private static readonly ILogger LOGGER = LogFactory.Logger<AsyncLocalTest>();

    [Test]
    public void TestBaseEnum()
    {
            LifecycleStage.LoadAll();
        }

    [Test]
    public async Task TestAwaitSelf()
    {
            var coroutine = DefaultCoroutineFactory.Default.Create("TestAwaitSelf");
            await coroutine.AsyncExec(async () => {
                Console.WriteLine("2 - 1 " + Coroutine.CurrentCoroutine!.Name + Thread.CurrentThread.ManagedThreadId);
                await coroutine.AsyncExec(async () => {
                    Console.WriteLine("1 - 1 " + Coroutine.CurrentCoroutine.Name + Thread.CurrentThread.ManagedThreadId);
                    await coroutine.AsyncExec(async () => {
                        Console.WriteLine("Delay " + Coroutine.CurrentCoroutine.Name + Thread.CurrentThread.ManagedThreadId);
                        await Task.Delay(200);
                    });
                    Console.WriteLine("1 - 2 " + Coroutine.CurrentCoroutine.Name + Thread.CurrentThread.ManagedThreadId);
                });
                Console.WriteLine("2 - 2 " + Coroutine.CurrentCoroutine.Name + Thread.CurrentThread.ManagedThreadId);
            });
        }

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