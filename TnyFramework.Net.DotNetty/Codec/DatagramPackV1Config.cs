//
//  文件名称：DataPacketV1Config.cs
//  简   述：配置
//  创建标识：lrg 2021/7/21
//

namespace TnyFramework.Net.DotNetty.Codec
{
    public class DataPacketV1Config
    {
        /// <summary>
        /// 可验证
        /// </summary>
        public bool VerifyEnable { get; set; } = false;

        /// <summary>
        /// 可加密
        /// </summary>
        public bool EncryptEnable { get; set; } = false;

        /// <summary>
        /// 可加废字节
        /// </summary>
        public bool WasteBytesEnable { get; set; } = false;

        /// <summary>
        /// 最大负载长度
        /// </summary>
        public int MaxPayloadLength { get; set; } = 0xFFFF;

        /// <summary>
        /// 可跳过包的数量
        /// </summary>
        public long SkipNumberStep { get; set; } = 30;
    }
}
