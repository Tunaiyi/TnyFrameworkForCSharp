// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using TnyFramework.Net.Extensions;

namespace TnyFramework.Net.Application;

public abstract class NetServer : INetServer
{
    string INetService.Service => Setting.ServiceName();

    /// <summary>
    /// 服务发现名
    /// </summary>
    public string ServeName => Setting.DiscoverService();

    public abstract bool IsOpen();

    public abstract IServiceSetting Setting { get; }
}

public abstract class NetServer<TSetting> : NetServer, INetServer<TSetting>
    where TSetting : IServiceSetting
{
    public override IServiceSetting Setting => ServiceSetting;

    public abstract TSetting ServiceSetting { get; }
}
