// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using TnyFramework.Net.Transport;

namespace TnyFramework.Net.Session;

public class Session : NetSession
{
    public Session(ICertificate certificate, ISessionContext context, INetTunnel tunnel) : base(certificate, context, tunnel)
    {
    }

    public override void OnUnactivated(INetTunnel tunnel)
    {
        if (!Certificate.IsAuthenticated())
        {
            Close();
            return;
        }
        if (IsOffline())
        {
            return;
        }
        lock (this)
        {
            if (IsOffline())
            {
                return;
            }
            var current = CurrentTunnel;
            if (current.IsActive())
            {
                return;
            }
            if (IsClosed())
            {
                return;
            }
            SetOffline();
        }
    }

    public override string ToString()
    {
        return $"Session[UserType:{ContactGroup}, UserId:${Identify}, Tunnel:{CurrentTunnel}]";
    }
}
