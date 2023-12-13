namespace TnyFramework.Net.Command.Tasks
{

    public interface ICommandExecutorFactory
    {
        ICommandExecutor CreateCommandExecutor(CommandBox commandBox);
    }

}
