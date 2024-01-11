// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

namespace TnyFramework.Net.DotNetty.Codec;

public class DataPacketV1Setting
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
