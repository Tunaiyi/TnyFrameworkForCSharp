using System;
using System.Threading.Tasks;
namespace TnyFramework.Net.Dispatcher
{
    public class RunnableCommand : BaseCommand
    {
        private readonly Action action;


        public static RunnableCommand Action(Action action)
        {
            return new RunnableCommand(action);
        }


        public RunnableCommand(Action action) : base("RunnableCommand")
        {
            this.action = action;
        }


        protected override Task Action()
        {
            action.Invoke();
            return Task.CompletedTask;
        }
    }
}
