// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

namespace TnyFramework.Net.Application;

public interface INetServer : IServedService
{
    /// <summary>
    /// 是否启动
    /// </summary>
    /// <returns></returns>
    bool IsOpen();

    IServiceSetting Setting { get; }
}

public interface INetServer<out TSetting> : INetServer
    where TSetting : IServiceSetting
{
    /// <summary>
    /// 设置
    /// </summary>
    TSetting ServiceSetting { get; }
}
