// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using TnyFramework.Net.Message;
using TnyFramework.Net.Rpc;

namespace TnyFramework.Net.Command.Dispatcher
{

    /// <summary>
    /// 转发器
    /// </summary>
    public interface IRpcForwarder
    {
        /// <summary>
        /// 获取转发 AccessPoint
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="from">发送的 Rpc 服务(可选)</param>
        /// <param name="sender">发送者(可选)</param>
        /// <param name="to">目标 Rpc 服务</param>
        /// <param name="receiver">接受者(可选)</param>
        /// <returns></returns>
        IRpcRemoteAccessPoint Forward(IMessage message, IRpcServicer from, IMessager sender, IRpcServicer to, IMessager receiver);
    }

}
