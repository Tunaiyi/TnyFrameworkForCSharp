using System;

namespace TnyFramework.Namespace.Listener
{

    /// <summary>
    /// 开始租约事件
    /// </summary>
    /// <param name="source">事件源</param>
    public delegate void LesseeOnLease(ILessee source);

    /// <summary>
    /// 恢复租约续约事件
    /// </summary>
    /// <param name="source">事件源</param>
    public delegate void LesseeOnResume(ILessee source);

    /// <summary>
    /// 租约续约事件
    /// </summary>
    /// <param name="source">事件源</param>
    public delegate void LesseeOnRenew(ILessee source);

    /// <summary>
    /// 租约取消事件
    /// </summary>
    /// <param name="source">事件源</param>
    public delegate void LesseeOnCompleted(ILessee source);

    /// <summary>
    /// 租约异常事件
    /// </summary>
    /// <param name="source">事件源</param>
    public delegate void LesseeOnError(ILessee source, Exception cause);

}
