// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using TnyFramework.Net.Rpc.Configuration;

namespace TnyFramework.Net.Hosting.Options;

public class RpcRemoteServiceSetting : IRpcRemoteServiceSetting
{
    // rpc服务名
    public string Service { get; set; } = "";

    // 服务发现-服务器服务名
    public string ServeName { get; set; } = "";

    public string Username { get; set; } = "";

    public string Password { get; set; } = "";

    public string Host { get; set; } = "";

    public int Port { get; set; }

    public string Guide { get; set; } = "";

    public int ConnectSize { get; set; } = 1;

    public long ConnectTimeout { get; set; } = 10000;

    public long AuthenticateTimeout { get; set; } = 30000;
}
