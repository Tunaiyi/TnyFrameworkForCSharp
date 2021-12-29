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
