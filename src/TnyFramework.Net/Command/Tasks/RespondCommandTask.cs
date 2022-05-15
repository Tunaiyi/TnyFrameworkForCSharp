using System.Threading.Tasks;
using TnyFramework.Net.Dispatcher;
using TnyFramework.Net.Message;
using TnyFramework.Net.Transport;

namespace TnyFramework.Net.Command.Tasks
{

    public class RespondCommandTask : ICommandTask, ICommand
    {
        private readonly IMessage message;

        private readonly TaskResponseSource source;

        public string Name => "RespondCommandTask";

        public RespondCommandTask(IMessage message, TaskResponseSource source)
        {
            this.message = message;
            this.source = source;
        }

        public ICommand Command => this;

        public Task Execute()
        {
            source.SetResult(message);
            return Task.CompletedTask;
        }

        public bool IsDone() => source.Task.IsCompleted;
    }

}
