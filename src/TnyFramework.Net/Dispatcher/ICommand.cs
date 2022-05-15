using System.Threading.Tasks;

namespace TnyFramework.Net.Dispatcher
{

    public interface ICommand
    {
        /// <summary>
        /// 执行
        /// </summary>
        Task Execute();

        /// <summary>
        /// 是否成功完成
        /// </summary>
        /// <returns></returns>
        bool IsDone();

        /// <summary>
        /// 命令名字
        /// </summary>
        string Name { get; }
    }

    public abstract class BaseCommand : ICommand
    {
        private bool done;

        protected BaseCommand() : this(null)
        {
        }

        protected BaseCommand(string name)
        {
            Name = name;
        }

        public string Name { get; }

        /// <summary>
        /// 执行
        /// </summary>
        public async Task Execute()
        {
            try
            {
                await Action();
            } finally
            {
                done = true;
            }
        }

        protected abstract Task Action();

        /// <summary>
        /// 是否成功完成
        /// </summary>
        /// <returns></returns>
        public bool IsDone() => done;
    }

}
