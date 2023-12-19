// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using TnyFramework.DI.Units;
using TnyFramework.Net.DotNetty.Codec;
using TnyFramework.Net.DotNetty.Configuration.Guide;

namespace TnyFramework.Net.DotNetty.Configuration.Channel
{

    public interface IDataPacketV1ChannelMakerSpec
    {
        DataPacketV1ChannelMakerSpec CloseOnEncodeError(bool value);

        DataPacketV1ChannelMakerSpec CloseOnDecodeError(bool value);

        DataPacketV1ChannelMakerSpec PackEncodeSetting(Action<IDataPacketV1SettingSpec> action);

        DataPacketV1ChannelMakerSpec PackDecodeSetting(Action<IDataPacketV1SettingSpec> action);

        DataPacketV1ChannelMakerSpec PackEncoder(UnitCreator<INetPacketEncoder, INettyGuideUnitContext> factory);

        DataPacketV1ChannelMakerSpec PackEncoder(Action<IUnitSpec<INetPacketEncoder, INettyGuideUnitContext>> action);

        DataPacketV1ChannelMakerSpec PackDecoder(INetPacketDecoder decoder);

        DataPacketV1ChannelMakerSpec PackDecoder(UnitCreator<INetPacketDecoder, INettyGuideUnitContext> factory);

        DataPacketV1ChannelMakerSpec PackDecoder(Action<IUnitSpec<INetPacketDecoder, INettyGuideUnitContext>> action);

        DataPacketV1ChannelMakerSpec CodecVerifier(ICodecVerifier verifier);

        DataPacketV1ChannelMakerSpec CodecVerifier(UnitCreator<ICodecVerifier, INettyGuideUnitContext> factory);

        DataPacketV1ChannelMakerSpec CodecVerifier(Action<IUnitSpec<ICodecVerifier, INettyGuideUnitContext>> action);

        DataPacketV1ChannelMakerSpec CodecCrypto(ICodecCrypto crypto);

        DataPacketV1ChannelMakerSpec CodecCrypto(UnitCreator<ICodecCrypto, INettyGuideUnitContext> factory);

        DataPacketV1ChannelMakerSpec CodecCrypto(Action<IUnitSpec<ICodecCrypto, INettyGuideUnitContext>> action);

        DataPacketV1ChannelMakerSpec ChannelPipelineChains(Action<IUnitCollectionSpec<IChannelPipelineChain, INettyGuideUnitContext>> action);
    }

}
