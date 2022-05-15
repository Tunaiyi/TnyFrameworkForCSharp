namespace TnyFramework.Net.Base
{

    public class NetAppScope : AppScope<NetAppScope>
    {
        /// <summary>
        /// 上线
        /// </summary>
        public static readonly IAppScope ONLINE = Of(1, "online");

        /// <summary>
        /// 开发
        /// </summary>
        public static readonly IAppScope DEVELOP = Of(2, "develop");

        /// <summary>
        /// 测试
        /// </summary>
        public static readonly IAppScope TEST = Of(3, "test");
    }

}
