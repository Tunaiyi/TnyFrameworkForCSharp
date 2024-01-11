// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using TnyFramework.Net.Session;

namespace TnyFramework.Net.Transport;

public interface ICommunicator : IConnectIdentity, IAddressPeer
{
    /// <summary>
    /// 接入模式
    /// </summary>
    public NetAccessMode AccessMode { get; }

    /// <summary>
    /// 认证
    /// </summary>
    ICertificate Certificate { get; }

    /// <summary>
    /// 是否登陆认证
    /// </summary>
    /// <returns></returns>
    bool IsAuthenticated() => Certificate.IsAuthenticated();
}
