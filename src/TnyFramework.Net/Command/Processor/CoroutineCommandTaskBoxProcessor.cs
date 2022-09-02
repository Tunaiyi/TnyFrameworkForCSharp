using TnyFramework.Coroutines.Async;
using TnyFramework.Net.Command.Tasks;

namespace TnyFramework.Net.Command.Processor
{

    public class CoroutineCommandTaskBoxProcessor : BaseCommandTaskBoxProcessor<CommandTaskBoxDriver>
    {
        private readonly ICoroutineFactory coroutineFactory;

        public CoroutineCommandTaskBoxProcessor()
        {
            coroutineFactory = DefaultCoroutineFactory.Default;
        }

        public CoroutineCommandTaskBoxProcessor(ICoroutineFactory coroutineFactory)
        {
            this.coroutineFactory = coroutineFactory;
        }

        protected override IAsyncExecutor CreateExecutor()
        {
            return coroutineFactory.Create("CoroutineCommandTaskBoxProcessor");
        }
    }

}
