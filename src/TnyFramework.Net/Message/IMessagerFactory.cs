using TnyFramework.Net.Base;

namespace TnyFramework.Net.Message
{

    public interface IMessagerFactory
    {
        /// <summary>
        /// 创建 Messager
        /// </summary>
        /// <param name="type">消息者类型</param>
        /// <param name="messagerId">消息者id</param>
        /// <typeparam name="TM">返回创建的 messager</typeparam>
        /// <returns></returns>
        IMessager CreateMessager(IMessagerType type, long messagerId);

        /// <summary>
        /// 创建 Messager
        /// </summary>
        /// <param name="messager">转发消息者</param>
        /// <typeparam name="TM">返回创建的 messager</typeparam>
        /// <returns></returns>
        IMessager CreateMessager(ForwardMessager messager);
    }

}
