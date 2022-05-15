namespace TnyFramework.Net.Endpoint
{

    public enum MessageHandleStrategy
    {
        ///
        /// 处理
        ///
        Handle = 1,

        ///
        /// 忽略
        ///
        Ignore = 2,

        ///
        /// 拦截抛出异常
        ///
        Throw = 3,
    }

}
