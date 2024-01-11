// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using TnyFramework.Coroutines.Async;
using TnyFramework.Net.Transport;

namespace TnyFramework.Net.Session;

public interface ISession : ICommunicator, IConnection, ISender, IAsyncExecutor
{
    long Id { get; }

    MessageHandleFilter? SendFilter { get; set; }

    MessageHandleFilter? ReceiveFilter { get; set; }

    void Heartbeat();

    SessionStatus Status { get; }

    long OfflineTime { get; }

    void Offline();

    bool IsOnline();

    bool IsOffline();
}
