using TnyFramework.Net.Command.Tasks;

namespace TnyFramework.Net.Command.Processor
{

    public abstract class CommandTaskBoxDriverExecutor<TDriver> : ICommandTaskBoxDriverExecutor<TDriver>
        where TDriver : CommandTaskBoxDriver
    {
        public abstract void Execute(TDriver driver);

        public void Execute(CommandTaskBoxDriver driver)
        {
            Execute((TDriver) driver);
        }
    }

}
