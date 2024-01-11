// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

namespace TnyFramework.Net.DotNetty.Hosting.Channel;

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
