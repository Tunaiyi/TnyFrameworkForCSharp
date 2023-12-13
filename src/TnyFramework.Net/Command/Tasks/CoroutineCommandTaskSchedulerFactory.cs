using System.Threading.Tasks;
using TnyFramework.Coroutines.Async;

namespace TnyFramework.Net.Command.Tasks;

public class CoroutineCommandTaskSchedulerFactory : ICommandTaskSchedulerFactory
{
    private readonly ICoroutineFactory coroutineFactory;

    public CoroutineCommandTaskSchedulerFactory()
    {
        coroutineFactory = DefaultCoroutineFactory.Default;
    }

    public CoroutineCommandTaskSchedulerFactory(ICoroutineFactory coroutineFactory)
    {
        this.coroutineFactory = coroutineFactory;
    }

    public TaskScheduler CreateTaskScheduler(CommandBox commandBox, string name)
    {
        return coroutineFactory.Create(name);
    }
}
