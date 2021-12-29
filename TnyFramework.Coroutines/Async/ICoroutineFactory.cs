namespace TnyFramework.Coroutines.Async
{
    /// <summary>
    /// 协程工厂
    /// </summary>
    public interface ICoroutineFactory
    {
        /// <summary>
        /// 创建协程
        /// </summary>
        /// <returns></returns>
        ICoroutine Create();
    }
}
