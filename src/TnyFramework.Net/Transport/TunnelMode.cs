using TnyFramework.Common.Enum;
namespace TnyFramework.Net.Transport
{
    public class TunnelMode : BaseEnum<TunnelMode>
    {
        /// <summary>
        /// 服务器通道
        /// </summary>
        public static readonly TunnelMode SERVER = E(1);

        /// <summary>
        /// 客户端通道
        /// </summary>
        public static readonly TunnelMode CLIENT = E(2);
    }
}
