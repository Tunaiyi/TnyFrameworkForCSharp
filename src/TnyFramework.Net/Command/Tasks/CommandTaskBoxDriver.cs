using System;
using System.Threading;
using System.Threading.Tasks;
using TnyFramework.Coroutines.Async;

namespace TnyFramework.Net.Command.Tasks
{

    public class CommandTaskBoxDriver : IAsyncExecutor
    {
        /* executor停止 */
        private const int STATUS_IDLE_VALUE = 0;

        /* executor提交 */
        private const int STATUS_SUBMIT_VALUE = 1;

        private volatile int status = STATUS_IDLE_VALUE;

        private readonly CommandTaskBox taskBox;

        private readonly IAsyncExecutor executor;

        public CommandTaskBoxDriver(CommandTaskBox taskBox, IAsyncExecutor executor)
        {
            this.taskBox = taskBox;
            this.executor = executor;
        }

        public void TrySummit()
        {
            var current = status;
            if (current != STATUS_IDLE_VALUE)
                return;
            if (Interlocked.CompareExchange(ref status, STATUS_SUBMIT_VALUE, STATUS_IDLE_VALUE) == current)
            {
                executor.AsyncExec(Execute);
            }
        }

        public Task AsyncExec(AsyncHandle handle)
        {
            return executor.AsyncExec(handle);
        }

        public Task<T> AsyncExec<T>(AsyncHandle<T> function)
        {
            return executor.AsyncExec(function);
        }
        
        private async Task Execute()
        {
            try
            {
                while (taskBox.Poll(out var task))
                {
                    var command = task.Command;
                    await command.Execute();
                }
            } finally
            {
                Interlocked.Exchange(ref status, STATUS_IDLE_VALUE);
                if (!taskBox.IsEmpty)
                {
                    TrySummit();
                }
            }
        }
    }

}
