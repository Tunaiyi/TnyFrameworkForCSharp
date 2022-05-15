using TnyFramework.Net.Dispatcher;
using TnyFramework.Net.Message;
using TnyFramework.Net.Transport;

namespace TnyFramework.Net.Plugin
{

    /// <summary>
    /// 插件接口
    /// </summary>
    public interface ICommandPlugin
    {
        /// <summary>
        /// 执行插件
        /// </summary>
        /// <param name="tunnel">管道</param>
        /// <param name="message">消息</param>
        /// <param name="context">command 上下文</param>
        /// <param name="attributes">参数</param>
        void Execute(ITunnel tunnel, IMessage message, MessageCommandContext context, object attributes);
    }

    public abstract class CommandPlugin<TUid, TAttribute> : ICommandPlugin
    {
        /// <summary>
        /// 执行插件
        /// </summary>
        /// <param name="tunnel"></param>
        /// <param name="message"></param>
        /// <param name="context"></param>
        /// <param name="attributes"></param>
        public abstract void Execute(ITunnel<TUid> tunnel, IMessage message, MessageCommandContext context, TAttribute attributes);

        public void Execute(ITunnel tunnel, IMessage message, MessageCommandContext context, object attributes)
        {
            Execute((ITunnel<TUid>) tunnel, message, context, (TAttribute) attributes);
        }
    }

}
