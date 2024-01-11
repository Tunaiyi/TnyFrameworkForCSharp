// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using TnyFramework.DI.Units;
using TnyFramework.Net.DotNetty.Bootstrap;

namespace TnyFramework.Net.DotNetty.Hosting.Guide;

public class ServerSettingSpec : UnitSpec<INettyServerSetting, object>, IServerSettingSpec
{
    private NettyServerSetting Setting { get; } = new();

    public ServerSettingSpec(string unitName = "") : base(unitName)
    {
        Default(_ => Setting);
    }

    /// <summary>
    /// 设置服务名
    /// </summary>
    /// <param name="value">服务名</param>
    /// <returns></returns>
    public ServerSettingSpec ServiceName(string value)
    {
        Setting.Name = value;
        return this;
    }

    /// <summary>
    /// 设置服务发现名
    /// </summary>
    /// <param name="value">服务发现名</param>
    /// <returns></returns>
    public ServerSettingSpec ServeName(string value)
    {
        Setting.ServeName = value;
        return this;
    }

    /// <summary>
    /// 设置主机名(域名/IP)
    /// </summary>
    /// <param name="value">域名</param>
    /// <returns></returns>
    public ServerSettingSpec Host(string value)
    {
        Setting.Host = value;
        return this;
    }

    /// <summary>
    /// 设置端口
    /// </summary>
    /// <param name="port"></param>
    /// <returns></returns>
    public ServerSettingSpec Port(int port)
    {
        Setting.Port = port;
        return this;
    }

    /// <summary>
    /// 是否使用 Libuv
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public ServerSettingSpec Libuv(bool value)
    {
        Setting.Libuv = value;
        return this;
    }
}
