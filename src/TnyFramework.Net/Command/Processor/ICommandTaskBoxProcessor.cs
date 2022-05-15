using TnyFramework.Net.Command.Tasks;

namespace TnyFramework.Net.Command.Processor
{

    public interface ICommandTaskBoxProcessor
    {
        /// <summary>
        /// 提交
        /// </summary>
        /// <param name="box"></param>
        void Submit(CommandTaskBox box);
    }

}
