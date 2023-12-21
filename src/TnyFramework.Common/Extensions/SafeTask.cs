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

    public static class SafeTask
    {
        public static async Task<bool> Delay(TimeSpan delay, CancellationToken cancellationToken)
        {

            try
            {
                await Task.Delay(delay, cancellationToken);
                return true;
            } catch (TaskCanceledException)
            {
                return false;
            }
        }

        public static async Task<bool> Delay(int millisecondsDelay, CancellationToken cancellationToken)
        {
            try
            {
                await Task.Delay(millisecondsDelay, cancellationToken);
                return true;
            } catch (TaskCanceledException)
            {
                return false;
            }

        }
    }

}
