using TnyFramework.Net.DotNetty.Exception;
namespace TnyFramework.Net.DotNetty.Codec
{
    public class DataPackageContext
    {
        // 序号累计器
        private readonly DataPacketV1Config config;


        public DataPackageContext(long accessId, DataPacketV1Config config)
        {
            AccessId = accessId;
            this.config = config;
            PacketNumber = 0;
        }


        public long AccessId { get; }

        /// <summary>
        /// 包数量
        /// </summary>
        public int PacketNumber { get; private set; }


        /// <summary>
        /// 包数量++
        /// </summary>
        /// <returns></returns>
        public int NextNumber()
        {
            return ++PacketNumber;
        }


        /// <summary>
        /// 将计步器移动到第 number 个包
        /// </summary>
        /// <param name="number">目标包序号</param>
        /// <exception cref="NetCodecException"></exception>
        public void GoToAndCheck(int number)
        {
            if (PacketNumber >= number)
            {
                throw NetCodecException.CauseDecodeError($"id {number} is handled!");
            }
            var maxSkipNumber = config.SkipNumberStep;
            if (number - PacketNumber > maxSkipNumber)
            {
                throw NetCodecException.CauseDecodeError("id " + number + " is illegal!");
            }
            while (PacketNumber < number)
            {
                // if (this.random != null)
                // {
                //     this.packetCode = this.random.nextInt(Integer.MAX_VALUE);
                // }
                PacketNumber++;
            }
        }
    }
}
