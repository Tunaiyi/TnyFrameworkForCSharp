namespace TnyFramework.Net.Command.Tasks
{

    public class SerialCommandExecutorFactory : ICommandExecutorFactory
    {
        private readonly ICommandTaskSchedulerFactory taskSchedulerFactory;

        public SerialCommandExecutorFactory()
        {
            taskSchedulerFactory = new CoroutineCommandTaskSchedulerFactory();
        }

        public SerialCommandExecutorFactory(ICommandTaskSchedulerFactory taskSchedulerFactory)
        {
            this.taskSchedulerFactory = taskSchedulerFactory;
        }

        public ICommandExecutor CreateCommandExecutor(CommandBox commandBox)
        {
            var taskScheduler = taskSchedulerFactory.CreateTaskScheduler(commandBox, "CoroutineCommandExecutor");
            return new SerialCommandExecutor(commandBox, taskScheduler);
        }
    }

}
