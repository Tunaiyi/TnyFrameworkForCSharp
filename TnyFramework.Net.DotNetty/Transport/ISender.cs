namespace TnyFramework.Net.DotNetty.Transport
{
    public interface ISender
    {
        /// <summary>
        /// 异步发送消息
        /// </summary>
        /// <param name="messageContext">发送消息上下文</param>
        /// <returns>返回发送回执</returns>
        ISendReceipt Send(MessageContext messageContext);
    }
}
