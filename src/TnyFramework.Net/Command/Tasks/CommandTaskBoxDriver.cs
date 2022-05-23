using System;
using System.Threading;
using System.Threading.Tasks;
using TnyFramework.Coroutines.Async;
using TnyFramework.Net.Command.Processor;

namespace TnyFramework.Net.Command.Tasks
{

    public class CommandTaskBoxDriver
    {
        /* executor停止 */
        private const int STATUS_IDLE_VALUE = 0;

        /* executor提交 */
        private const int STATUS_SUBMIT_VALUE = 1;

        private volatile int status = STATUS_IDLE_VALUE;

        private readonly CommandTaskBox taskBox;

        private readonly Action<AsyncHandle> executor;

        public CommandTaskBoxDriver(CommandTaskBox taskBox, Action<AsyncHandle> executor)
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
                executor(Execute);
            }
        }

        internal async Task Execute()
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
