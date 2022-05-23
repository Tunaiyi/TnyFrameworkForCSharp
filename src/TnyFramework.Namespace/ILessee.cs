using System.Threading.Tasks;
using TnyFramework.Common.Event;
using TnyFramework.Namespace.Listener;

namespace TnyFramework.Namespace
{

    public interface ILessee
    {
        /// <summary>
        /// 承租信息
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 租约 id
        /// </summary>
        long Id { get; }

        /// <summary>
        /// 超时时间
        /// </summary>
        long Ttl { get; }

        /// <summary>
        /// 是否存活
        /// </summary>
        bool IsLive();

        /// <summary>
        /// 是否暂停(离线)
        /// </summary>
        bool IsPause();

        /// <summary>
        /// 是否停止
        /// </summary>
        bool IsStop();

        /// <summary>
        /// 是否正在生成租约
        /// </summary>
        bool IsGranting();

        /// <summary>
        /// 是否关闭 
        /// </summary>
        bool IsShutdown();

        /// <summary>
        /// 生成租约
        /// </summary>
        /// <returns> 返回 task </returns>
        Task<bool> Lease();

        /// <summary>
        /// 生成租约 
        /// </summary>
        /// <param name="ttl">超时时间</param>
        /// <returns>返回 task </returns>
        Task<bool> Lease(long ttl);

        /// <summary>
        /// 撤回
        /// </summary>
        /// <returns>返回 task </returns>
        Task<bool> Revoke();

        /// <summary>
        /// 关闭
        /// </summary>
        /// <returns>返回 task </returns>
        Task Shutdown();

        IEventBox<LesseeOnRenew> RenewEvent { get; }

        IEventBox<LesseeOnLease> LeaseEvent { get; }

        IEventBox<LesseeOnResume> ResumeEvent { get; }

        IEventBox<LesseeOnCompleted> CompletedEvent { get; }

        IEventBox<LesseeOnError> ErrorEvent { get; }

//    /**
// * @return 租约事件
// */
//    IEventBus<LesseeListener> event () ;
    }

}
