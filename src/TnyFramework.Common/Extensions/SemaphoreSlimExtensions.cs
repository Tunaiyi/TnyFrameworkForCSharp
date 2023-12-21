// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.Threading;
using System.Threading.Tasks;

namespace TnyFramework.Common.Extensions
{

    public enum WakenStatus
    {
        Ok,

        Interrupted,

        Timeout
    }

    public static class WakenStatusExtensions
    {
        public static bool IsOk(this WakenStatus self)
        {
            return self == WakenStatus.Ok;
        }

        public static bool IsTimeout(this WakenStatus self)
        {
            return self == WakenStatus.Timeout;
        }

        public static bool IsInterrupted(this WakenStatus self)
        {
            return self == WakenStatus.Interrupted;
        }

        public static bool IsAbnormal(this WakenStatus self)
        {
            return self != WakenStatus.Ok;
        }
    }

    public static class SemaphoreSlimExtensions
    {
        public static async Task<bool> SafeWaitAsync(this SemaphoreSlim self, CancellationToken cancellationToken)
        {
            try
            {
                await self.WaitAsync(cancellationToken);
                return true;
            } catch (OperationCanceledException)
            {
                return false;
            }
        }

        public static async Task<WakenStatus> SafeWaitAsync(this SemaphoreSlim self, TimeSpan timeout, CancellationToken cancellationToken)
        {
            try
            {
                return await self.WaitAsync(timeout, cancellationToken) ? WakenStatus.Ok : WakenStatus.Timeout;

            } catch (OperationCanceledException)
            {
                return WakenStatus.Interrupted;
            }
        }

        public static async Task<WakenStatus> SafeWaitAsync(this SemaphoreSlim self, int millisecondsTimeout, CancellationToken cancellationToken)
        {
            try
            {
                return await self.WaitAsync(millisecondsTimeout, cancellationToken) ? WakenStatus.Ok : WakenStatus.Timeout;
            } catch (OperationCanceledException)
            {
                return WakenStatus.Interrupted;
            }
        }
    }

}
