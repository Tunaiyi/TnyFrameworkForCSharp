using TnyFramework.Coroutines.Async;
using TnyFramework.Net.Command.Tasks;
namespace TnyFramework.Net.Command.Processor
{
    public class CoroutineCommandTaskBoxDriverExecutor<TDriver> : CommandTaskBoxDriverExecutor<TDriver>
        where TDriver : CommandTaskBoxDriver
    {
        private readonly ICoroutine coroutine;


        public CoroutineCommandTaskBoxDriverExecutor(ICoroutine coroutine)
        {
            this.coroutine = coroutine;
        }


        public override void Execute(TDriver driver)
        {
            coroutine.Run(driver.Execute);
        }
    }
}
