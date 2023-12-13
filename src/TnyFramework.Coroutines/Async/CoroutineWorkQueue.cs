// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System.Collections.Concurrent;

namespace TnyFramework.Coroutines.Async
{

    /// <summary>
    /// 协程任务队列
    /// </summary>
    public class CoroutineWorkQueue
    {
        private volatile ConcurrentQueue<ICoroutineWork> workQueue = new();

        public int WorkCount => workQueue.Count;

        public bool IsWorkEmpty => workQueue.IsEmpty;

        internal void Enqueue(ICoroutineWork request)
        {
            workQueue.Enqueue(request);
        }

        /// <summary>
        /// 获取当前帧队列
        /// </summary>
        /// <returns></returns>
        internal ConcurrentQueue<ICoroutineWork> CurrentFrameQueue => workQueue;
    }

}
