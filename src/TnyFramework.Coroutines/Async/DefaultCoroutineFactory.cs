using System.Threading;
namespace TnyFramework.Coroutines.Async
{
    /// <summary>
    /// 默认协程工厂
    /// </summary>
    public class DefaultCoroutineFactory : ICoroutineFactory
    {
        private int index;

        private readonly ICoroutineExecutor executor;

        public static ICoroutineFactory Default { get; } = new DefaultCoroutineFactory("DefaultCoroutineFactory");


        /// <summary>
        /// 工厂名(作为协程名前缀)
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="executor">执行器</param>
        public DefaultCoroutineFactory(string name = null, ICoroutineExecutor executor = null)
        {
            Name = name ?? "CoroutineFactory";
            this.executor = executor ?? ThreadPoolCoroutineExecutor.Default;
        }


        private string Name { get; }


        public ICoroutine Create()
        {
            var currentIndex = Interlocked.Increment(ref index);
            return new Coroutine(executor, $"{Name}-{currentIndex}");
        }


        public ICoroutine Create(string name)
        {
            var currentIndex = Interlocked.Increment(ref index);
            return new Coroutine(executor, $"{name}-{currentIndex}");
        }
    }
}
