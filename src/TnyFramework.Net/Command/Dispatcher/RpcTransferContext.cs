// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using TnyFramework.Net.Message;

namespace TnyFramework.Net.Command.Dispatcher
{

    public interface IRpcTransferContext : IRpcTransactionContext, IRpcEnterCompletable
    {
        /// <summary>
        /// 转发
        /// </summary>
        /// <param name="to">目标</param>
        /// <param name="operationName">操作</param>
        /// <returns>返回是否成功</returns>
        bool Transfer(INetMessager to, string operationName);

        /// <summary>
        /// 传送消息
        /// </summary>
        IMessage Message { get; }

        /// <summary>
        /// 发送服务
        /// </summary>
        INetMessager From { get; }

        /// <summary>
        /// 目标服务
        /// </summary>
        INetMessager To { get; }
    }

}