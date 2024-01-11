// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System.Collections.Generic;

namespace TnyFramework.Net.Application;

public interface IServedServerSetting : IServerSetting
{
    /// <summary>
    /// 上报绑定域名
    /// </summary>
    string ServeHost { get; }

    /// <summary>
    /// 上报绑定端口
    /// </summary>
    int ServePort { get; }

    /// <summary>
    /// 信息
    /// </summary>
    IDictionary<string, string>? Metadata { get; }
}
