using System.Threading.Tasks;
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

        public Task AsyncExec(CommandTaskBox box, AsyncHandle handle)
        {
            var driver = box.GetAttachment<TDriver>() ?? box.SetAttachmentIfNull(this, () => CreateDriver(box));
            return driver.AsyncExec(handle);
        }

        public Task<T> AsyncExec<T>(CommandTaskBox box, AsyncHandle<T> function)
        {
            var driver = box.GetAttachment<TDriver>() ?? box.SetAttachmentIfNull(this, () => CreateDriver(box));
            return driver.AsyncExec(function);
        }

        private CommandTaskBoxDriver CreateDriver(CommandTaskBox box)
        {
            return new CommandTaskBoxDriver(box, CreateExecutor());
        }

        protected abstract IAsyncExecutor CreateExecutor();
    }

}
