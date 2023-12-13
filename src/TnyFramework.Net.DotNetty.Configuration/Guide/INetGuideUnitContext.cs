// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using TnyFramework.Net.Base;
using TnyFramework.Net.Command.Dispatcher.Monitor;
using TnyFramework.Net.DotNetty.Bootstrap;
using TnyFramework.Net.DotNetty.Codec;
using TnyFramework.Net.DotNetty.Configuration.Channel;
using TnyFramework.Net.DotNetty.Transport;
using TnyFramework.Net.Message;
using TnyFramework.Net.Transport;

namespace TnyFramework.Net.DotNetty.Configuration.Guide
{

    public interface INetGuideUnitContext
    {
        IChannelMaker LoadChannelMaker();

        INettyTunnelFactory LoadTunnelFactory();

        IMessageFactory LoadMessageFactory();

        IContactFactory LoadContactFactory();

        IMessageCodec LoadMessageCodec();

        IMessageBodyCodec LoadMessageBodyCodec();

        IMessageHeaderCodec LoadMessageHeaderCodec();

        INetworkContext LoadNetworkContext();

        RpcMonitor LoadRpcMonitor();

        IDataPacketV1ChannelMakerUnitContext ChannelMakerUnitContext { get; }

        INetUnitContext UnitContext { get; }
    }

}
