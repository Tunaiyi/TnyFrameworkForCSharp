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
        private volatile ConcurrentQueue<CoroutineWork> workQueue = new ConcurrentQueue<CoroutineWork>();

        private volatile ConcurrentQueue<CoroutineWork> currentFrameQueue = new ConcurrentQueue<CoroutineWork>();

        public int WorkCount => workQueue.Count;

        public bool IsWorkEmpty => workQueue.IsEmpty;

        internal void Enqueue(CoroutineWork request)
        {
            workQueue.Enqueue(request);
        }

        /// <summary>
        /// 获取当前帧队列
        /// </summary>
        /// <returns></returns>
        internal ConcurrentQueue<CoroutineWork> CurrentFrameQueue {
            get {
                if (!currentFrameQueue.IsEmpty || workQueue.IsEmpty)
                {
                    return currentFrameQueue;
                }
                (workQueue, currentFrameQueue) = (currentFrameQueue, workQueue);
                return currentFrameQueue;
            }
        }
    }

}
