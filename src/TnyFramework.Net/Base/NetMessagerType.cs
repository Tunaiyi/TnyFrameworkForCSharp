namespace TnyFramework.Net.Base
{

    /// <summary>
    /// 默认消息者类型
    /// </summary>
    public class NetMessagerType : MessagerType<NetMessagerType>
    {
        /// <summary>
        /// 匿名
        /// </summary>
        public static readonly NetMessagerType ANONYMITY = Of(0, ANONYMITY_USER_TYPE);

        /// <summary>
        /// 默认用户
        /// </summary>
        /// <returns></returns>
        public static readonly NetMessagerType DEFAULT_USER = Of(1, DEFAULT_USER_TYPE);
    }

}
