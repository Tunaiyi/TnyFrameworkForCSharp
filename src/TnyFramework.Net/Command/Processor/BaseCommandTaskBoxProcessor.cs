using System;
using TnyFramework.Coroutines.Async;
using TnyFramework.Net.Command.Tasks;

namespace TnyFramework.Net.Command.Processor
{

    public abstract class BaseCommandTaskBoxProcessor<TDriver> : ICommandTaskBoxProcessor
        where TDriver : CommandTaskBoxDriver
    {
        public void Submit(CommandTaskBox box)
        {
            // 创角任务触发器
            var driver = box.GetAttachment<TDriver>() ?? box.SetAttachmentIfNull(this, () => CreateDriver(box));
            driver.TrySummit(); // 尝试提交
        }

        private CommandTaskBoxDriver CreateDriver(CommandTaskBox box)
        {
            return new CommandTaskBoxDriver(box, CreateExecutor());
        }

        protected abstract Action<AsyncHandle> CreateExecutor();
    }

}
