using System.Threading.Tasks;

namespace TnyFramework.Net.Command.Tasks;

public interface ICommandTaskSchedulerFactory
{
    TaskScheduler CreateTaskScheduler(CommandBox commandBox, string name);
}
