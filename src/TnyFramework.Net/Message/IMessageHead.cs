namespace TnyFramework.Net.Message
{
    /// <summary>
    /// 消息头
    /// </summary>
    public interface IMessageHead : IMessageSchema
    {
        /// <summary>
        /// 请求Id
        /// </summary>
        long Id { get; }

        /// <summary>
        /// 消息响应码
        /// </summary>
        int Code { get; }

        /// <summary>
        /// 请求时间
        /// </summary>
        long Time { get; }
        
        
    }
}
