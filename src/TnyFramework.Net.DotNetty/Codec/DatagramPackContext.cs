// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using TnyFramework.Net.Exceptions;

namespace TnyFramework.Net.DotNetty.Codec
{

    public class DataPackageContext
    {
        // 序号累计器
        private readonly DataPacketV1Setting setting;

        public DataPackageContext(long accessId, DataPacketV1Setting setting)
        {
            AccessId = accessId;
            this.setting = setting;
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
            var maxSkipNumber = setting.SkipNumberStep;
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
