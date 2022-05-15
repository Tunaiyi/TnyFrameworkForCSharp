using TnyFramework.Net.Dispatcher;

namespace TnyFramework.Net.Command.Tasks
{

    public interface ICommandTask
    {
        ICommand Command { get; }
    }

}
