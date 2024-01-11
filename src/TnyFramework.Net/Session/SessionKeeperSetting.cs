// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using TnyFramework.DI.Container;

namespace TnyFramework.Net.Session;

public class SessionKeeperSetting : ISessionKeeperSetting
{
    private readonly SessionSetting sessionSetting = new();

    public string Name { get; set; } = "";

    public string KeeperFactory { get; set; } = Unit.DefaultName<SessionKeeperFactory>();

    public string SessionFactory { get; set; } = Unit.DefaultName<SessionFactory>();

    public long OfflineCloseDelay { get; set; }

    public int OfflineMaxSize { get; set; }

    public long ClearSessionInterval { get; set; } = 60000;

    public ISessionSetting SessionSetting => sessionSetting;
}
