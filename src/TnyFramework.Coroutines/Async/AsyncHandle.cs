using System.Threading.Tasks;

namespace TnyFramework.Coroutines.Async
{

    /// <summary>
    /// 协程 Action 委托
    /// </summary>
    public delegate Task AsyncHandle();

    /// <summary>
    /// 协程 Func 委托
    /// </summary>
    /// <typeparam name="T">返回类型</typeparam>
    public delegate Task<T> AsyncHandle<T>();

}
