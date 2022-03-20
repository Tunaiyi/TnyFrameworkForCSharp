using TnyFramework.Net.Command.Tasks;
namespace TnyFramework.Net.Command.Processor
{
    public abstract class BaseCommandTaskBoxProcessor<TDriver> : ICommandTaskBoxProcessor
        where TDriver : CommandTaskBoxDriver
    {
        public void Submit(CommandTaskBox box)
        {
            CommandTaskBoxDriver driver = box.GetAttachment<TDriver>();
            if (driver == null)
            {
                // 创角任务触发器
                driver = box.SetAttachmentIfNull(this, () => CreateDriver(box));
            }
            driver.TrySummit(); // 尝试提交
        }


        protected abstract CommandTaskBoxDriver CreateDriver(CommandTaskBox box);
    }
}
