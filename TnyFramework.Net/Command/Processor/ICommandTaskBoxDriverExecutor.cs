using TnyFramework.Net.Command.Tasks;
namespace TnyFramework.Net.Command.Processor
{
    public interface ICommandTaskBoxDriverExecutor
    {
        void Execute(CommandTaskBoxDriver driver);
    }

    public interface ICommandTaskBoxDriverExecutor<in TDriver> : ICommandTaskBoxDriverExecutor
        where TDriver : CommandTaskBoxDriver
    {
        void Execute(TDriver driver);
    }
}
