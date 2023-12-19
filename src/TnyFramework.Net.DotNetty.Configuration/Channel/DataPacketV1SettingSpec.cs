// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using TnyFramework.DI.Units;
using TnyFramework.Net.DotNetty.Codec;
using TnyFramework.Net.DotNetty.Configuration.Guide;

namespace TnyFramework.Net.DotNetty.Configuration.Channel
{

    public class DataPacketV1EncodeSettingSpec : DataPacketV1SettingSpec
    {
    }

    public class DataPacketV1DecodeSettingSpec : DataPacketV1SettingSpec
    {
    }

    public class DataPacketV1SettingSpec : UnitSpec<DataPacketV1Setting, INettyGuideUnitContext>, IDataPacketV1SettingSpec
    {
        public DataPacketV1Setting Setting { get; } = new DataPacketV1Setting();

        public DataPacketV1SettingSpec(string unitName = "") : base(unitName)
        {
            Default(_ => Setting);
        }

        public IDataPacketV1SettingSpec VerifyEnable(bool enable)
        {
            Setting.VerifyEnable = enable;
            return this;
        }

        public IDataPacketV1SettingSpec EncryptEnable(bool enable)
        {
            Setting.EncryptEnable = enable;
            return this;
        }

        public IDataPacketV1SettingSpec WasteBytesEnable(bool enable)
        {
            Setting.WasteBytesEnable = enable;
            return this;
        }

        public IDataPacketV1SettingSpec MaxPayloadLength(int value)
        {
            Setting.MaxPayloadLength = value;
            return this;
        }

        public IDataPacketV1SettingSpec SkipNumberStep(int value)
        {
            Setting.SkipNumberStep = value;
            return this;
        }
    }

}
