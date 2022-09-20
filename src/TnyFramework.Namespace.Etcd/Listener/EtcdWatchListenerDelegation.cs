// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

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
