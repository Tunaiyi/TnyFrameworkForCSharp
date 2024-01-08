// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using TnyFramework.Net.Application;
using TnyFramework.Net.Session;

namespace TnyFramework.Net.Transport
{

    public interface ITunnel : ICommunicator, IConnection, ISender, INetService
    {
        /// <summary>
        /// 通道 Id
        /// </summary>
        long Id { get; }

        /// <summary>
        /// 访问 id
        /// </summary>
        /// <returns></returns>
        long AccessId { get; }

        /// <summary>
        /// 通道状态
        /// </summary>
        TunnelStatus Status { get; }

        /// <summary>
        /// 是否已经开启
        /// </summary>
        /// <returns></returns>
        bool IsOpen();

        /// <summary>
        /// 会话
        /// </summary>
        ISession Session { get; }
    }

}
