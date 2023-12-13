// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System.Threading;

namespace TnyFramework.Net.Transport
{

    public static class ConnectIdFactory
    {
        private static volatile int _TUNNEL_ID_CREATOR = 0;

        private static volatile int _TRANSPORT_ID_CREATOR = 0;

        private static volatile int _ENDPOINT_ID_CREATOR = 0;

        public static long NewEndpointId()
        {
            return Interlocked.Increment(ref _ENDPOINT_ID_CREATOR);
        }

        public static long NewTunnelId()
        {
            return Interlocked.Increment(ref _TUNNEL_ID_CREATOR);
        }

        public static long NewTransportId()
        {
            return Interlocked.Increment(ref _TRANSPORT_ID_CREATOR);
        }
    }

}
