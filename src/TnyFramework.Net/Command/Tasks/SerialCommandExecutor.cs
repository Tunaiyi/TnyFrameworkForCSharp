// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System.Threading;
using System.Threading.Tasks;
using TnyFramework.Common.Extensions;
using TnyFramework.Coroutines.Async;

namespace TnyFramework.Net.Command.Tasks;

public class SerialCommandExecutor : ICommandExecutor
{
    /* executor停止 */
    private const int STATUS_IDLE_VALUE = 0;

    /* executor提交 */
    private const int STATUS_SUBMIT_VALUE = 1;

    private volatile int status = STATUS_IDLE_VALUE;

    private readonly CommandBox box;

    public TaskScheduler TaskScheduler { get; }

    public SerialCommandExecutor(CommandBox box, TaskScheduler scheduler)
    {
        this.box = box;
        TaskScheduler = scheduler;
    }

    public void TrySummit()
    {
        var current = status;
        if (current != STATUS_IDLE_VALUE)
            return;
        if (Interlocked.CompareExchange(ref status, STATUS_SUBMIT_VALUE, STATUS_IDLE_VALUE) == current)
        {
            TaskScheduler.StartNew(ExecuteLoop);
        }
    }

    public Task AsyncExec(AsyncHandle handle)
    {
        return TaskScheduler.StartNew(handle.Invoke).Unwrap();
    }

    public Task<T> AsyncExec<T>(AsyncHandle<T> function)
    {
        return TaskScheduler.StartNew(function.Invoke).Unwrap();
    }

    private async Task ExecuteLoop()
    {
        try
        {
            while (box.Poll(out var command))
            {
                if (!command.IsDone())
                {
                    await command.Execute();
                }
            }
        } finally
        {
            Interlocked.Exchange(ref status, STATUS_IDLE_VALUE);
            if (!box.IsEmpty)
            {
                TrySummit();
            }
        }
    }
}
