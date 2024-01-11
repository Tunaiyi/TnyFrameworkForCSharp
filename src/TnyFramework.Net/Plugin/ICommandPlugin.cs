// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using TnyFramework.Net.Command.Dispatcher;
using TnyFramework.Net.Message;
using TnyFramework.Net.Transport;

namespace TnyFramework.Net.Plugin;

/// <summary>
/// 插件接口
/// </summary>
public interface ICommandPlugin
{
    /// <summary>
    /// 执行插件
    /// </summary>
    /// <param name="tunnel">管道</param>
    /// <param name="message">消息</param>
    /// <param name="context">command 上下文</param>
    /// <param name="attributes">参数</param>
    void Execute(ITunnel tunnel, IMessage message, RpcInvokeContext context, object? attributes);
}

public abstract class CommandPlugin<TAttribute> : ICommandPlugin
{
    /// <summary>
    /// 执行插件
    /// </summary>
    /// <param name="tunnel"></param>
    /// <param name="message"></param>
    /// <param name="context"></param>
    /// <param name="attributes"></param>
    public abstract void Execute(ITunnel tunnel, IMessage message, RpcInvokeContext context, TAttribute? attributes);

    public void Execute(ITunnel tunnel, IMessage message, RpcInvokeContext context, object? attributes)
    {
        Execute(tunnel, message, context, (TAttribute?) attributes);
    }
}
