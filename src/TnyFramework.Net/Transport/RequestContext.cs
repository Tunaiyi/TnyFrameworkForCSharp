namespace TnyFramework.Net.Transport
{

    public abstract class RequestContext : MessageContext
    {
        private const long DEFAULT_FUTURE_TIMEOUT = 10000L;

        /// <summary>
        /// 设置消息 body
        /// </summary>
        /// <param name="body"> 消息body</param>
        /// <returns>返回当前 context</returns>
        public RequestContext WithRequestBody(object body)
        {
            WithBody(body);
            return this;
        }

        /// <summary>
        /// 设置响应等待者
        /// </summary>
        /// <returns>返回当前 context</returns>
        public RequestContext WillRespondAwaiter()
        {
            WillRespondAwaiter(DEFAULT_FUTURE_TIMEOUT);
            return this;
        }

        /// <summary>
        /// 设置响应等待者
        /// </summary>
        /// <param name="timeout">超时</param>
        /// <returns>返回当前 context</returns>
        public abstract RequestContext WillRespondAwaiter(long timeout);
    }

}
