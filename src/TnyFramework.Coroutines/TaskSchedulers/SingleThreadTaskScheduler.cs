// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.Threading;

namespace TnyFramework.Coroutines.TaskSchedulers
{

    public class SingleDedicatedTaskScheduler : DedicatedTaskScheduler
    {
        protected SingleDedicatedTaskScheduler(string? name) : base(name)
        {
        }

        public SingleDedicatedTaskScheduler(string? name, Action<Exception>? exceptionHandler = null, int threadMaxStackSize = 0)
            : base(1, name, exceptionHandler, threadMaxStackSize)
        {
        }

        public SingleDedicatedTaskScheduler(string? name, TimeSpan? deadlockTimeout = null, Action<Exception>? exceptionHandler = null,
            int threadMaxStackSize = 0)
            : base(1, name, deadlockTimeout, exceptionHandler, threadMaxStackSize)
        {
        }

        public SingleDedicatedTaskScheduler(string? name, TimeSpan? deadlockTimeout = null, ApartmentState state = ApartmentState.Unknown,
            Action<Exception>? exceptionHandler = null, int threadMaxStackSize = 0)
            : base(1, name, deadlockTimeout, state, exceptionHandler, threadMaxStackSize)
        {
        }
    }

}
