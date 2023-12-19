// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using TnyFramework.Net.Configuration.Guide;
using TnyFramework.Net.DotNetty.Bootstrap;
using TnyFramework.Net.DotNetty.Codec;
using TnyFramework.Net.DotNetty.Configuration.Channel;
using TnyFramework.Net.DotNetty.Transport;

namespace TnyFramework.Net.DotNetty.Configuration.Guide
{

    public interface INettyGuideUnitContext : INetGuideUnitContext
    {
        IChannelMaker LoadChannelMaker();

        INettyTunnelFactory LoadTunnelFactory();

        IMessageCodec LoadMessageCodec();

        IMessageBodyCodec LoadMessageBodyCodec();

        IMessageHeaderCodec LoadMessageHeaderCodec();

        IDataPacketV1ChannelMakerUnitContext ChannelMakerUnitContext { get; }
    }

}
