using System.Threading.Tasks;
using TnyFramework.Net.Message;

namespace TnyFramework.Net.Transport
{

    public interface ISendReceipt
    {
        //
        /// <summary>
        /// 获取响应 Task
        /// </summary>
        /// <returns>响应 Task</returns>
        Task<IMessage> Respond();

        /// <summary>
        /// 是否可以等待响应
        /// </summary>
        /// <returns>是否可以等待响应</returns>
        bool IsRespondAwaitable();

        /// <summary>
        /// 返回发送 Task
        /// </summary>
        /// <returns></returns>
        Task Written();
    }

}
