namespace TnyFramework.Net.DotNetty.Configuration.Channel
{
    public interface IDataPacketV1SettingSpec
    {
        /// <summary>
        /// 可验证
        /// </summary>
        IDataPacketV1SettingSpec VerifyEnable(bool enable);


        /// <summary>
        /// 可加密
        /// </summary>
        IDataPacketV1SettingSpec EncryptEnable(bool enable);


        /// <summary>
        /// 可加废字节
        /// </summary>
        IDataPacketV1SettingSpec WasteBytesEnable(bool enable);


        /// <summary>
        /// 最大负载长度
        /// </summary>
        IDataPacketV1SettingSpec MaxPayloadLength(int enable);


        /// <summary>
        /// 可跳过包的数量
        /// </summary>
        IDataPacketV1SettingSpec SkipNumberStep(int enable);
    }
}
