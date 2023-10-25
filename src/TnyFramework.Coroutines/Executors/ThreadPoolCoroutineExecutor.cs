// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.Threading.Tasks;

namespace TnyFramework.Coroutines.Executors
{

    /// <summary>
    /// 线程池协程运行器
    /// </summary>
    public class ThreadPoolCoroutineExecutor : ICoroutineExecutor
    {
        private ThreadPoolCoroutineExecutor()
        {
        }

        public static ICoroutineExecutor Default { get; } = new ThreadPoolCoroutineExecutor();

        public void Summit(Action action)
        {
            Task.Run(action);
        }
    }

}
