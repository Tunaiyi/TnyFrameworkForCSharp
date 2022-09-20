namespace TnyFramework.Common.Lifecycle
{

    /// <summary>
    /// 什么周期优先级
    /// </summary>
    public interface ILifecyclePriority
    {
        int Order { get; }
    }

}
