using System;
using Etcdserverpb;

namespace TnyFramework.Namespace.Etcd.Listener
{

    /// <summary>
    /// 开始监听
    /// </summary>
    public delegate void EtcdWatchOnWatch();

    /// <summary>
    /// 监听到节点改变
    /// </summary>
    /// <param name="response">响应</param>
    public delegate void EtcdWatchOnChange(WatchResponse response);

    /// <summary>
    /// 监听异常
    /// </summary>
    /// <param name="exception">异常</param>
    public delegate void EtcdWatchOnError(Exception exception);

    /// <summary>
    /// 监听结束
    /// </summary>
    public delegate void EtcdWatchOnCompleted();

}
